// <copyright file="RperfResult.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NetworkPerformanceShared;
    using NetworkPerformanceTestClient.ExternalTools.Common;
    using Newtonsoft.Json;
    using Serilog;

    /// <summary>
    /// Result of a Rperf test run.
    /// </summary>
    public class RperfResult : PingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RperfResult"/> class.
        /// </summary>
        /// <param name="testParameters">Parameters used for testing.</param>
        /// <param name="rawStandardOutput">Raw standrad output of Rperf.</param>
        /// <param name="measurments">Measurments of test run.</param>
        public RperfResult(RperfTestParameters testParameters, string rawStandardOutput, List<PingMeasurment> measurments)
        {
            this.TestParameters = testParameters;
            this.RawStandardOutput = rawStandardOutput;
            this.Measurments = measurments.OrderBy(m => m.Index).ToList();

            this.MinimumLatency = this.Measurments.Min(item => item.LatencyOverall);
            this.MaximumLatency = this.Measurments.Max(item => item.LatencyOverall);
            this.AverageLatency = this.Measurments.Average(item => item.LatencyOverall);
            this.StandardDeviationLatency = CalculationUtils.GetStandardDeviation(this.Measurments.Select(item => item.LatencyOverall));
        }

        /// <summary>Gets or sets the count of sent messages.</summary>
        public override int SentMessages { get; set; }

        /// <summary>Gets or sets the count of received messages.</summary>
        public override int ReceivedMessages { get; set; }

        /// <summary>Gets or sets the count of lost messages.</summary>
        public override int LostMessages { get; set; }

        /// <summary>
        /// Create <see cref="RperfResult"/> from Rperf output and full log file (csv).
        /// </summary>
        /// <param name="testParameters">Parameters used for testing.</param>
        /// <param name="consoleOutput">Output of Rperf.</param>
        /// <param name="logPath">Path to log file of Rperf.</param>
        /// <returns><see cref="RperfResult"/> created from Rperf output and full log file (csv).</returns>
        public static RperfResult FromOutputAndLog(RperfTestParameters testParameters, string consoleOutput, string logPath)
        {
            try
            {
                using (var reader = new StreamReader(@$"{logPath}"))
                {
                    var measurments = new List<PingMeasurment>();

                    Log.Verbose("First lines in CSV:");
                    string[] lines = new string[3];
                    for (int i = 0; i < 3; i++)
                    {
                        lines[i] = reader.ReadLine();
                        Log.Verbose($"{lines[i]}");
                    }

                    MatchCollection matches = Regex.Matches(lines[1], @"Test Results: Sent Duration \[s\]: ([0-9]+\.[0-9]+) \| Sent Packets: ([0-9]+), Received Packets: ([0-9]+), Lost Packets: ([0-9]+), Average Latency \[ms\]: ([0-9]+\.[0-9]+)");
                    if (matches.Count == 1)
                    {
                        if (matches[0].Groups.Count == 6)
                        {
                            var sentDuration = double.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                            var sentPacketCount = int.Parse(matches[0].Groups[2].Value);
                            var receivedPacketCount = int.Parse(matches[0].Groups[3].Value);
                            var lostPacketPacketCount = int.Parse(matches[0].Groups[4].Value);
                            var averageLatency = double.Parse(matches[0].Groups[5].Value, CultureInfo.InvariantCulture);

                            while (!reader.EndOfStream)
                            {
                                var line = reader.ReadLine();
                                var values = line.Split(',');

                                if (values.Length == 6)
                                {
                                    try
                                    {
                                        var index = int.Parse(values[0]);
                                        var sendTime = double.Parse(values[1], CultureInfo.InvariantCulture);
                                        var receiveTime = double.Parse(values[2], CultureInfo.InvariantCulture);
                                        var latencyOverall = float.Parse(values[3], CultureInfo.InvariantCulture);
                                        var latencyClientToServer = float.Parse(values[4], CultureInfo.InvariantCulture);
                                        var latencyServerToClient = float.Parse(values[5], CultureInfo.InvariantCulture);
                                        var measurment = new PingMeasurment(index, sendTime, receiveTime, latencyOverall, latencyClientToServer, latencyServerToClient);
                                        measurments.Add(measurment);
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error($"{e.Message}\nValues: {JsonConvert.SerializeObject(values)}");
                                    }
                                }
                            }

                            File.Delete(logPath);

                            var result = new RperfResult(testParameters, consoleOutput, measurments);
                            result.SentMessages = sentPacketCount;
                            result.ReceivedMessages = receivedPacketCount;
                            result.LostMessages = lostPacketPacketCount;

                            return result;
                        }
                    }

                    throw new Exception($"Unable to parse output");
                }
            }
            catch (Exception e)
            {
                Log.Error($"Rperf Output: {consoleOutput}");
                throw e;
            }
        }
    }
}