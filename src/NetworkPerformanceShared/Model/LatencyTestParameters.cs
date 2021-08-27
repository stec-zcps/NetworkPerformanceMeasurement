// <copyright file="LatencyTestParameters.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
namespace NetworkPerformanceShared.Model
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represents the parameters of a latency test.
    /// </summary>
    public class LatencyTestParameters
    {
        /// <summary>Gets or sets id of the the <see cref="LatencyTestParameters"/>.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets <see cref="LatencyTestResult"/> in which these parameters have been used.</summary>
        [ForeignKey("LatencyTestResultId")]
        public LatencyTestResult RelatedTestResult { get; set; }

        /// <summary>Gets or sets timestamp when test was started.</summary>
        public DateTime StartTimestamp { get; set; }

        /// <summary>Gets or sets technology used for packet transimission (e.g. Ethernet, WiFi, ...).</summary>
        public string TransmissionTechnology { get; set; }

        /// <summary>Gets or sets size of test message in bytes.</summary>
        public int MessageSize { get; set; }

        /// <summary>Gets or sets messages sent per second.</summary>
        public int MessagesPerSecond { get; set; }

        /// <summary>Gets or sets duration of the test in seconds.</summary>
        public int TestDuration { get; set; }

        /// <summary>Gets or sets server used for testing.</summary>
        public string TestServer { get; set; }

        /// <summary>Gets or sets tool used for testing.</summary>
        public string TestTool { get; set; }

        /// <summary>Gets or sets mode of the used test tool.</summary>
        public string TestMode { get; set; }

        /// <summary>Gets or sets protocol used for testing.</summary>
        public string Protocol { get; set; }

        /// <summary>Gets or sets comment to describe some test details.</summary>
        public string Comment { get; set; }
    }
}