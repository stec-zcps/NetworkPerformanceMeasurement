// <copyright file="Ethtool.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using NetworkPerformanceShared;

    /// <summary>
    /// Allows the execution of ethtool via shell on local system.
    /// </summary>
    public class Ethtool
    {
        /// <summary>
        /// Executes ethtool via shell.
        /// </summary>
        /// <param name="interfaceName">Name of the interface for which iwconfig shall be executed.</param>
        /// <returns><see cref="EthtoolResult"/> parsed from string output of ethtool.</returns>
        public static EthtoolResult Run(string interfaceName)
        {
            var command = $"ethtool {interfaceName}";
            var output = command.Bash();
            var result = new EthtoolResult(interfaceName, output);

            return result;
        }
    }
}
