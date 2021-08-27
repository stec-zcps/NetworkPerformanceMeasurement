// <copyright file="Sockperf.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
// Copyright 2021 Fraunhofer Institute for Manufacturing Engineering and Automation IPA
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

namespace NetworkPerformanceTestClient.ExternalTools.Sockperf
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using CommandLine;
    using NetworkPacketCapturer.KunbusTap;
    using NetworkPacketCapturer.Sockperf;
    using NetworkPerformanceShared;
    using NetworkPerformanceTestClient.Utils;
    using Newtonsoft.Json;
    using Serilog;

    /// <summary>
    /// External tool Sockperf.
    /// </summary>
    public class Sockperf : ExternalTool
    {
        /// <summary>Gets the default path to the Sockperf output log.</summary>
        public const string DefaultSockperfOutputLog = "sockperf.output.csv";

        /// <summary>Gets the default path to the Sockperf executable.</summary>
        public const string DefaultSockperfPath = "sockperf";

        /// <summary>
        /// Run Rperf test with args.
        /// </summary>
        /// <param name="args">Args from program call.</param>
        public static void RunWithArgs(string[] args)
        {
            var exitCode = Parser.Default.ParseArguments<SockperfPingPongCommandLineOptions, SockperfPlaybackCommandLineOptions, SockperfThrougputCommandLineOptions>(args).MapResult(
                    (SockperfPingPongCommandLineOptions cmdOptions) =>
                    {
                        Sockperf.LogSockerperfConfig(cmdOptions);

                        SockperfPacketCapturer sockperPacketCapturer = null;
                        if (cmdOptions.CapturePackets)
                        {
                            if (cmdOptions.KunbusTapAvailable)
                            {
                                var kunbusTapConfiguration = new KunbusTapConfiguration("A", "D");
                                sockperPacketCapturer = new SockperfPacketCapturer(ExternalTool.GetCapturingNetworkInterface(cmdOptions), kunbusTapConfiguration);
                            }
                            else
                            {
                                sockperPacketCapturer = new SockperfPacketCapturer(ExternalTool.GetCapturingNetworkInterface(cmdOptions));
                            }
                        }

                        var testCounter = 1;
                        var testCount = cmdOptions.MessagesPerSecond.ToList().Count * cmdOptions.MessageSize.ToList().Count;
                        foreach (var messagesPerSecondValue in cmdOptions.MessagesPerSecond)
                        {
                            foreach (var messageSizeValue in cmdOptions.MessageSize)
                            {
                                if (sockperPacketCapturer != null)
                                {
                                    var clientIp = NetworkUtil.GetIpOfNetworkInterface(cmdOptions.NetworkInterfaceName);
                                    sockperPacketCapturer.StartCapturing(clientIp, IPAddress.Parse(cmdOptions.ServerIp), Convert.ToUInt16(cmdOptions.Port), messageSizeValue);
                                    Thread.Sleep(1000);
                                }

                                try
                                {
                                    Log.Information($"Running test {testCounter} of {testCount} with message size {messageSizeValue} bytes and message rate {messagesPerSecondValue} messages/second");
                                    var sockperfPingPongResult = Sockperf.RunLatencyTest(cmdOptions.ServerIp, cmdOptions.Port, cmdOptions.Protocol, cmdOptions.Time, messagesPerSecondValue, messageSizeValue, cmdOptions.WaitForPong);

                                    if (sockperPacketCapturer != null)
                                    {
                                        Thread.Sleep(1000);
                                        sockperfPingPongResult.CapturedPackets = sockperPacketCapturer.StopCapturing(Sockperf.PacketAnalysisTimeout);
                                    }

                                    Log.Information($"Average Latency (ms): {sockperfPingPongResult.AverageLatency}");
                                    Log.Information($"Standard Deviation: {sockperfPingPongResult.StandardDeviationLatency}");
                                    Log.Information($"Sent Messages: {sockperfPingPongResult.SentMessages}");

                                    // Store result in database
                                    var comment = NetworkUtil.GetInterfaceDetailsForDocumentation(cmdOptions.NetworkInterfaceName);
                                    var latencyTestResult = sockperfPingPongResult.ToLatencyTestResult(DateTime.Now.ToString(), cmdOptions.TransmissionTechnology, comment, cmdOptions.KunbusTapAvailable);
                                    Program.Database.Insert(latencyTestResult);
                                }
                                catch (Exception e)
                                {
                                    Log.Error(e.Message);
                                }

                                testCounter++;
                                Thread.Sleep(5000);
                            }
                        }

                        Log.Information($"Finished running {testCount} tests");

                        return 0;
                    },
                    (SockperfPlaybackCommandLineOptions cmdOptions) =>
                    {
                        Sockperf.LogSockerperfConfig(cmdOptions);

                        SockperfPacketCapturer sockperPacketCapturer = null;
                        if (cmdOptions.CapturePackets)
                        {
                            sockperPacketCapturer = new SockperfPacketCapturer(ExternalTool.GetCapturingNetworkInterface(cmdOptions));
                        }

                        foreach (var playbackDataFile in cmdOptions.PlaybackDataFiles)
                        {
                            if (sockperPacketCapturer != null)
                            {
                                throw new NotImplementedException();

                                // sockperPacketCapturer.StartCapturing(IPAddress.Parse(cmdOptions.ClientIp), IPAddress.Parse(cmdOptions.ServerIp), Convert.ToUInt16(cmdOptions.Port), messageSizeValue);
                            }

                            try
                            {
                                var sockperfPingPongResult = Sockperf.RunLatencyTest(cmdOptions.ServerIp, cmdOptions.Port, cmdOptions.Protocol, cmdOptions.Time, playbackDataFile);

                                if (sockperPacketCapturer != null)
                                {
                                    sockperfPingPongResult.CapturedPackets = sockperPacketCapturer.StopCapturing(Sockperf.PacketAnalysisTimeout);
                                }

                                Log.Information($"Average Latency (ms): {sockperfPingPongResult.AverageLatency}");
                                Log.Information($"Standard Deviation: {sockperfPingPongResult.StandardDeviationLatency}");
                                Log.Information($"Sent Messages: {sockperfPingPongResult.SentMessages}");
                                Log.Information($"Captured Packets Count: {sockperfPingPongResult.CapturedPackets.Count}");

                                // Store result in database
                                var comment = NetworkUtil.GetInterfaceDetailsForDocumentation(cmdOptions.NetworkInterfaceName);
                                var latencyTestResult = sockperfPingPongResult.ToLatencyTestResult(DateTime.Now.ToString(), cmdOptions.TransmissionTechnology, comment, cmdOptions.KunbusTapAvailable);
                                Program.Database.Insert(latencyTestResult);
                            }
                            catch (ExternalException e)
                            {
                                Log.Error(e.Message);
                            }

                            Thread.Sleep(5000);
                        }

                        return 0;
                    },
                    (SockperfThrougputCommandLineOptions cmdOptions) =>
                    {
                        Sockperf.LogSockerperfConfig(cmdOptions);

                        try
                        {
                            var sockperfThroughputResult = Sockperf.RunThrougput(cmdOptions.ServerIp, cmdOptions.Port, cmdOptions.Protocol, cmdOptions.Time);

                            Log.Information($"Message Rate [msg/s]: {sockperfThroughputResult.MessageRatePerSecond}");
                            Log.Information($"Bandwidth [Mbps]: {sockperfThroughputResult.BandWidthInMbps}");

                            // Store result in database
                            var comment = NetworkUtil.GetInterfaceDetailsForDocumentation(cmdOptions.NetworkInterfaceName);
                            var throughputTestResult = sockperfThroughputResult.ToThroughputTestResult(DateTime.Now.ToString(), cmdOptions.TransmissionTechnology, comment);
                            Program.Database.DatabaseContext.Add(throughputTestResult);
                            Program.Database.DatabaseContext.SaveChanges();
                        }
                        catch (ExternalException e)
                        {
                            Log.Error(e.Message);
                        }

                        // sockperfResults.Add(sockperfThroughputResult);
                        return 0;
                    },
                    errs => HandleParseError(errs));
        }

        /// <summary>
        /// Run latency test.
        /// </summary>
        /// <param name="serverIp">IP address of Sockperf server.</param>
        /// <param name="serverPort">Port of Sockperf server.</param>
        /// <param name="protocol">Protocol used for packet transmission.</param>
        /// <param name="time">test duration.</param>
        /// <param name="messagesPerSecond">Messages send per second.</param>
        /// <param name="messageSize">Size of send messages in bytes.</param>
        /// <param name="waitForPong">Value indicating if Sockperf should wait for pong before sending next ping packet.</param>
        /// <param name="sockperfPath">Path to Sockperf executable.</param>
        /// <param name="sockperfOutputLog">Path to Sockperf output log.</param>
        /// <returns><see cref="SockperfLatencyResult"/> from test run.</returns>
        public static SockperfLatencyResult RunLatencyTest(
            string serverIp,
            int serverPort,
            string protocol,
            int time,
            int messagesPerSecond,
            int messageSize,
            bool waitForPong,
            string sockperfPath = Sockperf.DefaultSockperfPath,
            string sockperfOutputLog = Sockperf.DefaultSockperfOutputLog)
        {
            return RunLatencyTest(serverIp, serverPort, protocol, time, messagesPerSecond, messageSize, waitForPong, string.Empty, sockperfPath, sockperfOutputLog);
        }

        /// <summary>
        /// Run latency test from playback file.
        /// </summary>
        /// <param name="serverIp">IP address of Sockperf server.</param>
        /// <param name="serverPort">Port of Sockperf server.</param>
        /// <param name="protocol">Protocol used for packet transmission.</param>
        /// <param name="time">test duration.</param>
        /// <param name="playbackFilePath">Playback file which should be used by Sockperf.</param>
        /// <param name="sockperfPath">Path to Sockperf executable.</param>
        /// <param name="sockperfOutputLog">Path to Sockperf output log.</param>
        /// <returns><see cref="SockperfLatencyResult"/> from test run.</returns>
        public static SockperfLatencyResult RunLatencyTest(
            string serverIp,
            int serverPort,
            string protocol,
            int time,
            string playbackFilePath = "",
            string sockperfPath = Sockperf.DefaultSockperfPath,
            string sockperfOutputLog = Sockperf.DefaultSockperfOutputLog)
        {
            return RunLatencyTest(serverIp, serverPort, protocol, time, -1, -1, false, playbackFilePath, sockperfPath, sockperfOutputLog);
        }

        /// <summary>
        /// Run througput test.
        /// </summary>
        /// <param name="serverIp">IP address of Sockperf server.</param>
        /// <param name="serverPort">Port of Sockperf server.</param>
        /// <param name="protocol">Protocol used for packet transmission.</param>
        /// <param name="time">Test duration.</param>
        /// <param name="sockperfPath">Path to Sockperf executable.</param>
        /// <returns><see cref="SockperfThrougputResult"/> of the test run.</returns>
        public static SockperfThrougputResult RunThrougput(
            string serverIp,
            int serverPort,
            string protocol,
            int time,
            string sockperfPath = Sockperf.DefaultSockperfPath)
        {
            if (!(protocol.Equals("tcp") || protocol.Equals("udp")))
            {
                throw new ArgumentException("Protocl for SockPerf must be 'tcp' or 'udp'");
            }

            var command = $"{sockperfPath} throughput " +
                $"--ip {serverIp} " +
                $"--port {serverPort} " +
                $"{(protocol.Equals("tcp") ? "--tcp" : string.Empty)} " +
                $"--time {time} " +
                $"--mps max " +
                $"--msg-size 1472";
            Log.Information($"SockPerf Command: {command}");
            var output = command.Bash();
            if (output.Contains("Usage: sockperf"))
            {
                throw new ExternalException($"Sockperf call failed: {output}");
            }
            else
            {
                Log.Information($"SockPerf Output\n: {output}");
            }

            var testParameters = new SockperfThrougputTestParameters(serverIp, serverPort, time, protocol, 1472);
            var result = SockperfThrougputResult.FromConsoleOutput(testParameters, output);

            return result;
        }

        private static SockperfLatencyResult RunLatencyTest(
            string serverIp,
            int serverPort,
            string protocol,
            int time,
            int messagesPerSecond,
            int messageSize,
            bool waitForPong,
            string playbackFilePath,
            string sockperfPath,
            string sockperfOutputLog)
        {
            if (!(protocol.Equals("tcp") || protocol.Equals("udp")))
            {
                throw new ArgumentException("Protocl for SockPerf must be 'tcp' or 'udp'");
            }

            // If playback file path...
            bool isPingPongRun;
            bool deletePlaybackFile = false;
            if (string.IsNullOrEmpty(playbackFilePath))
            {
                // ... is empty ...
                if (waitForPong)
                {
                    // ... and wait for pong is enabled, it is a ping pong run
                    isPingPongRun = true;
                }
                else
                {
                    // .. and wait for pong is not enabled, playback mode must be used to avoid limitations of ping pong mode
                    isPingPongRun = false;

                    var messageInterval = 1D / messagesPerSecond;
                    var numberOfEntries = time * messagesPerSecond;
                    var currentTime = 1D;

                    var csvPlaybackFile = new StringBuilder();
                    for (int i = 0; i < numberOfEntries; i++)
                    {
                        csvPlaybackFile.AppendLine($"{currentTime.ToString("n15", CultureInfo.InvariantCulture)}, {messageSize}");
                        currentTime += messageInterval;
                    }

                    playbackFilePath = @"pingPongPlayback.csv";
                    File.WriteAllText(playbackFilePath, csvPlaybackFile.ToString());
                    deletePlaybackFile = true;
                }
            }
            else
            {
                // ... is not empty, it is a playback run
                isPingPongRun = false;
            }

            isPingPongRun = true;

            var pingPongSpecificArgs = $"--time {time} --mps {messagesPerSecond} --msg-size {messageSize} --reply-every 1";
            var playbackSpecificArgs = $"--data-file {playbackFilePath} --reply-every 1";
            var command = $"{sockperfPath} " +
                $"{(isPingPongRun ? "under-load" : "playback")} " +
                $"--ip {serverIp} --port {serverPort} " +
                $"{(protocol.Equals("tcp") ? "--tcp" : string.Empty)} " +
                $"--full-log {sockperfOutputLog} " +
                $"{(isPingPongRun ? $"{pingPongSpecificArgs}" : $"{playbackSpecificArgs}")}";
            Log.Information($"SockPerf Command: {command}");
            var output = command.Bash();
            if (output.Contains("Usage: sockperf"))
            {
                throw new ExternalException($"Sockperf call failed: {output}");
            }
            else
            {
                Log.Information($"SockPerf Output\n: {output}");
            }

            SockperfTestParameters testParameters;
            if (isPingPongRun || deletePlaybackFile)
            {
                testParameters = new SockperfPingPongTestParameters(serverIp, serverPort, time, protocol, messageSize, messagesPerSecond);
            }
            else
            {
                testParameters = new SockperfPlaybackTestParameters(serverIp, serverPort, time, protocol, playbackFilePath);
            }

            var result = SockperfLatencyResult.FromOutputAndLog(testParameters, output, sockperfOutputLog);

            if (deletePlaybackFile)
            {
                File.Delete(playbackFilePath);
            }

            return result;
        }

        private static int HandleParseError(IEnumerable<Error> errs)
        {
            return -1;
        }

        private static void LogSockerperfConfig(SockperfCommandLineOptions cmdOptions)
        {
            Log.Information($"Server: {cmdOptions.ServerIp}:{cmdOptions.Port}");
            Log.Information($"Protocol: {cmdOptions.Protocol}");
            Log.Information($"Time: {cmdOptions.Time}");
            if (cmdOptions is SockperfPingPongCommandLineOptions)
            {
                var cmdOptionsPingPong = (SockperfPingPongCommandLineOptions)cmdOptions;
                Log.Information($"Messages Per Second: {JsonConvert.SerializeObject(cmdOptionsPingPong.MessagesPerSecond)}");
                Log.Information($"Message Size: {JsonConvert.SerializeObject(cmdOptionsPingPong.MessageSize)}");
            }
        }
    }
}
