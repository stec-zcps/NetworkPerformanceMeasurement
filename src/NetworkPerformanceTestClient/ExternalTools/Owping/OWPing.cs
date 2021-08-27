// <copyright file="OWPing.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Threading;
    using CommandLine;
    using NetworkPerformanceShared;
    using Newtonsoft.Json;
    using Serilog;

    /// <summary>
    /// External tool owping.
    /// </summary>
    public class OWPing : ExternalTool
    {
        /// <summary>Gets the default path to the owping executable.</summary>
        public const string DefaultOWPingPath = "owping";

        /// <summary>
        /// Run owping test with args.
        /// </summary>
        /// <param name="args">Args from program call.</param>
        public static void RunWithArgs(string[] args)
        {
            var exitCode = Parser.Default.ParseArguments<OWPingCommandLineOptions>(args).MapResult(
                    (OWPingCommandLineOptions cmdOptions) =>
                    {
                        OWPing.LogOWPingConfig(cmdOptions);

                        // SockperfPacketCapturer sockperPacketCapturer = null;
                        // if (cmdOptions.CapturePackets)
                        // {
                        //    sockperPacketCapturer = new SockperfPacketCapturer(ExternalTool.GetCapturingNetworkInterface(cmdOptions));
                        // }
                        foreach (var messagesPerSecondValue in cmdOptions.MessagesPerSecond)
                        {
                            foreach (var messageSizeValue in cmdOptions.MessageSize)
                            {
                                // if (sockperPacketCapturer != null)
                                // {
                                //    sockperPacketCapturer.StartCapturing(IPAddress.Parse(cmdOptions.ClientIp), IPAddress.Parse(cmdOptions.ServerIp), Convert.ToUInt16(cmdOptions.Port), messageSizeValue);
                                // }
                                var owpingResult = OWPing.RunPing(cmdOptions.ServerIp, cmdOptions.Port, cmdOptions.Time, messagesPerSecondValue, messageSizeValue);

                                // if (sockperPacketCapturer != null)
                                // {
                                //    Thread.Sleep(1000);

                                // sockperfPingPongResult.CapturedSockperfPackets = sockperPacketCapturer.CapturedSockperfPackets;
                                // }
                                Log.Information($"Average Latency (ms): {owpingResult.AverageLatency}");
                                Log.Information($"Sent Messages: {owpingResult.SentMessages}");
                                Log.Information($"Captured Packets Count: {owpingResult.CapturedPackets.Count}");

                                // Store result in database
                                var latencyTestResult = owpingResult.ToLatencyTestResult(DateTime.Now.ToString(), cmdOptions.TransmissionTechnology, cmdOptions.Comment);
                                Program.Database.Insert(latencyTestResult);

                                Thread.Sleep(5000);
                            }
                        }

                        return 0;
                    },
                    errs => HandleParseError(errs));
        }

        /// <summary>
        /// Run owping test.
        /// </summary>
        /// <param name="serverIp">IP address of the owping server.</param>
        /// <param name="serverPort">Port of the iowping server.</param>
        /// <param name="time">Test duration.</param>
        /// <param name="messagesPerSecond">Messages that should be send per second.</param>
        /// <param name="messageSize">Size of the messages in bytes.</param>
        /// <param name="owpingPath">Path to owping executable.</param>
        /// <returns>The result of the test run.</returns>
        public static OWPingResult RunPing(
            string serverIp,
            int serverPort,
            int time,
            int messagesPerSecond,
            int messageSize,
            string owpingPath = OWPing.DefaultOWPingPath)
        {
            var command = $"{owpingPath} {serverIp} -s {messageSize} -i {1 / messagesPerSecond} -c {time * messagesPerSecond} -v -U -t";
            Log.Information($"owping command\n: {command}");
            var output = command.Bash();
            Log.Information($"owping Output\n: {output}");

            var testParameters = new OWPingTestParameters(serverIp, serverPort, time, messageSize, messagesPerSecond);
            var result = OWPingResult.FromOutput(testParameters, output);

            return result;
        }

        private static int HandleParseError(IEnumerable<Error> errs)
        {
            return -1;
        }

        private static void LogOWPingConfig(OWPingCommandLineOptions cmdOptions)
        {
            Log.Information($"Server: {cmdOptions.ServerIp}:{cmdOptions.Port}");
            Log.Information($"Time: {cmdOptions.Time}");
            Log.Information($"Messages Per Second: {JsonConvert.SerializeObject(cmdOptions.MessagesPerSecond)}");
            Log.Information($"Message Size: {JsonConvert.SerializeObject(cmdOptions.MessageSize)}");
        }
    }
}
