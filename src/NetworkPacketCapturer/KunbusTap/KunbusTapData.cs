// <copyright file="KunbusTapData.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System;

    /// <summary>
    /// Represents the additional information (20 bytes) added by KUNBUS TAP CURIOUS to a captured packet.
    /// </summary>
    public class KunbusTapData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KunbusTapData"/> class.
        /// </summary>
        /// <param name="tapAdditional20Bytes">The 20 bytes with additional information added by KUNBUS TAP CURIOUS to a captured packet.</param>
        public KunbusTapData(byte[] tapAdditional20Bytes)
        {
            this.RawData = tapAdditional20Bytes;
            this.RawHexData = BitConverter.ToString(this.RawData).Replace("-", " ");

            Array.Copy(this.RawData, 0, this.FcsBytes, 0, 4);
            Array.Reverse(this.FcsBytes, 0, this.FcsBytes.Length);
            Array.Copy(this.RawData, 3, this.IdentifierBytes, 0, 6);
            Array.Reverse(this.IdentifierBytes, 0, this.IdentifierBytes.Length);
            Array.Copy(this.RawData, 10, this.PortBytes, 0, 1);
            Array.Reverse(this.PortBytes, 0, this.PortBytes.Length);

            // Array.Copy(rawData, 11, this.portHexBytes, 0, 1);
            // Array.Reverse(this.portHexBytes, 0, this.portHexBytes.Length);
            Array.Copy(this.RawData, 12, this.TimestampBytes, 0, 8);
            Array.Reverse(this.TimestampBytes, 0, this.TimestampBytes.Length);

            this.FcsHex = BitConverter.ToString(this.FcsBytes).Replace("-", string.Empty).ToLower();
            this.IdentifierHex = BitConverter.ToString(this.IdentifierBytes).Replace("-", string.Empty).ToLower();
            this.PortHex = BitConverter.ToString(this.PortBytes).Replace("-", string.Empty).ToLower();
            switch (this.PortHex)
            {
                case "10":
                    this.Channel = 2;
                    this.PortName = "D";
                    break;

                case "20":
                    this.Channel = 2;
                    this.PortName = "C";
                    break;

                case "40":
                    this.Channel = 1;
                    this.PortName = "B";
                    break;

                case "80":
                    this.Channel = 1;
                    this.PortName = "A";
                    break;
            }

            this.TimestampHex = BitConverter.ToString(this.TimestampBytes).Replace("-", string.Empty).ToLower();
            this.TimestampInNs = long.Parse(this.TimestampHex, System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>Gets the raw 20 byte data added by Kunbus TAP COURIOUS.</summary>
        public byte[] RawData { get; private set; }

        /// <summary>Gets the raw 20 byte data added by Kunbus TAP COURIOUS as hex string.</summary>
        public string RawHexData { get; private set; }

        /// <summary>Gets FCS as bytes.</summary>
        public byte[] FcsBytes { get; private set; } = new byte[4];

        /// <summary>Gets FCS as hex string.</summary>
        public string FcsHex { get; private set; }

        /// <summary>Gets identifier as bytes.</summary>
        public byte[] IdentifierBytes { get; private set; } = new byte[6];

        /// <summary>Gets identifier as hex string.</summary>
        public string IdentifierHex { get; private set; }

        /// <summary>Gets port as bytes.</summary>
        public byte[] PortBytes { get; private set; } = new byte[1];

        /// <summary>Gets port as hex string.</summary>
        public string PortHex { get; private set; }

        /// <summary>Gets port name where packet was captured.</summary>
        public string PortName { get; private set; } = string.Empty;

        /// <summary>Gets channel where packet was captured.</summary>
        public int Channel { get; private set; } = -1;

        /// <summary>Gets timestamp when packet was captured as bystes.</summary>
        public byte[] TimestampBytes { get; private set; } = new byte[8];

        /// <summary>Gets timestamp when packet was captured as hex string.</summary>
        public string TimestampHex { get; private set; }

        /// <summary>Gets timestamp when packet was captured in nanoseconds.</summary>
        public long TimestampInNs { get; private set; }

        /// <summary>Gets timestamp when packet was captured in milliseconds.</summary>
        public double TimestampInMs
        {
            get { return this.TimestampInNs / 1000000D; }
        }

        /// <summary>
        /// Converts the data to pretty string.
        /// </summary>
        /// <returns>Pretty string with data from KUNBUS TAP CURIOUS.</returns>
        public override string ToString()
        {
            return $"FCS: 0x{this.FcsHex}\nIdentifier: 0x{this.IdentifierHex}\nChannel: {this.Channel}\nPort: Con {this.PortName} (0x{this.PortHex})\nTimestamp (Hex): 0x{this.TimestampHex}\nTimestamp (ns): {this.TimestampInNs}";
        }
    }
}
