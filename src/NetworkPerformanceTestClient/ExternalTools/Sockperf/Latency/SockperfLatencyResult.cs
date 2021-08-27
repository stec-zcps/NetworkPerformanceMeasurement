// <copyright file="SockperfLatencyResult.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Text.RegularExpressions;
    using NetworkPerformanceShared;
    using NetworkPerformanceTestClient.ExternalTools.Common;
    using NetworkPerformanceTestClient.ExternalTools.Owping;
    using Newtonsoft.Json;
    using Serilog;

    /// <summary>
    /// Result of a Sockperf test run to measure latency.
    /// </summary>
    public class SockperfLatencyResult : PingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SockperfLatencyResult"/> class.
        /// </summary>
        public SockperfLatencyResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SockperfLatencyResult"/> class.
        /// </summary>
        /// <param name="testParameters">Parameters used for testing.</param>
        /// <param name="rawStandardOutput">Raw standrad output of sockperf.</param>
        /// <param name="measurments">Measurments of test run.</param>
        public SockperfLatencyResult(SockperfTestParameters testParameters, string rawStandardOutput, List<PingMeasurment> measurments)
        {
            this.TestParameters = testParameters;
            this.RawStandardOutput = rawStandardOutput;
            this.Measurments = measurments.OrderBy(m => m.Index).ToList();

            this.MinimumLatency = this.Measurments.Min(item => item.LatencyOverall);
            this.MaximumLatency = this.Measurments.Max(item => item.LatencyOverall);
            this.AverageLatency = this.Measurments.Average(item => item.LatencyOverall);
            this.StandardDeviationLatency = CalculationUtils.GetStandardDeviation(this.Measurments.Select(item => item.LatencyOverall));
        }

        /// <inheritdoc/>
        public override int SentMessages
        {
            get { return SockperfLatencyResult.GetSentMessagesValidFromOutput(this.RawStandardOutput); } set { }
        }

        /// <inheritdoc/>
        public override int ReceivedMessages
        {
            get { return SockperfLatencyResult.GetReceivedMessagesValidFromOutput(this.RawStandardOutput); } set { }
        }

        /// <inheritdoc/>
        public override int LostMessages
        {
            get { return this.SentMessages - this.ReceivedMessages; } set { }
        }

        /// <summary>
        /// Create <see cref="SockperfLatencyResult"/> from Sockperf output and full log file (csv).
        /// </summary>
        /// <param name="testParameters">Parameters used for testing.</param>
        /// <param name="consoleOutput">Output of Sockperf.</param>
        /// <param name="logPath">Path to log file of Sockperf.</param>
        /// <returns><see cref="SockperfLatencyResult"/> created from Sockperf output and full log file (csv).</returns>
        public static SockperfLatencyResult FromOutputAndLog(SockperfTestParameters testParameters, string consoleOutput, string logPath)
        {
            try
            {
                var sentMessagesValid = SockperfLatencyResult.GetSentMessagesValidFromOutput(consoleOutput);
                var sentMessagesTotal = SockperfLatencyResult.GetSentMessagesTotalFromOutput(consoleOutput);
                var receivedMessagesValid = SockperfLatencyResult.GetReceivedMessagesValidFromOutput(consoleOutput);
                var receivedMessagesTotal = SockperfLatencyResult.GetReceivedMessagesTotalFromOutput(consoleOutput);

                using (var reader = new StreamReader(@$"{logPath}"))
                {
                    var measurments = new List<PingMeasurment>();

                    Log.Verbose("First lines in CSV:");
                    for (int i = 0; i < 21; i++)
                    {
                        string line = reader.ReadLine();
                        Log.Verbose($"{line}");
                    }

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(',');

                        if (values.Length == 4)
                        {
                            try
                            {
                                var index = long.Parse(values[0]);

                                // Shift index to match logged packets with captured packets
                                // --> Only valid packets are stored in .csv, but counter in packet is increased by warumup packets
                                index += sentMessagesTotal - receivedMessagesValid + 1;
                                Log.Information($"Index Shift: {sentMessagesTotal - receivedMessagesValid + 1}");
                                var sendTime = double.Parse(values[1], CultureInfo.InvariantCulture);
                                var receiveTime = double.Parse(values[2], CultureInfo.InvariantCulture);
                                var latency = float.Parse(values[3], CultureInfo.InvariantCulture) / 1000;
                                var measurment = new PingMeasurment(index, sendTime, receiveTime, latency, -1, -1);
                                measurments.Add(measurment);
                            }
                            catch (Exception e)
                            {
                                Log.Error($"{e.Message}\nValues: {JsonConvert.SerializeObject(values)}");
                            }
                        }
                    }

                    // File.Delete(logPath);
                    var result = new SockperfLatencyResult(testParameters, consoleOutput, measurments);
                    return result;
                }
            }
            catch (Exception e)
            {
                Log.Error($"SockPerf Output: {consoleOutput}");
                throw e;
            }
        }

        /// <summary>
        /// Get sent messages valid from Sockperf output.
        /// </summary>
        /// <param name="sockperfOutput">Sockperf output.</param>
        /// <returns>Valid sent messages from Sockperf output.</returns>
        public static int GetSentMessagesValidFromOutput(string sockperfOutput)
        {
            var sentMessages = -1;

            MatchCollection matchesSendMessages = Regex.Matches(sockperfOutput, @"\[Valid Duration\] RunTime=[0-9]+.[0-9]+ sec; SentMessages=([0-9]+);");
            if (matchesSendMessages.Count == 1)
            {
                if (matchesSendMessages[0].Groups.Count == 2)
                {
                    var sentMessagesString = matchesSendMessages[0].Groups[1].Value;
                    sentMessages = int.Parse(sentMessagesString);
                }
                else
                {
                    Log.Error($"Regcular Expression Group Count of Match should be 2 and not {matchesSendMessages[0].Groups.Count}");
                }
            }
            else
            {
                Log.Error($"Regcular Expression Match Count should be 1 and not {matchesSendMessages.Count}");
            }

            return sentMessages;
        }

        /// <summary>
        /// Get sent messages total from Sockperf output.
        /// </summary>
        /// <param name="sockperfOutput">Sockperf output.</param>
        /// <returns>Total sent messages from Sockperf output.</returns>
        public static int GetSentMessagesTotalFromOutput(string sockperfOutput)
        {
            var sentMessages = -1;

            MatchCollection matchesSendMessages = Regex.Matches(sockperfOutput, @"\[Total Run\] RunTime=[0-9]+.[0-9]+ sec; Warm up time=[0-9]+ msec; SentMessages=([0-9]+);");
            if (matchesSendMessages.Count == 1)
            {
                if (matchesSendMessages[0].Groups.Count == 2)
                {
                    var sentMessagesString = matchesSendMessages[0].Groups[1].Value;
                    sentMessages = int.Parse(sentMessagesString);
                }
                else
                {
                    Log.Error($"Regcular Expression Group Count of Match should be 2 and not {matchesSendMessages[0].Groups.Count}");
                }
            }
            else
            {
                Log.Error($"Regcular Expression Match Count should be 1 and not {matchesSendMessages.Count}");
            }

            return sentMessages;
        }

        /// <summary>
        /// Get received messages valid from Sockperf output.
        /// </summary>
        /// <param name="sockperfOutput">Sockperf output.</param>
        /// <returns>Valid received messages from Sockperf output.</returns>
        public static int GetReceivedMessagesValidFromOutput(string sockperfOutput)
        {
            var receivedMessages = -1;

            MatchCollection matchesReceivedMessages = Regex.Matches(sockperfOutput, @"\[Valid Duration\] RunTime=[0-9]+.[0-9]+ sec; SentMessages=[0-9]+; ReceivedMessages=([0-9]+)");
            if (matchesReceivedMessages.Count == 1)
            {
                if (matchesReceivedMessages[0].Groups.Count == 2)
                {
                    var receivedMessagesString = matchesReceivedMessages[0].Groups[1].Value;
                    receivedMessages = int.Parse(receivedMessagesString);
                }
                else
                {
                    Log.Error($"Regcular Expression Group Count of Match should be 2 and not {matchesReceivedMessages[0].Groups.Count}");
                }
            }
            else
            {
                Log.Error($"Regcular Expression Match Count should be 1 and not {matchesReceivedMessages.Count}");
            }

            return receivedMessages;
        }

        /// <summary>
        /// Get received messages total from Sockperf output.
        /// </summary>
        /// <param name="sockperfOutput">Sockperf output.</param>
        /// <returns>Total received messages from Sockperf output.</returns>
        public static int GetReceivedMessagesTotalFromOutput(string sockperfOutput)
        {
            var receivedMessages = -1;

            MatchCollection matchesReceivedMessages = Regex.Matches(sockperfOutput, @"\[Total Run\] RunTime=[0-9]+.[0-9]+ sec; Warm up time=[0-9]+ msec; SentMessages=[0-9]+; ReceivedMessages=([0-9]+)");
            if (matchesReceivedMessages.Count == 1)
            {
                if (matchesReceivedMessages[0].Groups.Count == 2)
                {
                    var receivedMessagesString = matchesReceivedMessages[0].Groups[1].Value;
                    receivedMessages = int.Parse(receivedMessagesString);
                }
                else
                {
                    Log.Error($"Regcular Expression Group Count of Match should be 2 and not {matchesReceivedMessages[0].Groups.Count}");
                }
            }
            else
            {
                Log.Error($"Regcular Expression Match Count should be 1 and not {matchesReceivedMessages.Count}");
            }

            return receivedMessages;
        }
    }
}
