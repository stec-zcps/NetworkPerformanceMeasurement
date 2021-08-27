// <copyright file="CommandLineOptions.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPerformanceTestClient.ExternalTools
{
    using System.Collections.Generic;
    using CommandLine;
    using NetworkPerformanceTestClient.Utils;

    /// <summary>
    /// Command line options to configure program execution.
    /// </summary>
    public class CommandLineOptions
    {
        private string transmissionTechnology = string.Empty;

        /// <summary>Gets or sets name of network interface used for sending packets.</summary>
        [Option("interface", Required = true, HelpText = "Gest name of network interface used for sending packets")]
        public string NetworkInterfaceName { get; set; }

        /// <summary>Gets or sets technology used for packet transmission (for test result documentation). It will be automatically determined from the network interface, but can be overwritten by this option..</summary>
        [Option("transmissionTechnology", Required = false, HelpText = "Technology used for packet transmission (for test result documentation). It will be automatically determined from the network interface, but can be overwritten by this option.")]
        public string TransmissionTechnology
        {
            get
            {
                if (string.IsNullOrEmpty(this.transmissionTechnology))
                {
                    return NetworkUtil.GetTransmissionTechnologyOfInterface(this.NetworkInterfaceName);
                }
                else
                {
                    return this.transmissionTechnology;
                }
            }

            set
            {
                this.transmissionTechnology = value;
            }
        }

        /// <summary>Gets or sets comment to add some details for test documentation.</summary>
        [Option("comment", Required = false, HelpText = "Comment to add some details for test documentation")]
        public string Comment { get; set; } = string.Empty;

        /// <summary>Gets a value indicating whether packets should be captured or not.</summary>
        public bool CapturePackets
        {
            get
            {
                if (this.CapturingInterfaceName.Length == 0 && this.CapturingInterfaceFriendlyName.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /// <summary>Gets or sets name of the network interface used for packet capturing.</summary>
        [Option("captureInterfaceName", Required = false, HelpText = "Name of the network interface used for packet capturing")]
        public string CapturingInterfaceName { get; set; } = string.Empty;

        /// <summary>Gets or sets friendly name of the network interface used for packet capturing.</summary>
        [Option("captureInterfaceFriendlyName", Required = false, HelpText = "Friendly name of the network interface used for packet capturing")]
        public string CapturingInterfaceFriendlyName { get; set; } = string.Empty;

        /// <summary>Gets or sets a value indicating whether if sent packages are captured via Kunbus TAP CURIOUS.</summary>
        [Option("tap", Required = false, HelpText = "Capture sent packages via Kunbus TAP CURIOUS")]
        public bool KunbusTapAvailable { get; set; } = false;

        /// <summary>Gets or sets comma-seperated list of CPU core ids to set CPU affinity of measurment tool.</summary>
        [Option("cpu-affinity", Required = false, Separator = ',', HelpText = "Comma-seperated list of CPU core ids to set CPU affinity of measurment tool")]
        public IEnumerable<int> CpuAffinity { get; set; } = new int[] { };
    }
}
