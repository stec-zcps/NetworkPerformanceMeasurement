// <copyright file="TestParameters.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System;

    /// <summary>
    /// Parameters used for a test run.
    /// </summary>
    public abstract class TestParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestParameters"/> class.
        /// </summary>
        /// <param name="serverIp">IP address of test server.</param>
        /// <param name="serverPort">Port of test server.</param>
        /// <param name="testDuration">Test duration.</param>
        /// <param name="messageSize">Messages size.</param>
        /// <param name="messagesPerSecond">Messages send per second.</param>
        /// <param name="testTool">Tool used for testing.</param>
        /// <param name="testMode">Mode of tool used for testing.</param>
        public TestParameters(string serverIp, int serverPort, int testDuration, int messageSize, int messagesPerSecond, string testTool, string testMode)
        {
            this.ServerIp = serverIp;
            this.ServerPort = serverPort;
            this.TestDuration = testDuration;
            this.MessageSize = messageSize;
            this.MessagesPerSecond = messagesPerSecond;
            this.TestTool = testTool;
            this.TestMode = testMode;
        }

        /// <summary>Gets or sets the timestamp when test test was strated.</summary>
        public DateTime StartTimestamp { get; set; } = DateTime.Now;

        /// <summary>Gets or sets the IP address of the test server.</summary>
        public string ServerIp { get; set; }

        /// <summary>Gets or sets the port of the test server.</summary>
        public int ServerPort { get; set; }

        /// <summary>Gets or sets the test duration.</summary>
        public int TestDuration { get; set; } = 1;

        /// <summary>Gets or sets the protocol used during test.</summary>
        public string Protocol { get; protected set; }

        /// <summary>Gets or sets the messages size.</summary>
        public int MessageSize { get; set; }

        /// <summary>Gets or sets messages send per second.</summary>
        public int MessagesPerSecond { get; set; }

        /// <summary>Gets or sets the test tool.</summary>
        public string TestTool { get; set; }

        /// <summary>Gets or sets mode of the test tool.</summary>
        public string TestMode { get; set; }
    }
}
