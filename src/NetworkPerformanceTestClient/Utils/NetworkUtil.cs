// <copyright file="NetworkUtil.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;
    using Newtonsoft.Json;

    /// <summary>
    /// Util class for to get information about systems network interface more easily.
    /// </summary>
    public class NetworkUtil
    {
        /// <summary>
        /// Get IP of a network interface.
        /// </summary>
        /// <param name="interfaceName">Network interface name for which the IP should be obtained.</param>
        /// <returns>IP of specified network interface.</returns>
        public static IPAddress GetIpOfNetworkInterface(string interfaceName)
        {
            var networkInterface = NetworkUtil.GetNetworkInterfaceByName(interfaceName);
            var ipAddresses = networkInterface.GetIPProperties().UnicastAddresses.Where(ip => ip.Address.AddressFamily == AddressFamily.InterNetwork).ToList();
            if (ipAddresses.Count == 1)
            {
                return ipAddresses[0].Address;
            }
            else
            {
                throw new ArgumentException($"Network interface '{interfaceName}' has multiple IPs: {JsonConvert.SerializeObject(ipAddresses)}");
            }
        }

        /// <summary>
        /// Gets the transmission technology of a network interface.
        /// </summary>
        /// <param name="interfaceName">Network interface name for which the transmission technology should be obtained.</param>
        /// <returns>Transmission technology of the specified network interface.</returns>
        public static string GetTransmissionTechnologyOfInterface(string interfaceName)
        {
            return NetworkUtil.GetInterfaceTypeByName(interfaceName).ToString();
        }

        /// <summary>
        /// Gets the details of a network interface for documentation purposes (e.g. storing in database).
        /// </summary>
        /// <param name="interfaceName">Name of the network interface.</param>
        /// <returns>Details about the network interface.</returns>
        public static string GetInterfaceDetailsForDocumentation(string interfaceName)
        {
            switch (NetworkUtil.GetInterfaceTypeByName(interfaceName))
            {
                case NetworkInterfaceType.Ethernet:
                    var ethtoolResult = Ethtool.Run(interfaceName);
                    return $"Network Interface: {ethtoolResult.InterfaceName} (IP: {NetworkUtil.GetIpOfNetworkInterface(interfaceName)}) | " +
                           $"Speed: {ethtoolResult.InterfaceSpeedMbps} Mbit/s";

                case NetworkInterfaceType.WiFi:
                    var iwconfigResult = Iwconfig.Run(interfaceName);
                    return $"Network Interface: {iwconfigResult.InterfaceName} (IP: {NetworkUtil.GetIpOfNetworkInterface(interfaceName)}) | " +
                           $"Speed: {iwconfigResult.BitRateMbps} Mbit/s | " +
                           $"SSID: {iwconfigResult.SSID} | " +
                           $"Access Point (MAC): {iwconfigResult.AccessPointMac} | " +
                           $"Frequency: {iwconfigResult.FrequencyGhz} GHz | " +
                           $"Link Quality: {iwconfigResult.LinkQuality} | " +
                           $"Signal: {iwconfigResult.SignalLeveldBm} dbm";

                case NetworkInterfaceType.Cellular:
                    return string.Empty;

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Gets the type of a network interface by name.
        /// </summary>
        /// <param name="interfaceName">Network interface name for which the type should be obtained.</param>
        /// <returns>Type of the specified network interface.</returns>
        public static NetworkInterfaceType GetInterfaceTypeByName(string interfaceName)
        {
            if (interfaceName.StartsWith("lo"))
            {
                return NetworkInterfaceType.Loopback;
            }
            else if (interfaceName.StartsWith("eth") || interfaceName.StartsWith("enp"))
            {
                return NetworkInterfaceType.Ethernet;
            }
            else if (interfaceName.StartsWith("wl"))
            {
                return NetworkInterfaceType.WiFi;
            }
            else if (interfaceName.StartsWith("ww"))
            {
                return NetworkInterfaceType.Cellular;
            }
            else
            {
                return NetworkInterfaceType.Unkown;
            }
        }

        private static NetworkInterface GetNetworkInterfaceByName(string interfaceName)
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            try
            {
                return adapters.First(ni => ni.Name.Equals(interfaceName));
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException($"Unkown network interface '{interfaceName}': {e}");
            }
        }
    }
}
