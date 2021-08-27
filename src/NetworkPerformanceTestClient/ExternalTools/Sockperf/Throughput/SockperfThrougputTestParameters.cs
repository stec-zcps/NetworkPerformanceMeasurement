// <copyright file="SockperfThrougputTestParameters.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    /// <summary>
    /// Test parameters for Sockperf in throughput mode.
    /// </summary>
    public class SockperfThrougputTestParameters : SockperfTestParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SockperfThrougputTestParameters"/> class.
        /// </summary>
        /// <param name="ip">IP address of Sockperf server.</param>
        /// <param name="port">Port of Sockperf server.</param>
        /// <param name="time">Test duration.</param>
        /// <param name="protocol">Protocol.</param>
        /// <param name="messageSize">Messages size.</param>
        public SockperfThrougputTestParameters(string ip, int port, int time, string protocol, int messageSize)
            : base(ip, port, time, protocol, messageSize, -1, "througput")
        {
        }
    }
}
