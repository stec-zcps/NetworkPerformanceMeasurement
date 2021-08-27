// <copyright file="RperfCommandLineOptions.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    /// Command line options specific to external tool Rperf.
    /// </summary>
    public class RperfCommandLineOptions : CommandLineOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RperfCommandLineOptions"/> class.
        /// </summary>
        public RperfCommandLineOptions()
        {
        }

        /// <summary>Gets or sets IP address of Rperf server..</summary>
        [Option('i', "serverIp", Required = false, HelpText = "Send to ip <ip> (default = 127.0.0.1)")]
        public string ServerIp { get; set; } = "127.0.0.1";

        /// <summary>Gets or sets port of Rperf server..</summary>
        [Option('p', "port", Required = false, HelpText = "Connecto to <port> (default = 11111)")]
        public int Port { get; set; } = 11111;

        /// <summary>Gets or sets protocol.</summary>
        [Option("protocol", Required = true, HelpText = "Protocol used for packet transmission")]
        public string Protocol { get; set; }

        /// <summary>Gets or sets test duration.</summary>
        [Option("time", Required = false, HelpText = "Run for <sec> seconds (default = 1, max = 36000000)")]
        public int Time { get; set; } = 1;

        /// <summary>Gets or sets messages size.</summary>
        [Option("msg-size", Required = false, Separator = ',', HelpText = "Use messages of size <size> bytes (minimum default 14).")]
        public IEnumerable<int> MessageSize { get; set; } = new int[] { 64 };

        /// <summary>Gets or sets messages send per second.</summary>
        [Option("msgs-per-sec", Required = false, Separator = ',', HelpText = "Set number of messages-per-second (default = 100)")]
        public IEnumerable<int> MessagesPerSecond { get; set; } = new int[] { 100 };

        /// <summary>Gets or sets a value indicating whether one way latency should be measured instread of round trip time.</summary>
        [Option("owl", Required = false, HelpText = "Measure one way latencies using timestamps of system clocks (client and server clock needs to be synchronized!)")]
        public bool MeasureOneWayLatency { get; set; } = false;

        /// <summary>Gets or sets a value indicating whether if Rperf should create a symmetric network load between client and server.</summary>
        [Option("syml-load", Required = false, HelpText = "Creates symmetric network load between client and server using ping packet size for pong packets. If this flag is not set pong packets have minimal packet size (16 bytes).")]
        public bool SymmetricNetworkLoad { get; set; } = false;

        /// <summary>Gets or sets warmup duration before test is started.</summary>
        [Option("warmup", Required = false, HelpText = "Warmup time before test is started (disabled by default)")]
        public int WarmupTime { get; set; } = 0;
    }
}
