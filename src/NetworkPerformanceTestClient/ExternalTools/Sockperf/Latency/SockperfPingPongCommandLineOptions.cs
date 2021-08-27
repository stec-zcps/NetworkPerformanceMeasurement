// <copyright file="SockperfPingPongCommandLineOptions.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Collections.Generic;
    using CommandLine;

    /// <summary>
    /// Command line options specific to external tool Sockperf (ping-pong mode).
    /// </summary>
    [Verb("ping-pong", HelpText = "Run Sockperf in ping-pong mode")]
    public class SockperfPingPongCommandLineOptions : SockperfCommandLineOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SockperfPingPongCommandLineOptions"/> class.
        /// </summary>
        public SockperfPingPongCommandLineOptions()
        {
        }

        /// <summary>Gets or sets the messages size.</summary>
        [Option('m', "msg-size", Required = false, Separator = ',', HelpText = "Use messages of size <size> bytes (minimum default 64).")]
        public IEnumerable<int> MessageSize { get; set; } = new int[] { 14 };

        /// <summary>Gets or sets messages per second.</summary>
        [Option('s', "msgs-per-sec", Required = false, Separator = ',', HelpText = "Set number of messages-per-second (default = 100)")]
        public IEnumerable<int> MessagesPerSecond { get; set; } = new int[] { 100 };

        /// <summary>Gets or sets a value indicating whether Sockperf should wait for pong before sending next ping packet.</summary>
        [Option("wait-for-pong", Required = false, HelpText = "Wait for pong before sending next packet.")]
        public bool WaitForPong { get; set; } = false;
    }
}
