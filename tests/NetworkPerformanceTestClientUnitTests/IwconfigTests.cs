// <copyright file="IwconfigTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    /// Tests for <see cref="Iwconfig"/>.
    /// </summary>
    public class IwconfigTests
    {
        /// <summary>
        /// Test parsing of iwconfig output.
        /// </summary>
        [Fact]
        public void ParseOutput()
        {
            var iwconfigOutput = @"lo        no wireless extensions.

                                   enp2s0    no wireless extensions.

                                   enp3s0    no wireless extensions.

                                   wlo2      IEEE 802.11  ESSID:off/any  
                                              Mode:Managed  Access Point: Not-Associated   Tx-Power=-2147483648 dBm   
                                              Retry short limit:7   RTS thr:off   Fragment thr:off
                                              Power Management:on
          
                                   wlp1s0    IEEE 802.11  ESSID:""ZCPS""  
                                             Mode: Managed Frequency:5.22 GHz Access Point: C0:C9:E3:A2:E3:11
                                             Bit Rate=600.4 Mb/s   Tx-Power=22 dBm
                                             Retry short limit:7   RTS thr:off Fragment thr: off
                                             Power Management: on
                                             Link Quality=61/70  Signal level=-49 dBm
                                             Rx invalid nwid:0  Rx invalid crypt: 0  Rx invalid frag: 0
                                             Tx excessive retries: 0  Invalid misc:671   Missed beacon:0

                                   docker0   no wireless extensions.

                                   br - 17b4c299132d    no wireless extensions.

                                   vethb203322 no wireless extensions.

                                   br - dc96d4daf2f9  no wireless extensions.

                                   vethc825c44  no wireless extensions.

                                   vethc19c02d  no wireless extensions.

                                   veth28235e0  no wireless extensions";

            var iwconfigResult = new IwconfigResult("wlp1s0", iwconfigOutput);

            Assert.Equal("wlp1s0", iwconfigResult.InterfaceName);
            Assert.Equal("ZCPS", iwconfigResult.SSID);
            Assert.Equal("C0:C9:E3:A2:E3:11", iwconfigResult.AccessPointMac);
            Assert.Equal(5.22, iwconfigResult.FrequencyGhz, 2);
            Assert.Equal(600.4, iwconfigResult.BitRateMbps, 2);
            Assert.Equal("61/70", iwconfigResult.LinkQuality);
            Assert.Equal(-49, iwconfigResult.SignalLeveldBm);
        }
    }
}
