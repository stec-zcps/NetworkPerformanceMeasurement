// <copyright file="PingResult.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPerformanceTestClient.ExternalTools.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using NetworkPacketCapturer;
    using NetworkPerformanceShared;
    using NetworkPerformanceShared.Model;
    using NetworkPerformanceTestClient.ExternalTools.Owping;
    using Serilog;

    /// <summary>
    /// Result of a ping based latency measurment.
    /// </summary>
    public abstract class PingResult
    {
        /// <summary>Gets or sets the packets caputred during test.</summary>
        public Dictionary<long, List<CapturedPacket>> CapturedPackets { get; set; } = new Dictionary<long, List<CapturedPacket>>();

        /// <summary>Gets or sets the used test parameters for command call.</summary>
        public TestParameters TestParameters { get; set; }

        /// <summary>Gets or sets raw standard output from test command call.</summary>
        public string RawStandardOutput { get; set; }

        /// <summary>Gets or sets list of all measured packets.</summary>
        public List<PingMeasurment> Measurments { get; set; } = new List<PingMeasurment>();

        /// <summary>Gets or sets sent message count. </summary>
        public abstract int SentMessages { get; set; }

        /// <summary>Gets or sets received message count. </summary>
        public abstract int ReceivedMessages { get; set; }

        /// <summary>Gets or sets lost message count. </summary>
        public abstract int LostMessages { get; set; }

        /// <summary>Gets or sets minimum packet latency in milliseconds (ms). </summary>
        public float MinimumLatency { get; set; }

        /// <summary>Gets or sets maximum packet latency in milliseconds (ms). </summary>
        public float MaximumLatency { get; set; }

        /// <summary>Gets or sets average packet latency in milliseconds (ms). </summary>
        public float AverageLatency { get; set; }

        /// <summary>Gets or sets standard deviation of packet latency in milliseconds (ms).</summary>
        public float StandardDeviationLatency { get; set; }

        /// <summary>
        /// Convert to <see cref="LatencyTestResult"/> suitable for storing in database.
        /// </summary>
        /// <param name="label">Label of the test.</param>
        /// <param name="transmissionTechnology">Transmission technology used for test.</param>
        /// <param name="comment">Comment describing the test.</param>
        /// <param name="tapEnabled">Set if Kunbus TAP CURIOUS was used during test.</param>
        /// <returns><see cref="PingResult"/> converted to <see cref="LatencyTestResult"/>.</returns>
        public LatencyTestResult ToLatencyTestResult(string label, string transmissionTechnology, string comment, bool tapEnabled = false)
        {
            // Test Parameters
            var latencyTestParameters = new LatencyTestParameters()
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
            var latencyTestResult = new LatencyTestResult()
            {
                Label = label,
                TestParameters = latencyTestParameters,
                MinimumLatency = this.MinimumLatency,
                MaximumLatency = this.MaximumLatency,
                AverageLatency = this.AverageLatency,
                StandardDeviationLatency = CalculationUtils.GetStandardDeviation(this.Measurments.Select(item => item.LatencyOverall).Where(val => val > 0)),
                SentMessages = this.SentMessages,
                ReceivedMessages = this.SentMessages - this.LostMessages,
                LostMessages = this.LostMessages,
            };

            var pingPacketCounter = 0;
            var pongPacketCounter = 0;
            var fullPingPongCounter = 0;
            var tapPortACounter = 0;
            var tapPortBCounter = 0;
            var tapPortCCounter = 0;
            var tapPortDCounter = 0;
            foreach (var entry in this.CapturedPackets.Values.ToList())
            {
                var pingPacketsForEntry = entry.Where(packet => packet.Direction == CapturedPacketDirection.PING).ToList().Count;
                var pongPacketsForEntry = entry.Where(packet => packet.Direction == CapturedPacketDirection.PONG).ToList().Count;

                pingPacketCounter += pingPacketsForEntry;
                pongPacketCounter += pongPacketsForEntry;

                if (pingPacketsForEntry > 0 && pongPacketsForEntry > 0)
                {
                    fullPingPongCounter++;
                }

                if (tapEnabled)
                {
                    tapPortACounter += entry.Where(packet => packet.KunbusTapData.PortName == "A").ToList().Count;
                    tapPortBCounter += entry.Where(packet => packet.KunbusTapData.PortName == "B").ToList().Count;
                    tapPortCCounter += entry.Where(packet => packet.KunbusTapData.PortName == "C").ToList().Count;
                    tapPortDCounter += entry.Where(packet => packet.KunbusTapData.PortName == "D").ToList().Count;
                }
            }

            Log.Information($"Captured Ping Packets: {pingPacketCounter}");
            Log.Information($"Captured Pong Packets: {pongPacketCounter}");
            Log.Information($"Captured Full Ping Pongs: {fullPingPongCounter}");
            if (tapEnabled)
            {
                Log.Information($"Captured Packets - TAP Port A: {tapPortACounter}");
                Log.Information($"Captured Packets - TAP Port B: {tapPortBCounter}");
                Log.Information($"Captured Packets - TAP Port C: {tapPortCCounter}");
                Log.Information($"Captured Packets - TAP Port D: {tapPortDCounter}");
            }

            // Test Measurments
            foreach (var measurment in this.Measurments)
            {
                var latencyTestMeasurment = new LatencyTestMeasurment()
                {
                    Index = measurment.Index,
                    SendTime = measurment.SendTime,
                    ReceiveTime = measurment.ReceiveTime * 1000,
                    Latency = measurment.LatencyOverall,
                    LatencyClientToServer = measurment.LatencyClientToServer,
                    LatencyServerToClient = measurment.LatencyServeToClient,
                    Jitter = measurment.LatencyOverall - this.AverageLatency,
                    RelatedTestResult = latencyTestResult,
                };

                if (this.CapturedPackets.Count > 0 && tapEnabled)
                {
                    if (this.CapturedPackets.ContainsKey(measurment.Index))
                    {
                        var capturedPackets = this.CapturedPackets[measurment.Index];

                        if (capturedPackets.Count > 1)
                        {
                            var pingPackets = capturedPackets.FindAll(p => p.Direction.Equals(CapturedPacketDirection.PING));
                            var pongPackets = capturedPackets.FindAll(p => p.Direction.Equals(CapturedPacketDirection.PONG));

                            if (tapEnabled)
                            {
                                var tapPingClientSide = pingPackets.Find(p => p.KunbusTapData.PortName.Equals("A") || p.KunbusTapData.PortName.Equals("B"));
                                latencyTestMeasurment.CapturedTapPingClientSide = tapPingClientSide != null ? true : false;
                                var tapPingServerSide = pingPackets.Find(p => p.KunbusTapData.PortName.Equals("C") || p.KunbusTapData.PortName.Equals("D"));
                                latencyTestMeasurment.CapturedTapPingServerSide = tapPingServerSide != null ? true : false;

                                var tapPongClientSide = pongPackets.Find(p => p.KunbusTapData.PortName.Equals("A") || p.KunbusTapData.PortName.Equals("B"));
                                latencyTestMeasurment.CapturedTapPongClientSide = tapPongClientSide != null ? true : false;
                                var tapPongServerSide = pongPackets.Find(p => p.KunbusTapData.PortName.Equals("C") || p.KunbusTapData.PortName.Equals("D"));
                                latencyTestMeasurment.CapturedTapPongServerSide = tapPongServerSide != null ? true : false;

                                latencyTestMeasurment.CapturedOneWayLatencyClientToServer =
                                    latencyTestMeasurment.CapturedTapPingServerSide && latencyTestMeasurment.CapturedTapPingClientSide ?
                                        (tapPingServerSide.KunbusTapData.TimestampInNs - tapPingClientSide.KunbusTapData.TimestampInNs) / 1000000F
                                        : -1F;

                                latencyTestMeasurment.CapturedOneWayLatencyServerToClient =
                                    latencyTestMeasurment.CapturedTapPongClientSide && latencyTestMeasurment.CapturedTapPongServerSide ?
                                        (tapPongClientSide.KunbusTapData.TimestampInNs - tapPongServerSide.KunbusTapData.TimestampInNs) / 1000000F
                                        : -1F;

                                latencyTestMeasurment.CapturedProcessingTimeServer =
                                    latencyTestMeasurment.CapturedTapPongServerSide && latencyTestMeasurment.CapturedTapPingServerSide ?
                                        (tapPongServerSide.KunbusTapData.TimestampInNs - tapPingServerSide.KunbusTapData.TimestampInNs) / 1000000F
                                        : -1F;

                                var capturedLatency =
                                    latencyTestMeasurment.CapturedTapPongClientSide && latencyTestMeasurment.CapturedTapPingClientSide ?
                                        ((tapPongClientSide.KunbusTapData.TimestampInNs - tapPingClientSide.KunbusTapData.TimestampInNs) / 1000000F)
                                        : -1F;
                                if (capturedLatency < -1)
                                {
                                    Log.Information(string.Empty);
                                }

                                latencyTestMeasurment.CapturedLatency = capturedLatency;
                            }
                            else
                            {
                                if (pingPackets.Count == 1 && pongPackets.Count == 1)
                                {
                                    var roundTripTimeSoftCapturing = (pongPackets[0].Timestamp.Date - pingPackets[0].Timestamp.Date).TotalMilliseconds
                                                                        + ((pongPackets[0].Timestamp.MicroSeconds - pingPackets[0].Timestamp.MicroSeconds) / 1000F);

                                    latencyTestMeasurment.CapturedLatency = (float)roundTripTimeSoftCapturing / 2F;
                                }
                            }
                        }
                    }
                    else
                    {
                        Log.Verbose($"No packets captured for '{measurment.Index}'");
                    }
                }
                else
                {
                    Log.Information("No packets captured");
                }

                latencyTestResult.TestMeasurments.Add(latencyTestMeasurment);
            }

            var avgLatency = latencyTestResult.AverageLatency;

            if (this.CapturedPackets.Count > 0)
            {
                // Calculate captured jitter
                var validMeasurments = latencyTestResult.TestMeasurments.Where(measurment => measurment.CapturedLatency > 0).ToList();
                if (validMeasurments.Count > 0)
                {
                    var averageCapturedLatency = validMeasurments.Select(item => item.CapturedLatency).Average();
                    foreach (var measurment in validMeasurments)
                    {
                        measurment.CapturedJitter = measurment.CapturedLatency - averageCapturedLatency;
                    }
                }
            }

            latencyTestResult.MinimumJitter = latencyTestResult.TestMeasurments.Select(item => item.Jitter).Where(val => val > 0).Min();
            latencyTestResult.MaximumJitter = latencyTestResult.TestMeasurments.Select(item => item.Jitter).Where(val => val > 0).Max();
            latencyTestResult.AverageJitter = latencyTestResult.TestMeasurments.Select(item => item.Jitter).Where(val => val > 0).Average();
            latencyTestResult.StandardDeviationJitter = CalculationUtils.GetStandardDeviation(latencyTestResult.TestMeasurments.Select(item => item.Jitter).Where(val => val > 0));

            // Analyse outliers
            var sigmaOutlierCount = 3;
            var sigmaOutlierCategorisationCount = 2;
            var uncategorizedOutlierCounter = 0;
            var pingOutlierCounter = 0;
            var pongOutlierCounter = 0;
            var processingClientOutlierCounter = 0;
            var processingServerOutlierCounter = 0;

            var outliers = latencyTestResult.TestMeasurments.Where(item => item.Latency > this.AverageLatency + (sigmaOutlierCount * this.StandardDeviationLatency)).ToList();
            var averageLatencyClientToServer = latencyTestResult.TestMeasurments.Select(item => item.CapturedOneWayLatencyClientToServer).Where(val => val > 0).Average();
            var standardDeviationLatencyClientToServer = CalculationUtils.GetStandardDeviation(latencyTestResult.TestMeasurments.Select(item => item.CapturedOneWayLatencyClientToServer).Where(val => val > 0));
            var averageLatencyServerToClient = latencyTestResult.TestMeasurments.Select(item => item.CapturedOneWayLatencyServerToClient).Where(val => val > 0).Average();
            var standardDeviationLatencyServerToClient = CalculationUtils.GetStandardDeviation(latencyTestResult.TestMeasurments.Select(item => item.CapturedOneWayLatencyServerToClient).Where(val => val > 0));
            var averageProcessingTimeServer = latencyTestResult.TestMeasurments.Select(item => item.CapturedProcessingTimeServer).Where(val => val > 0).Average();
            var standardDeviationProcessingTimeServer = CalculationUtils.GetStandardDeviation(latencyTestResult.TestMeasurments.Select(item => item.CapturedProcessingTimeServer).Where(val => val > 0));
            var averageProcessingTimeClient = latencyTestResult.TestMeasurments.Select(item => item.Latency - item.CapturedLatency).Where(val => val > 0).Average();
            var standardDeviationProcessingTimeClient = CalculationUtils.GetStandardDeviation(latencyTestResult.TestMeasurments.Select(item => item.Latency - item.CapturedLatency).Where(val => val > 0));

            // Log.Information($"Outlier Threadshold - Ping: {standardDeviationLatencyClientToServer}");
            // Log.Information($"Outlier Threadshold - Pong: {standardDeviationLatencyServerToClient}");
            // Log.Information($"Outlier Threadshold - Processing Server: {standardDeviationProcessingTimeServer}");
            // Log.Information($"Outlier Threadshold - Processing Client: {standardDeviationProcessingTimeClient}");
            foreach (var measurment in latencyTestResult.TestMeasurments)
            {
                if (measurment.Latency > this.AverageLatency + (sigmaOutlierCount * this.StandardDeviationLatency))
                {
                    measurment.IsOutlier = true;
                    if (measurment.CapturedOneWayLatencyClientToServer > (averageLatencyClientToServer + (sigmaOutlierCategorisationCount * standardDeviationLatencyClientToServer)))
                    {
                        measurment.IsPingOutlier = true;
                        pingOutlierCounter++;
                    }

                    if (measurment.CapturedOneWayLatencyServerToClient > (averageLatencyServerToClient + (sigmaOutlierCategorisationCount * standardDeviationLatencyServerToClient)))
                    {
                        measurment.IsPongOutlier = true;
                        pongOutlierCounter++;
                    }

                    if (measurment.CapturedProcessingTimeServer > (averageProcessingTimeServer + (sigmaOutlierCategorisationCount * standardDeviationProcessingTimeServer)))
                    {
                        measurment.IsProcessingServerOutlier = true;
                        processingServerOutlierCounter++;
                    }

                    if ((measurment.Latency - measurment.CapturedLatency) > (averageProcessingTimeClient + (sigmaOutlierCategorisationCount * standardDeviationProcessingTimeClient)))
                    {
                        measurment.IsProcessingClientOutlier = true;
                        processingClientOutlierCounter++;
                    }

                    if (!measurment.IsPingOutlier && !measurment.IsPongOutlier && !measurment.IsProcessingClientOutlier && !measurment.IsProcessingServerOutlier)
                    {
                        uncategorizedOutlierCounter++;
                    }
                }
            }

            Log.Information($" Outliers: {outliers.Count}\n* Uncategorized Outliers: {uncategorizedOutlierCounter}\n* Ping: {pingOutlierCounter}\n* Pong: {pongOutlierCounter}\n* Processing Client: {processingClientOutlierCounter}\n* Processing Server: {processingServerOutlierCounter}");

            return latencyTestResult;
        }
    }
}
