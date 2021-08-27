// <copyright file="SockperfThrougputResult.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Globalization;
    using System.Text.RegularExpressions;
    using NetworkPerformanceShared.Model;
    using NetworkPerformanceTestClient.ExternalTools.Common;
    using Serilog;

    /// <summary>
    /// Result of a Sockperf test run to measure throughput.
    /// </summary>
    public class SockperfThrougputResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SockperfThrougputResult"/> class.
        /// </summary>
        public SockperfThrougputResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SockperfThrougputResult"/> class.
        /// </summary>
        /// <param name="messageRate">Message rate measured by test run.</param>
        /// <param name="bandWidth">Bandwidth measured by test run.</param>
        public SockperfThrougputResult(int messageRate, float bandWidth)
        {
            this.MessageRatePerSecond = messageRate;
            this.BandWidthInMbps = bandWidth;
        }

        /// <summary>Gets or sets used test parameters for command call.</summary>
        public TestParameters TestParameters { get; set; }

        /// <summary>Gets or sets the measured message rate per second.</summary>
        public int MessageRatePerSecond { get; set; }

        /// <summary>Gets or sets the measured bandwidth in Mbps.</summary>
        public float BandWidthInMbps { get; set; }

        /// <summary>
        /// Create <see cref="SockperfThrougputResult"/> from Sockperf output.
        /// </summary>
        /// <param name="testParameters">Parameters used for testing.</param>
        /// <param name="consoleOutput">Output of Sockperf.</param>
        /// <returns><see cref="SockperfThrougputResult"/> created from Sockperf output.</returns>
        public static SockperfThrougputResult FromConsoleOutput(TestParameters testParameters, string consoleOutput)
        {
            var result = new SockperfThrougputResult();
            result.TestParameters = testParameters;

            // Get message rate
            var messageRateRegExPattern = "Message Rate is ([0-9]+)";
            MatchCollection matchesMessageRate = Regex.Matches(consoleOutput, messageRateRegExPattern);
            if (matchesMessageRate.Count == 1)
            {
                if (matchesMessageRate[0].Groups.Count == 2)
                {
                    var messageRateString = matchesMessageRate[0].Groups[1].Value;
                    result.MessageRatePerSecond = int.Parse(messageRateString);
                }
                else
                {
                    Log.Error($"Regcular Expression Group Count of Match should be 2 and not {matchesMessageRate[0].Groups.Count}");
                }
            }
            else
            {
                Log.Error($"Regcular Expression Match Count should be 1 and not {matchesMessageRate.Count}");
            }

            // Get bandwidth
            var bandWithRegExPattern = "BandWidth is [0-9]+.[0-9]+ MBps \\(([0-9]+.[0-9]+) Mbps\\)";
            MatchCollection matchesBandWidth = Regex.Matches(consoleOutput, bandWithRegExPattern);
            if (matchesBandWidth.Count == 1)
            {
                if (matchesBandWidth[0].Groups.Count == 2)
                {
                    var bandWidthString = matchesBandWidth[0].Groups[1].Value;
                    result.BandWidthInMbps = float.Parse(bandWidthString, CultureInfo.InvariantCulture);
                }
                else
                {
                    Log.Error($"Regcular Expression Group Count of Match should be 2 and not {matchesMessageRate[0].Groups.Count}");
                }
            }
            else
            {
                Log.Error($"Regcular Expression Match Count should be 1 and not {matchesMessageRate.Count}");
            }

            return result;
        }

        /// <summary>
        /// Convert to <see cref="ThroughputTestResult"/> suitable for storing in database.
        /// </summary>
        /// <param name="label">Label of the test.</param>
        /// <param name="transmissionTechnology">Transmission technology used for test.</param>
        /// <param name="comment">Comment describing the test.</param>
        /// <returns><see cref="SockperfThrougputResult"/> converted to <see cref="ThroughputTestResult"/>.</returns>
        public ThroughputTestResult ToThroughputTestResult(string label, string transmissionTechnology, string comment)
        {
            // Test Parameters
            var throughputTestParameters = new ThroughputTestParameters()
            {
                StartTimestamp = this.TestParameters.StartTimestamp,
                MessageSize = this.TestParameters.MessageSize,
                MessagesPerSecond = this.TestParameters.MessagesPerSecond,
                TestDuration = this.TestParameters.TestDuration,
                TestServer = $"{this.TestParameters.ServerIp}:{this.TestParameters.ServerPort}",
                TestTool = this.TestParameters.TestTool,
                TestMode = this.TestParameters.TestMode,
                Protocol = this.TestParameters.Protocol,
                Comment = comment,
                TransmissionTechnology = transmissionTechnology,
            };

            // Test Result
            var throughputTestResult = new ThroughputTestResult()
            {
                Label = label,
                TestParameters = throughputTestParameters,
                Bandwidth = this.BandWidthInMbps,
                MessageRate = this.MessageRatePerSecond,
            };

            return throughputTestResult;
        }
    }
}
