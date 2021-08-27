// <copyright file="SockperPingPongModeTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    /// Tests for <see cref="Sockperf"/> in ping-pong mode.
    /// </summary>
    public class SockperPingPongModeTests
    {
        /// <summary>
        /// Test parsing of Sockperf output in ping-pong mode.
        /// </summary>
        [Fact]
        public void ParseSockperfPingPongOutput()
        {
            var sockperfPingPongOutput = @": sockperf: == version #3.7-5.git3c89bb2b0094 ==
                                            sockperf[CLIENT] send on:sockperf: using recvfrom() to block on socket(s)

                                            [ 0] IP = 127.0.0.1       PORT = 11111 # TCP
                                            sockperf: Warmup stage (sending a few dummy messages)...
                                            sockperf: Starting test...
                                            sockperf: Test end (interrupted by timer)
                                            sockperf: Test ended
                                            sockperf: [Total Run] RunTime=1.001 sec; Warm up time=400 msec; SentMessages=502; ReceivedMessages=501
                                            sockperf: ========= Printing statistics for Server No: 0
                                            sockperf: [Valid Duration] RunTime=0.550 sec; SentMessages=276; ReceivedMessages=276
                                            sockperf: ====> avg-latency=111.959 (std-dev=15.671)
                                            sockperf: # dropped messages = 0; # duplicated messages = 0; # out-of-order messages = 0
                                            sockperf: Summary: Latency is 111.959 usec
                                            sockperf: Total 276 observations; each percentile contains 2.76 observations
                                            sockperf: ---> <MAX> observation =  156.083
                                            sockperf: ---> percentile 99.999 =  156.083
                                            sockperf: ---> percentile 99.990 =  156.083
                                            sockperf: ---> percentile 99.900 =  156.083
                                            sockperf: ---> percentile 99.000 =  149.074
                                            sockperf: ---> percentile 90.000 =  129.316
                                            sockperf: ---> percentile 75.000 =  121.045
                                            sockperf: ---> percentile 50.000 =  114.020
                                            sockperf: ---> percentile 25.000 =  104.187
                                            sockperf: ---> <MIN> observation =   60.214";

            var sentMessages = SockperfLatencyResult.GetSentMessagesValidFromOutput(sockperfPingPongOutput);
            var receivedMessages = SockperfLatencyResult.GetReceivedMessagesValidFromOutput(sockperfPingPongOutput);

            var lostMessages = sentMessages - receivedMessages;

            Assert.Equal(276, sentMessages);
            Assert.Equal(276, receivedMessages);
            Assert.Equal(0, lostMessages);
        }
    }
}
