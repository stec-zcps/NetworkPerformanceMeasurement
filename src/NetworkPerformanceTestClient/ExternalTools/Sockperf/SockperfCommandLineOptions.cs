// <copyright file="SockperfCommandLineOptions.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using CommandLine;

    /// <summary>
    /// Command line options specific to external tool Sockperf.
    /// </summary>
    public class SockperfCommandLineOptions : CommandLineOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SockperfCommandLineOptions"/> class.
        /// </summary>
        public SockperfCommandLineOptions()
        {
        }

        /// <summary>Gets or sets IP address of Sockperf server.</summary>
        [Option('s', "serverIp", Required = false, HelpText = "Send to ip <ip> (default = 127.0.0.1)")]
        public string ServerIp { get; set; } = "127.0.0.1";

        /// <summary>Gets or sets port of Sockperf server.</summary>
        [Option('p', "port", Required = false, HelpText = "Connecto to <port> (default = 11111)")]
        public int Port { get; set; } = 11111;

        /// <summary>Gets or sets test duration.</summary>
        [Option('t', "time", Required = false, HelpText = "Run for <sec> seconds (default = 1, max = 36000000)")]
        public int Time { get; set; } = 1;

        /// <summary>Gets or sets a value indicating whether TCP should be used as protocol.</summary>
        [Option("tcp", Required = false, HelpText = "Use TCP protocol (default = UDP).")]
        public bool Tcp { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether UDP should be used as protocol.</summary>
        [Option("udp", Required = false, HelpText = "Use UDP protocol")]
        public bool Udp { get; set; } = false;

        /// <summary>Gets the protocol.</summary>
        public string Protocol
        {
            get { return this.Tcp ? "tcp" : "udp"; }
        }
    }
}
