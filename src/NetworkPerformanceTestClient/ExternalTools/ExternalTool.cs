// <copyright file="ExternalTool.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System;
    using NetworkPacketCapturer;
    using SharpPcap;

    /// <summary>
    /// Abstract base class for external tools.
    /// </summary>
    public abstract class ExternalTool
    {
        /// <summary>Gets the timout to wait for packets when stopping packet capturing.</summary>
        protected static readonly TimeSpan PacketAnalysisTimeout = TimeSpan.FromMilliseconds(30_000);

        /// <summary>
        /// Get the network interface which should be used for packet capturing by <see cref="CommandLineOptions"/>.
        /// </summary>
        /// <param name="cmdOptions">Command line options.</param>
        /// <returns>The network interface that should be used for packet capturing.</returns>
        protected static ICaptureDevice GetCapturingNetworkInterface(CommandLineOptions cmdOptions)
        {
            if (cmdOptions.CapturePackets)
            {
                if (cmdOptions.CapturingInterfaceName.Length > 0)
                {
                    return PacketCapturer.GetNetworkInterfaceFromName(cmdOptions.CapturingInterfaceName);
                }
                else if (cmdOptions.CapturingInterfaceFriendlyName.Length > 0)
                {
                    return PacketCapturer.GetNetworkInterfaceFromFriendlyName(cmdOptions.CapturingInterfaceFriendlyName);
                }
                else
                {
                    throw new ArgumentException("Whether network interface name nor network interface friendly name are configured");
                }
            }
            else
            {
                throw new ArgumentException("Packet capturing is not enabled");
            }
        }
    }
}
