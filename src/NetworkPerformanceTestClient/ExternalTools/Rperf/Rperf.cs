// <copyright file="Rperf.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPerformanceTestClient.ExternalTools.Owping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using CommandLine;
    using NetworkPacketCapturer.KunbusTap;
    using NetworkPacketCapturer.Rperf;
    using NetworkPerformanceShared;
    using NetworkPerformanceTestClient.Utils;
    using Newtonsoft.Json;
    using Serilog;

    /// <summary>
    /// External tool Rperf.
    /// </summary>
    public class Rperf : ExternalTool
    {
        /// <summary>Gets the default path to the Rperf output log.</summary>
        public const string DefaultRperfOutputLog = "rperf.output.csv";

        /// <summary>Gets the default path to the rperf executable.</summary>
        public const string DefaultRperfPath = @"/home/ipa/Development/rperf_remote/target/release/rperf";

        /// <summary>
        /// Run Rperf test with args.
        /// </summary>
        /// <param name="args">Args from program call.</param>
        public static void RunWithArgs(string[] args)
        {
            var exitCode = Parser.Default.ParseArguments<RperfCommandLineOptions>(args).MapResult(
                    (RperfCommandLineOptions cmdOptions) =>
                    {
                        Rperf.LogConfig(cmdOptions);

                        RperfPacketCapturer packetCapturer = null;
                        if (cmdOptions.CapturePackets)
                        {
                            if (cmdOptions.KunbusTapAvailable)
                            {
                                var kunbusTapConfiguration = new KunbusTapConfiguration("A", "D");
                                packetCapturer = new RperfPacketCapturer(ExternalTool.GetCapturingNetworkInterface(cmdOptions), kunbusTapConfiguration);
                            }
                            else
                            {
                                packetCapturer = new RperfPacketCapturer(ExternalTool.GetCapturingNetworkInterface(cmdOptions));
                            }
                        }

                        var testCounter = 1;
                        var testCount = cmdOptions.MessagesPerSecond.ToList().Count * cmdOptions.MessageSize.ToList().Count;
                        foreach (var messagesPerSecondValue in cmdOptions.MessagesPerSecond)
                        {
                            foreach (var messageSizeValue in cmdOptions.MessageSize)
                            {
                                if (packetCapturer != null)
                                {
                                    var clientIp = NetworkUtil.GetIpOfNetworkInterface(cmdOptions.NetworkInterfaceName);
                                    packetCapturer.StartCapturing(clientIp, IPAddress.Parse(cmdOptions.ServerIp), Convert.ToUInt16(cmdOptions.Port), messageSizeValue);
                                    Thread.Sleep(1000);
                                }

                                try
                                {
                                    Log.Information($"Running test {testCounter} of {testCount} with message size {messageSizeValue} bytes and message rate {messagesPerSecondValue} messages/second");
                                    var rperfResult = Rperf.RunPingPong(
                                        cmdOptions.ServerIp,
                                        cmdOptions.Port,
                                        cmdOptions.Time,
                                        messagesPerSecondValue,
                                        messageSizeValue,
                                        cmdOptions.WarmupTime,
                                        cmdOptions.Protocol,
                                        cmdOptions.MeasureOneWayLatency,
                                        cmdOptions.CpuAffinity.ToList());

                                    if (packetCapturer != null)
                                    {
                                        Thread.Sleep(1000);
                                        rperfResult.CapturedPackets = packetCapturer.StopCapturing(Rperf.PacketAnalysisTimeout);
                                    }

                                    Log.Information($"Average Latency (ms): {rperfResult.AverageLatency}");
                                    Log.Information($"Sent Messages: {rperfResult.SentMessages}");
                                    Log.Information($"Captured Packets Count: {rperfResult.CapturedPackets.Count}");

                                    // Store result in database
                                    string comment;

                                    if (string.IsNullOrEmpty(cmdOptions.Comment))
                                    {
                                        comment = NetworkUtil.GetInterfaceDetailsForDocumentation(cmdOptions.NetworkInterfaceName);
                                    }
                                    else
                                    {
                                        comment = cmdOptions.Comment;
                                    }

                                    var latencyTestResult = rperfResult.ToLatencyTestResult(DateTime.Now.ToString(), cmdOptions.TransmissionTechnology, comment, cmdOptions.KunbusTapAvailable);
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

                        return 0;
                    },
                    errs => HandleParseError(errs));
        }

        /// <summary>
        /// Run Rperf ping pong test.
        /// </summary>
        /// <param name="serverIp">IP address of the Rperf server.</param>
        /// <param name="serverPort">Port of the Rperf server.</param>
        /// <param name="time">Test duration.</param>
        /// <param name="messagesPerSecond">Messages send poer second.</param>
        /// <param name="messageSize">Size of send messages.</param>
        /// <param name="warmupTime">Warmup time before test.</param>
        /// <param name="protocol">Protocol used for packet transmission.</param>
        /// <param name="measureOneWayLatency">Value indicating if one way latency instead of round trip time should be measured.</param>
        /// <param name="cpuAffinity">List with CPU core ids to execute Rperf.</param>
        /// <param name="rperfPath">Path to Rperf executable.</param>
        /// <param name="rperfOutputLog">Path to Rperf output log.</param>
        /// <returns><see cref="RperfResult"/> from test run.</returns>
        public static RperfResult RunPingPong(
            string serverIp,
            int serverPort,
            int time,
            int messagesPerSecond,
            int messageSize,
            int warmupTime,
            string protocol,
            bool measureOneWayLatency,
            List<int> cpuAffinity,
            string rperfPath = Rperf.DefaultRperfPath,
            string rperfOutputLog = Rperf.DefaultRperfOutputLog)
        {
            var cmd = $"{rperfPath}";
            if (cpuAffinity.Count > 0)
            {
                cmd = $"taskset --cpu-list {string.Join(",", cpuAffinity)} " + cmd;
            }

            var commandArguments = $"client " +
                $"-i {serverIp} " +
                $"--port {serverPort} " +
                $"--size {messageSize} " +
                $"--mps {messagesPerSecond} " +
                $"-t {time} " +
                $"{(warmupTime > 0 ? $"--warmup {warmupTime}" : string.Empty)} " +
                $"--protocol {protocol} " +
                $"{(measureOneWayLatency ? "--owl" : string.Empty)} " +
                $"--rtt " +
                $"--log {rperfOutputLog}";
            Log.Information($"rperf command\n: {cmd} {commandArguments}");
            var output = cmd.Bash(commandArguments);
            Log.Information($"rperf Output\n: {output}");

            var testParameters = new RperfTestParameters(serverIp, serverPort, time, messageSize, messagesPerSecond, protocol);
            var result = RperfResult.FromOutputAndLog(testParameters, output, rperfOutputLog);

            return result;
        }

        private static int HandleParseError(IEnumerable<Error> errs)
        {
            return -1;
        }

        private static void LogConfig(CommandLineOptions genericCmdOptions)
        {
            RperfCommandLineOptions cmdOptions = (RperfCommandLineOptions)genericCmdOptions;

            Log.Information($"Server: {cmdOptions.ServerIp}:{cmdOptions.Port}");
            Log.Information($"Time: {cmdOptions.Time}");
            Log.Information($"Messages Per Second: {JsonConvert.SerializeObject(cmdOptions.MessagesPerSecond)}");
            Log.Information($"Message Size: {JsonConvert.SerializeObject(cmdOptions.MessageSize)}");
        }
    }
}
