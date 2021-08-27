// <copyright file="PingMeasurment.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    /// <summary>
    /// Measurment during a ping latency test run.
    /// </summary>
    public class PingMeasurment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PingMeasurment"/> class.
        /// </summary>
        public PingMeasurment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PingMeasurment"/> class.
        /// </summary>
        /// <param name="index">Index of the measurment packet.</param>
        /// <param name="sendTime">Send time of the measurment packet.</param>
        /// <param name="receiveTime">Receive time of the measurment packet.</param>
        /// <param name="latencyOverall">Overall latency of the measurment packet.</param>
        /// <param name="latencyClientToServer">One way latency from client to server.</param>
        /// <param name="latencyServeToClient">One way latency from server to client.</param>
        public PingMeasurment(long index, double sendTime, double receiveTime, float latencyOverall, float latencyClientToServer, float latencyServeToClient)
        {
            this.Index = index;
            this.SendTime = sendTime;
            this.ReceiveTime = receiveTime;
            this.LatencyOverall = latencyOverall;
            this.LatencyClientToServer = latencyClientToServer;
            this.LatencyServeToClient = latencyServeToClient;
        }

        /// <summary>Gets or sets the index of the measurment.</summary>
        public long Index { get; set; }

        /// <summary>Gets or sets overall latency of the measurment.</summary>
        public float LatencyOverall { get; set; }

        /// <summary>Gets or sets the one way latency from client to server.</summary>
        public float LatencyClientToServer { get; set; }

        /// <summary>Gets or sets from server to client.</summary>
        public float LatencyServeToClient { get; set; }

        /// <summary>Gets or sets send time.</summary>
        public double SendTime { get; set; }

        /// <summary>Gets or sets the receive time.</summary>
        public double ReceiveTime { get; set; }
    }
}
