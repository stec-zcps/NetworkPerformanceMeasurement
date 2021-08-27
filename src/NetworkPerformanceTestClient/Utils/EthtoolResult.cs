// <copyright file="EthtoolResult.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPerformanceTestClient.Utils
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// Result of ethtool execution.
    /// </summary>
    public class EthtoolResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EthtoolResult"/> class.
        /// </summary>
        /// <param name="interfaceName">Name of network interface for which ethtool was executed.</param>
        /// <param name="ethtoolOutput">Output out ethtool.</param>
        public EthtoolResult(string interfaceName, string ethtoolOutput)
        {
            this.InterfaceName = interfaceName;

            var matches = Regex.Matches(ethtoolOutput, @"Speed:[ |\t]*([0-9]+)Mb/s");

            if (matches.Count == 1)
            {
                if (matches[0].Groups.Count == 2)
                {
                    this.InterfaceSpeedMbps = int.Parse(matches[0].Groups[1].Value);
                }
            }
        }

        /// <summary> Gets or sets the name of the interface.</summary>
        public string InterfaceName { get; set; }

        /// <summary> Gets or sets interface speed in Mbps.</summary>
        public int InterfaceSpeedMbps { get; set; }
    }
}
