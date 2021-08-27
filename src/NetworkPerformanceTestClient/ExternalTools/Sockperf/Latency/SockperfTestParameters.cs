// <copyright file="SockperfTestParameters.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using NetworkPerformanceTestClient.ExternalTools.Common;

    /// <summary>
    /// Test parameters for Sockperf.
    /// </summary>
    public class SockperfTestParameters : TestParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SockperfTestParameters"/> class.
        /// </summary>
        /// <param name="serverIp">IP address of Sockperf server.</param>
        /// <param name="serverPort">Port of Sockperf server.</param>
        /// <param name="time">Test durtaion.</param>
        /// <param name="protocol">Protocol (UDP or TCP).</param>
        /// <param name="messageSize">Messages size.</param>
        /// <param name="messagesPerSecond">Messages send per second.</param>
        /// <param name="testMode">Test mode (e.g. ping-pong).</param>
        public SockperfTestParameters(string serverIp, int serverPort, int time, string protocol, int messageSize, int messagesPerSecond, string testMode)
            : base(serverIp, serverPort, time, messageSize, messagesPerSecond, "Sockperf", testMode)
        {
            this.Protocol = protocol;
        }
    }
}
