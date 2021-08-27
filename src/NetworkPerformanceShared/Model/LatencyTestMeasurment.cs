// <copyright file="LatencyTestMeasurment.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPerformanceShared.Model
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents a measurment within a latency test.
    /// </summary>
    public class LatencyTestMeasurment
    {
        /// <summary>Gets or sets id of the the <see cref="LatencyTestMeasurment"/>.</summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>Gets or sets the related <see cref="LatencyTestResult"/> of this measurment.</summary>
        [ForeignKey("LatencyTestResultId")]
        public LatencyTestResult RelatedTestResult { get; set; }

        /// <summary>Gets or sets index / position of this measurment in the related latency test.</summary>
        public long Index { get; set; }

        /// <summary>Gets or sets time when message was sent relativ to test start (in nanoseconds).</summary>
        public double SendTime { get; set; }

        /// <summary>Gets or sets time when message was received relativ to test start (in nanoseconds).</summary>
        public double ReceiveTime { get; set; }

        /// <summary>Gets or sets measured latency (via tool).</summary>
        public float Latency { get; set; }

        /// <summary>Gets or sets measured one way latency from client to server (via tool).</summary>
        public float LatencyClientToServer { get; set; } = -1F;

        /// <summary>Gets or sets measured one way latency from server to client (via tool).</summary>
        public float LatencyServerToClient { get; set; } = -1F;

        /// <summary>Gets or sets captured latency (via network paket capturing / TAP).</summary>
        public float CapturedLatency { get; set; } = -1F;

        /// <summary>Gets or sets measured jitter to previous packet.</summary>
        public float Jitter { get; set; }

        /// <summary>Gets or sets captured jitter (via network paket capturing / TAP).</summary>
        public float CapturedJitter { get; set; } = -1F;

        /// <summary>Gets or sets captured (via TAP) one way latency from client to server.</summary>
        public float CapturedOneWayLatencyClientToServer { get; set; } = -1F;

        /// <summary>Gets or sets captured (via TAP) one way latency from server to client.</summary>
        public float CapturedOneWayLatencyServerToClient { get; set; } = -1F;

        /// <summary>Gets or sets captured (via TAP) processing time in server to reply to a ping with a pong.</summary>
        public float CapturedProcessingTimeServer { get; set; } = -1F;

        /// <summary>Gets or sets a value indicating whether a client-side ping packet was captured by KUNBUS TAP CURIOUS.</summary>
        public bool CapturedTapPingClientSide { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether a server-side ping packet was captured by KUNBUS TAP CURIOUS.</summary>
        public bool CapturedTapPingServerSide { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether a client-side pong packet was captured by KUNBUS TAP CURIOUS.</summary>
        public bool CapturedTapPongClientSide { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether a server-side pong packet was captured by KUNBUS TAP CURIOUS.</summary>
        public bool CapturedTapPongServerSide { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether the measurment is an outlier.</summary>
        public bool IsOutlier { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether the measurment is an outlier caused on trasmission from client to server.</summary>
        public bool IsPingOutlier { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether the measurment is an outlier caused on trasmission from server to client.</summary>
        public bool IsPongOutlier { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether the measurment is an outlier caused by processing in the server.</summary>
        public bool IsProcessingServerOutlier { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether the measurment is an outlier caused by processing in the client.</summary>
        public bool IsProcessingClientOutlier { get; set; } = false;
    }
}
