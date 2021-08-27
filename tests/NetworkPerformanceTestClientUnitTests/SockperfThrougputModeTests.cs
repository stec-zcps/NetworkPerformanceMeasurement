// <copyright file="SockperfThrougputModeTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPerformanceTestClientUnitTests
{
    using NetworkPerformanceTestClient.ExternalTools.Sockperf;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="Sockperf"/> in throughput mode.
    /// </summary>
    public class SockperfThrougputModeTests
    {
        /// <summary>
        /// Test parsing of Sockperf output in throughput mode.
        /// </summary>
        [Fact]
        public void ParseSockperfThrougputOutput()
        {
            var sockperfThrougputOutput = "sockperf: Summary: Message Rate is 75439 [msg/sec]\n" +
                                          "sockperf: Summary: BandWidth is 105.902 MBps (847.215 Mbps)";

            var result = SockperfThrougputResult.FromConsoleOutput(null, sockperfThrougputOutput);

            Assert.Equal(75439, result.MessageRatePerSecond);
            Assert.Equal(847.215, result.BandWidthInMbps, 3);
        }
    }
}
