// <copyright file="OWPingResult.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NetworkPacketCapturer;
    using NetworkPerformanceTestClient.ExternalTools.Common;

    /// <summary>
    /// Result of owping test run.
    /// </summary>
    public class OWPingResult : PingResult
    {
        private int sentMessages;

        private int lostMessages;

        /// <inheritdoc/>
        public override int SentMessages
        {
            get { return this.sentMessages; } set { this.sentMessages = value; }
        }

        /// <inheritdoc/>
        public override int ReceivedMessages
        {
            get { return this.SentMessages - this.LostMessages; } set { }
        }

        /// <inheritdoc/>
        public override int LostMessages
        {
            get { return this.lostMessages; } set { this.lostMessages = value; }
        }

        /// <summary>
        /// Create <see cref="OWPingResult"/> from owping outpu.
        /// </summary>
        /// <param name="testParameters">Parameters used for testing.</param>
        /// <param name="output">Output of owping.</param>
        /// <returns><see cref="OWPingResult"/> of the testrun.</returns>
        public static OWPingResult FromOutput(OWPingTestParameters testParameters, string output)
        {
            var owpingResult = new OWPingResult();
            owpingResult.RawStandardOutput = output;
            owpingResult.TestParameters = testParameters;

            var outputLines = Regex.Split(output, "\r\n|\r|\n");

            // Get sending and receiving host (owping performs
            MatchCollection matches = Regex.Matches(outputLines[1], @"--- owping statistics from \[(.+)\]:([0-9]+) to \[(.+)\]:([0-9]+) ---");
            if (matches.Count == 1)
            {
                if (matches[0].Groups.Count == 5)
                {
                    var senderHost = matches[0].Groups[1].Value;
                    var senderPort = matches[0].Groups[2].Value;
                    var receiverHost = matches[0].Groups[3].Value;
                    var receiverPort = matches[0].Groups[4].Value;
                }
            }

            // Get lines with individual packet results
            int i = 3;
            while (outputLines[i].Length != 0 && i < outputLines.Length)
            {
                matches = Regex.Matches(outputLines[i], @"seq_no=([0-9]+).*delay=(-*[0-9]+\.[0-9]+e-[0-9]+) ms.*sent=([0-9]+\.[0-9]+).*recv=([0-9]+\.[0-9]+)");
                if (matches.Count == 1)
                {
                    if (matches[0].Groups.Count == 5)
                    {
                        var index = int.Parse(matches[0].Groups[1].Value);
                        var latency = float.Parse(matches[0].Groups[2].Value, CultureInfo.InvariantCulture);
                        var sentTime = double.Parse(matches[0].Groups[3].Value, CultureInfo.InvariantCulture);
                        var recvTime = double.Parse(matches[0].Groups[4].Value, CultureInfo.InvariantCulture);
                        var packetMeasurement = new PingMeasurment(index, sentTime, recvTime, latency, -1, -1);
                        owpingResult.Measurments.Add(packetMeasurement);
                    }
                }

                i++;
            }

            var startTimestamp = owpingResult.Measurments[0].SendTime;
            foreach (var measurment in owpingResult.Measurments)
            {
                measurment.SendTime = measurment.SendTime - startTimestamp;
                measurment.ReceiveTime = measurment.ReceiveTime - startTimestamp;
            }

            // Get test summary
            var resultLines = outputLines.ToList().GetRange(i + 1, 9).ToArray();
            matches = Regex.Matches(resultLines[4], @"([0-9]+) sent, ([0-9]+) lost \(0.000%\), 0 duplicates");
            if (matches.Count == 1)
            {
                if (matches[0].Groups.Count == 3)
                {
                    owpingResult.sentMessages = int.Parse(matches[0].Groups[1].Value);
                    owpingResult.lostMessages = int.Parse(matches[0].Groups[2].Value);
                }
            }

            matches = Regex.Matches(resultLines[5], @"one-way delay min/median/max = (-*[0-9]+\.[0-9]+)/(-*[0-9]+\.[0-9]+)/(-*[0-9]+\.[0-9]+) ms");
            if (matches.Count == 1)
            {
                if (matches[0].Groups.Count == 4)
                {
                    owpingResult.MinimumLatency = float.Parse(matches[0].Groups[1].Value, CultureInfo.InvariantCulture);
                    owpingResult.AverageLatency = float.Parse(matches[0].Groups[2].Value, CultureInfo.InvariantCulture);
                    owpingResult.MaximumLatency = float.Parse(matches[0].Groups[3].Value, CultureInfo.InvariantCulture);
                }
            }

            return owpingResult;
        }
    }
}