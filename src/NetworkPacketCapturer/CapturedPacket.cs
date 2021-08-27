// <copyright file="CapturedPacket.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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

namespace NetworkPacketCapturer
{
    using System.Net;
    using NetworkPacketCapturer.KunbusTap;
    using SharpPcap;

    /// <summary>
    /// Captured network package.
    /// </summary>
    public class CapturedPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CapturedPacket"/> class.
        /// </summary>
        /// <param name="direction">Direction of packet.</param>
        /// <param name="index">Packet index.</param>
        /// <param name="packetSize">Packet size in bytes.</param>
        /// <param name="sourceIp">Source IP address.</param>
        /// <param name="sourcePort">Source port.</param>
        /// <param name="destinationIp">Destination IP address.</param>
        /// <param name="destinationPort">Destination port.</param>
        /// <param name="timestamp">Timestamp.</param>
        public CapturedPacket(
            CapturedPacketDirection direction,
            long index,
            int packetSize,
            IPAddress sourceIp,
            ushort sourcePort,
            IPAddress destinationIp,
            ushort destinationPort,
            PosixTimeval timestamp)
        {
            this.Direction = direction;
            this.Index = index;
            this.PacketSize = packetSize;
            this.SourceIp = sourceIp;
            this.SourcePort = sourcePort;
            this.DestinationIP = destinationIp;
            this.DestinationPort = destinationPort;
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CapturedPacket"/> class.
        /// </summary>
        /// <param name="direction">Direction of packet.</param>
        /// <param name="index">Packet index.</param>
        /// <param name="packetSize">Packet size in bytes.</param>
        /// <param name="sourceIp">Source IP address.</param>
        /// <param name="sourcePort">Source port.</param>
        /// <param name="destinationIp">Destination IP address.</param>
        /// <param name="destinationPort">Destination port.</param>
        /// <param name="timestamp">Timestamp.</param>
        /// <param name="kunbusTapData">Data from Kunbus TAP CURIOUS.</param>
        public CapturedPacket(
            CapturedPacketDirection direction,
            long index,
            int packetSize,
            IPAddress sourceIp,
            ushort sourcePort,
            IPAddress destinationIp,
            ushort destinationPort,
            PosixTimeval timestamp,
            KunbusTapData kunbusTapData)
        : this(direction, index, packetSize, sourceIp, sourcePort, destinationIp, destinationPort, timestamp)
        {
            this.KunbusTapData = kunbusTapData;
        }

        /// <summary> Gets the index.</summary>
        public long Index { get; private set; }

        /// <summary> Gets the packet size in bytes.</summary>
        public int PacketSize { get; private set; }

        /// <summary> Gets the source IP address.</summary>
        public IPAddress SourceIp { get; private set; }

        /// <summary> Gets the source port.</summary>
        public ushort SourcePort { get; private set; }

        /// <summary> Gets the destination IP address.</summary>
        public IPAddress DestinationIP { get; private set; }

        /// <summary> Gets the destination port.</summary>
        public ushort DestinationPort { get; private set; }

        /// <summary> Gets the timestamp.</summary>
        public PosixTimeval Timestamp { get; private set; }

        /// <summary> Gets the direction.</summary>
        public CapturedPacketDirection Direction { get; private set; }

        /// <summary> Gets or sets the <see cref="KunbusTapData"/>.</summary>
        public KunbusTapData KunbusTapData { get; set; }
    }
}
