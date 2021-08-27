// <copyright file="RperfPacketCapturer.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPacketCapturer.Rperf
{
    using System;
    using NetworkPacketCapturer.KunbusTap;
    using SharpPcap;

    /// <summary>
    /// Packet capturer for Rperf.
    /// </summary>
    public class RperfPacketCapturer : PacketCapturer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RperfPacketCapturer"/> class.
        /// </summary>
        /// <param name="networkInterface">Network interface that should be used for packet capturing.</param>
        public RperfPacketCapturer(ICaptureDevice networkInterface)
            : base(networkInterface)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RperfPacketCapturer"/> class.
        /// </summary>
        /// <param name="networkInterface">Network interface that should be used for packet capturing.</param>
        /// <param name="kunbusTapConfig">Configuration for Kunbus TAP CURIOUS.</param>
        public RperfPacketCapturer(ICaptureDevice networkInterface, KunbusTapConfiguration kunbusTapConfig)
            : base(networkInterface, kunbusTapConfig)
        {
        }

        /// <inheritdoc/>
        protected override int PingPacketSize => this.MessageSize;

        /// <inheritdoc/>
        protected override int PongPacketSize => 16;

        /// <inheritdoc/>
        protected override long GetPacketIndex(byte[] packetPayload)
        {
            var packetIndexBytes = new byte[8];
            Array.Copy(packetPayload, 0, packetIndexBytes, 0, 8);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(packetIndexBytes);
            }

            var packetIndex = BitConverter.ToInt32(packetIndexBytes, 0);

            return packetIndex;
        }
    }
}
