// <copyright file="KunbusTapConfiguration.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPacketCapturer.KunbusTap
{
    /// <summary>
    /// Configuration for Kunbus TAP CURIOUS.
    /// </summary>
    public class KunbusTapConfiguration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KunbusTapConfiguration"/> class.
        /// </summary>
        /// <param name="tapPortClient">Port where the client is connected.</param>
        /// <param name="tapPortServer">Port where the server is connected.</param>
        public KunbusTapConfiguration(string tapPortClient, string tapPortServer)
        {
            this.TapPortClient = tapPortClient;
            this.TapPortServer = tapPortServer;
        }

        /// <summary>Gets the port where the client is connected.</summary>
        public string TapPortClient { get; private set; }

        /// <summary>Gets the port where the server is connected.</summary>
        public string TapPortServer { get; private set; }
    }
}
