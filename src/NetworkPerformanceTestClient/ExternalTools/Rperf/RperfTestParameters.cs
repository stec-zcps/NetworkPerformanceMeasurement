// <copyright file="RperfTestParameters.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using NetworkPerformanceTestClient.ExternalTools.Common;

    /// <summary>
    /// Test parameters for <see cref="Rperf"/>.
    /// </summary>
    public class RperfTestParameters : TestParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RperfTestParameters"/> class.
        /// </summary>
        /// <param name="serverIp">IP address for Rperf server.</param>
        /// <param name="serverPort">Port of Rperf server.</param>
        /// <param name="testDuration">Test duration.</param>
        /// <param name="messageSize">Messages size.</param>
        /// <param name="messagesPerSecond">Messages per second.</param>
        /// <param name="protocol">Protocol.</param>
        public RperfTestParameters(string serverIp, int serverPort, int testDuration, int messageSize, int messagesPerSecond, string protocol)
            : base(serverIp, serverPort, testDuration, messageSize, messagesPerSecond, "rperf", "ping-pong")
        {
            this.Protocol = protocol;
            this.MessageSize = messageSize;
            this.MessagesPerSecond = messagesPerSecond;
        }
    }
}