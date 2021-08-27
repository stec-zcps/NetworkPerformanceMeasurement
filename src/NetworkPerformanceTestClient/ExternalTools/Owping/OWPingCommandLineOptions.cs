// <copyright file="OWPingCommandLineOptions.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using CommandLine;

    /// <summary>
    /// Command line options specific to external tool owping.
    /// </summary>
    public class OWPingCommandLineOptions : CommandLineOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OWPingCommandLineOptions"/> class.
        /// </summary>
        public OWPingCommandLineOptions()
        {
        }

        /// <summary>Gets or sets the IP of the owping server.</summary>
        [Option('s', "serverIp", Required = false, HelpText = "Send to ip <ip> (default = 127.0.0.1)")]
        public string ServerIp { get; set; } = "127.0.0.1";

        /// <summary>Gets or sets the port of the owping server.</summary>
        [Option('p', "port", Required = false, HelpText = "Connecto to <port> (default = 11111)")]
        public int Port { get; set; } = 11111;

        /// <summary>Gets or sets the test duration.</summary>
        [Option('t', "time", Required = false, HelpText = "Run for <sec> seconds (default = 1, max = 36000000)")]
        public int Time { get; set; } = 1;

        /// <summary>Gets or sets the messages size.</summary>
        [Option('m', "msg-size", Required = false, Separator = ',', HelpText = "Use messages of size <size> bytes (minimum default 14).")]
        public IEnumerable<int> MessageSize { get; set; } = new int[] { 14 };

        /// <summary>Gets or sets messages send per second.</summary>
        [Option('s', "msgs-per-sec", Required = false, Separator = ',', HelpText = "Set number of messages-per-second (default = 100)")]
        public IEnumerable<int> MessagesPerSecond { get; set; } = new int[] { 100 };
    }
}
