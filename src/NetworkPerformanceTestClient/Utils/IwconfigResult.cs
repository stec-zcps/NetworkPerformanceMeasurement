// <copyright file="IwconfigResult.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System.Globalization;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Result of iwconfig execution.
    /// </summary>
    public class IwconfigResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IwconfigResult"/> class.
        /// </summary>
        /// <param name="interfaceName">Name of network interface for which iwconfig was executed.</param>
        /// <param name="iwconfigOutput">Output of iwconfig.</param>
        public IwconfigResult(string interfaceName, string iwconfigOutput)
        {
            this.InterfaceName = interfaceName;

            var matches = Regex.Matches(iwconfigOutput, @$"{interfaceName}[ |\t]*IEEE 802.11[ |\t]*ESSID:""([a-z|A-Z|0-9]*)""[ |\t|\n|\r|\r\n]*Mode:[ |\t]*Managed[ |\t]*Frequency:[ |\t]*([0-9]+.[0-9]+) GHz[ |\t]*Access Point: ([a-z|A-Z|0-9|:]*)[ |\t|\n|\r|\r\n]*Bit Rate=([0-9]+.[0-9]+) Mb/s[a-z|A-Z|0-9|:|\-|=| |\t|\n|\r|\r\n]*Link Quality=([0-9]+/[0-9]+)[ |\t]*Signal level=([\-|0-9]+) dBm");

            if (matches.Count == 1)
            {
                if (matches[0].Groups.Count == 7)
                {
                    this.SSID = matches[0].Groups[1].Value;
                    this.FrequencyGhz = float.Parse(matches[0].Groups[2].Value, CultureInfo.InvariantCulture);
                    this.AccessPointMac = matches[0].Groups[3].Value;
                    this.BitRateMbps = float.Parse(matches[0].Groups[4].Value, CultureInfo.InvariantCulture);
                    this.LinkQuality = matches[0].Groups[5].Value;
                    this.SignalLeveldBm = int.Parse(matches[0].Groups[6].Value);
                }
            }
        }

        /// <summary> Gets or sets the name of the interface.</summary>
        public string InterfaceName { get; set; }

        /// <summary> Gets or sets SSID.</summary>
        public string SSID { get; set; }

        /// <summary> Gets or sets the MAC of the access point.</summary>
        public string AccessPointMac { get; set; }

        /// <summary> Gets or sets frequency in GHz.</summary>
        public float FrequencyGhz { get; set; }

        /// <summary> Gets or sets bit rate in Mbps.</summary>
        public float BitRateMbps { get; set; }

        /// <summary> Gets or sets the link quality.</summary>
        public string LinkQuality { get; set; }

        /// <summary> Gets or sets the signal level in dBm.</summary>
        public int SignalLeveldBm { get; set; }
    }
}
