// <copyright file="OWPingTestParameters.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    /// Parameters for test with owping.
    /// </summary>
    public class OWPingTestParameters : TestParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OWPingTestParameters"/> class.
        /// </summary>
        /// <param name="serverIp">IP address of owping server.</param>
        /// <param name="serverPort">Port of owping server.</param>
        /// <param name="testDuration">Test duration.</param>
        /// <param name="messageSize">Messages size.</param>
        /// <param name="messagesPerSecond">Messages send per second.</param>
        public OWPingTestParameters(string serverIp, int serverPort, int testDuration, int messageSize, int messagesPerSecond)
            : base(serverIp, serverPort, testDuration, messageSize, messagesPerSecond, "owping", "cliet-to-server")
        {
            this.Protocol = "UDP";
            this.MessageSize = messageSize;
            this.MessagesPerSecond = messagesPerSecond;
        }
    }
}