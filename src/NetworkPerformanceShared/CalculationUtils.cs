// <copyright file="CalculationUtils.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPerformanceShared
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Util class that offerns some calculations (e.g. standard deviation).
    /// </summary>
    public static class CalculationUtils
    {
        /// <summary>
        /// Gets the standard deviation of the given values.
        /// </summary>
        /// <param name="values">Values for which the standard deviation should be calculated.</param>
        /// <returns>The standard deviation of the given values.</returns>
        public static float GetStandardDeviation(IEnumerable<float> values)
        {
            double average = values.Average();

            double sum = values.Sum(d => (d - average) * (d - average));

            return (float)Math.Sqrt(sum / values.Count());
        }
    }
}
