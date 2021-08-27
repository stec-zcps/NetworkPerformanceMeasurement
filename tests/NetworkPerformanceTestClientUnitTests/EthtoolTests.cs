// <copyright file="EthtoolTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using NetworkPerformanceTestClient.Utils;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="Ethtool"/>.
    /// </summary>
    public class EthtoolTests
    {
        /// <summary>
        /// Test parsing of ehttool output.
        /// </summary>
        [Fact]
        public void ParseOutput()
        {
            var ethtoolgOutput = @"Settings for enp3s0:
									   Supported ports: [ TP ]
									   Supported link modes:   10baseT/Half 10baseT/Full 
																100baseT/Half 100baseT/Full 
																1000baseT/Full 
										Supported pause frame use: Symmetric
										Supports auto-negotiation: Yes
										Supported FEC modes: Not reported
										Advertised link modes:  10baseT/Half 10baseT/Full 
																100baseT/Half 100baseT/Full 
																1000baseT/Full 
										Advertised pause frame use: Symmetric
										Advertised auto-negotiation: Yes
										Advertised FEC modes: Not reported
										Speed: 100Mb/s
										Duplex: Full
										Port: Twisted Pair
										PHYAD: 1
										Transceiver: internal
										Auto-negotiation: on
										MDI-X: on (auto)
								Cannot get wake-on-lan settings: Operation not permitted
									Current message level: 0x00000007 (7)
												   drv probe link
									Link detected: yes";

            var ethtoolResult = new EthtoolResult("enp3s0", ethtoolgOutput);

            Assert.Equal("enp3s0", ethtoolResult.InterfaceName);
            Assert.Equal(100, ethtoolResult.InterfaceSpeedMbps);
        }
    }
}
