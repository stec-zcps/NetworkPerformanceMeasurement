// <copyright file="LatencyTestResult.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// Represtens the results of a latency test.
    /// </summary>
    public class LatencyTestResult
    {
        /// <summary>Gets or sets id of the the <see cref="LatencyTestResult"/>.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets label for this results (e.g. for use in dashboard).</summary>
        public string Label { get; set; }

        /// <summary>Gets or sets count of sent messages.</summary>
        public long SentMessages { get; set; }

        /// <summary>Gets or sets count of received messages.</summary>
        public long ReceivedMessages { get; set; }

        /// <summary>Gets or sets count of lost messages.</summary>
        public long LostMessages { get; set; }

        /// <summary>Gets or sets minimum of the measured latencies.</summary>
        public double MinimumLatency { get; set; }

        /// <summary>Gets or sets maximum of the measured latencies.</summary>
        public double MaximumLatency { get; set; }

        /// <summary>Gets or sets average of measured latencies.</summary>
        public double AverageLatency { get; set; }

        /// <summary>Gets or sets standard deviation of the measured latencies.</summary>
        public double StandardDeviationLatency { get; set; }

        /// <summary>Gets or sets minimum of the measured jitters.</summary>
        public double MinimumJitter { get; set; }

        /// <summary>Gets or sets maximum of the measured jitters.</summary>
        public double MaximumJitter { get; set; }

        /// <summary>Gets or sets average of measured jitters.</summary>
        public double AverageJitter { get; set; }

        /// <summary>Gets or sets standard deviation of the measured jitters.</summary>
        public double StandardDeviationJitter { get; set; }

        /// <summary>Gets or sets the parameters used for this test.</summary>
        public LatencyTestParameters TestParameters { get; set; }

        /// <summary>Gets or sets the related measurements of this test.</summary>
        [NotMapped]
        public List<LatencyTestMeasurment> TestMeasurments { get; set; } = new List<LatencyTestMeasurment>();
    }
}