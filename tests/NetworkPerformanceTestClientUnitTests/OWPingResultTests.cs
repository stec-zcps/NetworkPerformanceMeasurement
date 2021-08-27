// <copyright file="OWPingResultTests.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using NetworkPerformanceTestClient.ExternalTools.Owping;
    using Xunit;

    /// <summary>
    /// Tests for <see cref="OWPing"/> in throughput mode.
    /// </summary>
    public class OWPingResultTests
    {
        /// <summary>
        /// Test parsing of owping output.
        /// </summary>
        [Fact]
        public void ParseFromOutput()
        {
            var owpingOutput = @"Approximately 13.0 seconds until results available

                                --- owping statistics from [IPA-linux-0077.ipa.svc.fortknox.local]:9637 to [10.3.2.216]:9585 ---
                                SID:    0a0302d8e44e5e2a5b368d965b5f70ab
                                seq_no=0 delay=2.593994e-01 ms (sync, err=0.389 ms) sent=1621352363.369304 recv=1621352363.369563
                                seq_no=1 delay=2.450943e-01 ms (sync, err=0.389 ms) sent=1621352363.391627 recv=1621352363.391872
                                seq_no=2 delay=2.412796e-01 ms (sync, err=0.389 ms) sent=1621352363.480084 recv=1621352363.480325
                                seq_no=3 delay=1.544952e-01 ms (sync, err=0.389 ms) sent=1621352363.505011 recv=1621352363.505165
                                seq_no=4 delay=6.008148e-02 ms (sync, err=0.389 ms) sent=1621352363.561842 recv=1621352363.561902
                                seq_no=5 delay=1.807213e-01 ms (sync, err=0.389 ms) sent=1621352363.584496 recv=1621352363.584676
                                seq_no=6 delay=2.112389e-01 ms (sync, err=0.389 ms) sent=1621352363.619139 recv=1621352363.619350
                                seq_no=7 delay=1.015663e-01 ms (sync, err=0.389 ms) sent=1621352363.632006 recv=1621352363.632107
                                seq_no=8 delay=1.316071e-01 ms (sync, err=0.389 ms) sent=1621352363.684407 recv=1621352363.684538
                                seq_no=9 delay=1.235008e-01 ms (sync, err=0.389 ms) sent=1621352363.773228 recv=1621352363.773351
                                seq_no=10 delay=1.511574e-01 ms (sync, err=0.389 ms) sent=1621352363.806665 recv=1621352363.806816
                                seq_no=11 delay=5.769730e-02 ms (sync, err=0.389 ms) sent=1621352363.817790 recv=1621352363.817847
                                seq_no=12 delay=1.902580e-01 ms (sync, err=0.389 ms) sent=1621352363.844875 recv=1621352363.845065
                                seq_no=13 delay=2.293587e-01 ms (sync, err=0.389 ms) sent=1621352363.865011 recv=1621352363.865240
                                seq_no=14 delay=7.152557e-02 ms (sync, err=0.389 ms) sent=1621352363.980827 recv=1621352363.980898
                                seq_no=15 delay=7.247925e-02 ms (sync, err=0.389 ms) sent=1621352364.006247 recv=1621352364.006319
                                seq_no=16 delay=2.126694e-01 ms (sync, err=0.389 ms) sent=1621352364.083767 recv=1621352364.083980
                                seq_no=17 delay=1.492500e-01 ms (sync, err=0.389 ms) sent=1621352364.142049 recv=1621352364.142198
                                seq_no=18 delay=1.974106e-01 ms (sync, err=0.389 ms) sent=1621352364.310388 recv=1621352364.310585
                                seq_no=19 delay=1.153946e-01 ms (sync, err=0.389 ms) sent=1621352364.345901 recv=1621352364.346016
                                seq_no=20 delay=1.516342e-01 ms (sync, err=0.389 ms) sent=1621352364.385466 recv=1621352364.385617
                                seq_no=21 delay=1.664162e-01 ms (sync, err=0.389 ms) sent=1621352364.432364 recv=1621352364.432530
                                seq_no=22 delay=1.101494e-01 ms (sync, err=0.389 ms) sent=1621352364.464637 recv=1621352364.464747
                                seq_no=23 delay=1.397133e-01 ms (sync, err=0.389 ms) sent=1621352364.506142 recv=1621352364.506281
                                seq_no=24 delay=1.702309e-01 ms (sync, err=0.389 ms) sent=1621352364.568075 recv=1621352364.568245
                                seq_no=25 delay=2.241135e-01 ms (sync, err=0.389 ms) sent=1621352364.669799 recv=1621352364.670023
                                seq_no=26 delay=2.336502e-01 ms (sync, err=0.389 ms) sent=1621352364.746490 recv=1621352364.746724
                                seq_no=27 delay=5.245209e-02 ms (sync, err=0.389 ms) sent=1621352364.749338 recv=1621352364.749390
                                seq_no=28 delay=2.145767e-01 ms (sync, err=0.389 ms) sent=1621352365.050957 recv=1621352365.051171
                                seq_no=29 delay=1.416206e-01 ms (sync, err=0.389 ms) sent=1621352365.069266 recv=1621352365.069408
                                seq_no=30 delay=3.066063e-01 ms (sync, err=0.389 ms) sent=1621352365.272500 recv=1621352365.272806
                                seq_no=31 delay=2.007484e-01 ms (sync, err=0.389 ms) sent=1621352365.464965 recv=1621352365.465166
                                seq_no=32 delay=2.484322e-01 ms (sync, err=0.389 ms) sent=1621352365.506742 recv=1621352365.506990
                                seq_no=33 delay=2.737045e-01 ms (sync, err=0.389 ms) sent=1621352365.605369 recv=1621352365.605642
                                seq_no=34 delay=1.816750e-01 ms (sync, err=0.389 ms) sent=1621352365.648003 recv=1621352365.648184
                                seq_no=35 delay=2.646446e-01 ms (sync, err=0.389 ms) sent=1621352365.714190 recv=1621352365.714455
                                seq_no=36 delay=1.816750e-01 ms (sync, err=0.389 ms) sent=1621352365.744614 recv=1621352365.744795
                                seq_no=37 delay=1.358986e-01 ms (sync, err=0.389 ms) sent=1621352365.954011 recv=1621352365.954147
                                seq_no=38 delay=1.440048e-01 ms (sync, err=0.389 ms) sent=1621352366.103853 recv=1621352366.103997
                                seq_no=39 delay=1.120567e-01 ms (sync, err=0.389 ms) sent=1621352366.129071 recv=1621352366.129183
                                seq_no=40 delay=4.577637e-02 ms (sync, err=0.389 ms) sent=1621352366.139946 recv=1621352366.139991
                                seq_no=41 delay=3.104210e-01 ms (sync, err=0.389 ms) sent=1621352366.254590 recv=1621352366.254900
                                seq_no=42 delay=1.168251e-01 ms (sync, err=0.389 ms) sent=1621352366.318781 recv=1621352366.318898
                                seq_no=43 delay=1.935959e-01 ms (sync, err=0.389 ms) sent=1621352366.544816 recv=1621352366.545010
                                seq_no=44 delay=1.115799e-01 ms (sync, err=0.389 ms) sent=1621352366.714213 recv=1621352366.714325
                                seq_no=45 delay=2.455711e-01 ms (sync, err=0.389 ms) sent=1621352366.724637 recv=1621352366.724882
                                seq_no=46 delay=7.343292e-02 ms (sync, err=0.389 ms) sent=1621352366.732542 recv=1621352366.732615
                                seq_no=47 delay=1.411438e-01 ms (sync, err=0.389 ms) sent=1621352366.752260 recv=1621352366.752401
                                seq_no=48 delay=2.636909e-01 ms (sync, err=0.389 ms) sent=1621352366.782999 recv=1621352366.783262
                                seq_no=49 delay=1.358986e-01 ms (sync, err=0.389 ms) sent=1621352367.018325 recv=1621352367.018461
                                seq_no=50 delay=1.897812e-01 ms (sync, err=0.389 ms) sent=1621352367.086227 recv=1621352367.086417
                                seq_no=51 delay=1.077652e-01 ms (sync, err=0.389 ms) sent=1621352367.135616 recv=1621352367.135724
                                seq_no=52 delay=5.626678e-02 ms (sync, err=0.389 ms) sent=1621352367.224924 recv=1621352367.224980
                                seq_no=53 delay=2.188683e-01 ms (sync, err=0.389 ms) sent=1621352367.349819 recv=1621352367.350038
                                seq_no=54 delay=1.521111e-01 ms (sync, err=0.389 ms) sent=1621352367.363549 recv=1621352367.363701
                                seq_no=55 delay=2.226830e-01 ms (sync, err=0.389 ms) sent=1621352367.922957 recv=1621352367.923180
                                seq_no=56 delay=1.177788e-01 ms (sync, err=0.389 ms) sent=1621352367.927290 recv=1621352367.927408
                                seq_no=57 delay=1.134872e-01 ms (sync, err=0.389 ms) sent=1621352367.985610 recv=1621352367.985723
                                seq_no=58 delay=1.788139e-01 ms (sync, err=0.389 ms) sent=1621352368.017638 recv=1621352368.017817
                                seq_no=59 delay=1.959801e-01 ms (sync, err=0.389 ms) sent=1621352368.051678 recv=1621352368.051874
                                seq_no=60 delay=-4.911423e-02 ms (sync, err=0.389 ms) sent=1621352368.052677 recv=1621352368.052628
                                seq_no=61 delay=2.107620e-01 ms (sync, err=0.389 ms) sent=1621352368.208392 recv=1621352368.208603
                                seq_no=62 delay=1.740456e-01 ms (sync, err=0.389 ms) sent=1621352368.235125 recv=1621352368.235299
                                seq_no=63 delay=2.498627e-01 ms (sync, err=0.389 ms) sent=1621352368.298361 recv=1621352368.298611
                                seq_no=64 delay=2.417564e-01 ms (sync, err=0.389 ms) sent=1621352368.317833 recv=1621352368.318075
                                seq_no=65 delay=3.509521e-01 ms (sync, err=0.389 ms) sent=1621352368.427781 recv=1621352368.428132
                                seq_no=66 delay=1.602173e-01 ms (sync, err=0.389 ms) sent=1621352368.517969 recv=1621352368.518129
                                seq_no=67 delay=1.578331e-01 ms (sync, err=0.389 ms) sent=1621352368.615072 recv=1621352368.615230
                                seq_no=68 delay=2.112389e-01 ms (sync, err=0.389 ms) sent=1621352368.763339 recv=1621352368.763550
                                seq_no=69 delay=1.573563e-01 ms (sync, err=0.389 ms) sent=1621352368.808558 recv=1621352368.808715
                                seq_no=70 delay=1.840591e-01 ms (sync, err=0.389 ms) sent=1621352369.185752 recv=1621352369.185936
                                seq_no=71 delay=2.198219e-01 ms (sync, err=0.389 ms) sent=1621352369.289786 recv=1621352369.290006
                                seq_no=72 delay=2.179146e-01 ms (sync, err=0.389 ms) sent=1621352369.308990 recv=1621352369.309208
                                seq_no=73 delay=2.260208e-01 ms (sync, err=0.389 ms) sent=1621352369.326297 recv=1621352369.326523
                                seq_no=74 delay=2.369881e-01 ms (sync, err=0.389 ms) sent=1621352369.506053 recv=1621352369.506290
                                seq_no=75 delay=2.198219e-01 ms (sync, err=0.389 ms) sent=1621352369.534188 recv=1621352369.534408
                                seq_no=76 delay=1.459122e-01 ms (sync, err=0.389 ms) sent=1621352369.558842 recv=1621352369.558988
                                seq_no=77 delay=2.369881e-01 ms (sync, err=0.389 ms) sent=1621352369.638138 recv=1621352369.638375
                                seq_no=78 delay=1.893044e-01 ms (sync, err=0.389 ms) sent=1621352369.696631 recv=1621352369.696820
                                seq_no=79 delay=-2.717972e-02 ms (sync, err=0.389 ms) sent=1621352369.697387 recv=1621352369.697360
                                seq_no=80 delay=2.360344e-01 ms (sync, err=0.389 ms) sent=1621352369.769613 recv=1621352369.769849
                                seq_no=81 delay=2.408028e-01 ms (sync, err=0.389 ms) sent=1621352369.981880 recv=1621352369.982121
                                seq_no=82 delay=2.388954e-01 ms (sync, err=0.389 ms) sent=1621352370.069805 recv=1621352370.070044
                                seq_no=83 delay=3.480911e-02 ms (sync, err=0.389 ms) sent=1621352370.076711 recv=1621352370.076746
                                seq_no=84 delay=2.121925e-01 ms (sync, err=0.389 ms) sent=1621352370.123003 recv=1621352370.123215
                                seq_no=85 delay=3.252029e-01 ms (sync, err=0.389 ms) sent=1621352370.209329 recv=1621352370.209654
                                seq_no=86 delay=7.295609e-02 ms (sync, err=0.389 ms) sent=1621352370.227558 recv=1621352370.227631
                                seq_no=87 delay=1.888275e-01 ms (sync, err=0.389 ms) sent=1621352370.412215 recv=1621352370.412404
                                seq_no=88 delay=1.487732e-01 ms (sync, err=0.389 ms) sent=1621352370.502980 recv=1621352370.503129
                                seq_no=89 delay=1.120567e-01 ms (sync, err=0.389 ms) sent=1621352370.605995 recv=1621352370.606107
                                seq_no=90 delay=2.336502e-02 ms (sync, err=0.389 ms) sent=1621352370.616263 recv=1621352370.616286
                                seq_no=91 delay=3.814697e-02 ms (sync, err=0.389 ms) sent=1621352370.631920 recv=1621352370.631958
                                seq_no=92 delay=1.969337e-01 ms (sync, err=0.389 ms) sent=1621352370.746685 recv=1621352370.746882
                                seq_no=93 delay=3.180504e-01 ms (sync, err=0.389 ms) sent=1621352370.824747 recv=1621352370.825065
                                seq_no=94 delay=2.250671e-01 ms (sync, err=0.389 ms) sent=1621352371.028749 recv=1621352371.028974
                                seq_no=95 delay=8.440018e-02 ms (sync, err=0.389 ms) sent=1621352371.408364 recv=1621352371.408448
                                seq_no=96 delay=1.821518e-01 ms (sync, err=0.389 ms) sent=1621352371.557757 recv=1621352371.557939
                                seq_no=97 delay=2.465248e-01 ms (sync, err=0.389 ms) sent=1621352371.629432 recv=1621352371.629678
                                seq_no=98 delay=1.530647e-01 ms (sync, err=0.389 ms) sent=1621352371.694884 recv=1621352371.695037
                                seq_no=99 delay=2.884865e-01 ms (sync, err=0.389 ms) sent=1621352371.774316 recv=1621352371.774604

                                --- owping statistics from [IPA-linux-0077.ipa.svc.fortknox.local]:9637 to [10.3.2.216]:9585 ---
                                SID:    0a0302d8e44e5e2a5b368d965b5f70ab
                                first:  2021-05-18T17:39:23.369
                                last:   2021-05-18T17:39:31.774
                                100 sent, 0 lost (0.000%), 0 duplicates
                                one-way delay min/median/max = -0.0491/0.2/0.351 ms, (err=0.389 ms)
                                one-way jitter = 0.1 ms (P95-P50)
                                hops = 0 (consistently)
                                no reordering";

            var result = OWPingResult.FromOutput(null, owpingOutput);

            Assert.Equal(100, result.Measurments.Count);

            Assert.Equal(100, result.SentMessages);
            Assert.Equal(0, result.LostMessages);

            Assert.Equal(-0.0491, result.MinimumLatency, 5);
            Assert.Equal(0.2, result.AverageLatency, 5);
            Assert.Equal(0.351, result.MaximumLatency, 5);
        }
    }
}
