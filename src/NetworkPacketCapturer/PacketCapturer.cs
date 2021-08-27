// <copyright file="PacketCapturer.cs" company="Fraunhofer Institute for Manufacturing Engineering and Automation IPA">
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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using NetworkPacketCapturer.KunbusTap;
    using PacketDotNet;
    using Serilog;
    using SharpPcap;
    using SharpPcap.LibPcap;

    /// <summary>
    /// Abstract packet capturer as basis for packet capturers for different measurment tools.
    /// </summary>
    public abstract class PacketCapturer
    {
        private bool capturing = false;

        private Thread packetAnalyzerThread;

        private object packetQueueLock = new object();

        private Queue<RawCapture> packetQueue = new Queue<RawCapture>();

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketCapturer"/> class.
        /// </summary>
        /// <param name="networkInterface">Network interface that should be used for packet capturing.</param>
        public PacketCapturer(ICaptureDevice networkInterface)
        {
            this.CapturingNetworkInterface = networkInterface;

            this.CapturingNetworkInterface.Open(DeviceMode.Promiscuous, -1);
            this.CapturingNetworkInterface.OnPacketArrival += this.OnPacketArrival;
            this.CapturingNetworkInterface.StartCapture();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PacketCapturer"/> class.
        /// </summary>
        /// <param name="networkInterface">Network interface that should be used for packet capturing.</param>
        /// <param name="kunbusTapConfig">Configuration for Kunbus TAP CURIOUS.</param>
        public PacketCapturer(ICaptureDevice networkInterface, KunbusTapConfiguration kunbusTapConfig)
            : this(networkInterface)
        {
            this.KunbusTapConfiguration = kunbusTapConfig;
        }

        /// <summary>Called when a new packet was captured.</summary>
        public event EventHandler<CapturedPacketArrivedEventArgs> PacketArrived;

        /// <summary>Gets the network interface used for packet capturing.</summary>
        public ICaptureDevice CapturingNetworkInterface { get; private set; }

        /// <summary>Gets the IP address of the client. </summary>
        public IPAddress ClientIp { get; private set; }

        /// <summary>Gets the IP address of the server. </summary>
        public IPAddress ServerIp { get; private set; }

        /// <summary>Gets the destination port of packets (usually port of test application on server).</summary>
        public ushort DestinationPort { get; private set; }

        /// <summary>Gets the size of packets in bytes.</summary>
        public int MessageSize { get; private set; }

        /// <summary>Gets the Kunbus TAP CURIOUS port where the server is connected.</summary>
        public string TapPortServer { get; private set; }

        /// <summary>Gets the Kunbus TAP CURIOUS port where the client is connected.</summary>
        public string TapPortClient { get; private set; }

        /// <summary>Gets a value indicating whether packet have additional 20 bytes added from Kunbus TAP CURIOUS.</summary>
        public bool TapAdditional20ByteEnabled { get; private set; } = true;

        /// <summary>Gets the configuration for Kunbus TAP CURIOUS.</summary>
        public KunbusTapConfiguration KunbusTapConfiguration { get; private set; }

        /// <summary>Gets the packet size of ping packets in bytes.</summary>
        protected abstract int PingPacketSize { get; }

        /// <summary>Gets the packet size of pong packets in bytes.</summary>
        protected abstract int PongPacketSize { get; }

        /// <summary>Gets the captured packets.</summary>
        protected Dictionary<long, List<CapturedPacket>> CapturedPackets { get; private set; } = new Dictionary<long, List<CapturedPacket>>();

        /// <summary>
        /// Gets the network interface by name.
        /// </summary>
        /// <param name="interfaceName">Name of the network interface.</param>
        /// <returns>Network interface matching to given name.</returns>
        public static ICaptureDevice GetNetworkInterfaceFromName(string interfaceName)
        {
            var devices = CaptureDeviceList.Instance;
            for (int i = 0; i < devices.Count; i++)
            {
                LibPcapLiveDevice currentDevice = (LibPcapLiveDevice)devices[i];
                if (currentDevice.Interface != null)
                {
                    if (currentDevice.Interface.Name == interfaceName)
                    {
                        return devices[i];
                    }
                }
            }

            PacketCapturer.LogAvailableNetworkInterfaces();
            throw new ArgumentException($"Unkown interface with name '{interfaceName}'");
        }

        /// <summary>
        /// Gets the network interface by name.
        /// </summary>
        /// <param name="interfaceFriendlyName">Friendly name of the network interface.</param>
        /// <returns>Network interface matching to given friendly name.</returns>
        public static ICaptureDevice GetNetworkInterfaceFromFriendlyName(string interfaceFriendlyName)
        {
            var devices = CaptureDeviceList.Instance;
            for (int i = 0; i < devices.Count; i++)
            {
                LibPcapLiveDevice currentDevice = (LibPcapLiveDevice)devices[i];
                if (currentDevice.Interface != null)
                {
                    if (currentDevice.Interface.FriendlyName == interfaceFriendlyName)
                    {
                        return devices[i];
                    }
                }
            }

            PacketCapturer.LogAvailableNetworkInterfaces();
            throw new ArgumentException($"Unkown interface with friendly name '{interfaceFriendlyName}'");
        }

        /// <summary>
        /// Logs the available network interfaces.
        /// </summary>
        public static void LogAvailableNetworkInterfaces()
        {
            var devices = CaptureDeviceList.Instance;
            Log.Information("Available network interfaces:");
            for (int i = 0; i < devices.Count; i++)
            {
                LibPcapLiveDevice currentDevice = (LibPcapLiveDevice)devices[i];
                Log.Information($"Friendly Name: {currentDevice.Interface.FriendlyName}");
                Log.Information($"Name: {currentDevice.Interface.Name}");
                Log.Information(string.Empty);
            }
        }

        /// <summary>
        /// Starts the packet capturing until stop is called.
        /// </summary>
        /// <param name="clientIp">IP address of the client sending or receiving the packets which should be captured.</param>
        /// <param name="serverIp">IP address of the server sending or receiving the packets which should be captured.</param>
        /// <param name="destinationPort">Destination port of the packets on server side.</param>
        /// <param name="messageSize">Size of the sent packets in bytes.</param>
        public void StartCapturing(IPAddress clientIp, IPAddress serverIp, ushort destinationPort, int messageSize)
        {
            this.ClientIp = clientIp;
            this.ServerIp = serverIp;
            this.DestinationPort = destinationPort;
            this.MessageSize = messageSize;

            this.CapturedPackets.Clear();

            this.capturing = true;
            this.packetAnalyzerThread = new Thread(this.ProcessPackets);
            this.packetAnalyzerThread.Priority = ThreadPriority.Highest;
            this.packetAnalyzerThread.Start();
        }

        /// <summary>
        /// Asynchronous stops the packet capturing.
        /// </summary>
        /// <param name="timeout">Timeout to wait for packets before capturing is stopped.</param>
        /// <returns>List of captured packets since <see cref="PacketCapturer.StartCapturing(IPAddress, IPAddress, ushort, int)"/> was called.</returns>
        public async Task<Dictionary<long, List<CapturedPacket>>> StopCapturingAsync(TimeSpan timeout)
        {
            this.capturing = false;

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (this.packetQueue.Count > 0 && (stopwatch.ElapsedMilliseconds < timeout.TotalMilliseconds))
            {
                await Task.Delay(100);
            }

            if (stopwatch.ElapsedMilliseconds > timeout.TotalMilliseconds)
            {
                Log.Error($"Aborted packet processing after '{timeout.Milliseconds}', return results so far");
            }
            else
            {
                Log.Information("Packet analysis completed");
            }

            return this.CapturedPackets;
        }

        /// <summary>
        /// Stops the packet capturing.
        /// </summary>
        /// <param name="timeout">Timeout to wait for packets before capturing is stopped.</param>
        /// <returns>List of captured packets since <see cref="PacketCapturer.StartCapturing(IPAddress, IPAddress, ushort, int)"/> was called.</returns>
        public Dictionary<long, List<CapturedPacket>> StopCapturing(TimeSpan timeout)
        {
            return this.StopCapturingAsync(timeout).Result;
        }

        /// <summary>
        /// Process captured packets.
        /// </summary>
        protected virtual void ProcessPackets()
        {
            while (this.capturing)
            {
                var capturedPacketsQueue = this.packetQueue;
                lock (this.packetQueueLock)
                {
                    this.packetQueue = new Queue<RawCapture>();
                }

                while (capturedPacketsQueue.Count > 0)
                {
                    var rawCapture = capturedPacketsQueue.Dequeue();
                    var packet = PacketDotNet.Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

                    if (packet.PayloadPacket is IPv4Packet)
                    {
                        var ipv4Packet = (IPv4Packet)packet.PayloadPacket;

                        // Packet from client to server...
                        if (ipv4Packet.DestinationAddress.Equals(this.ServerIp))
                        {
                            // TCP or UDP protocol
                            if (ipv4Packet.PayloadPacket is TcpPacket || ipv4Packet.PayloadPacket is UdpPacket)
                            {
                                var transportPacket = (TransportPacket)ipv4Packet.PayloadPacket;

                                // ... is a Sockperf packet if destination port matches Sockperf server port and ...
                                if (transportPacket.DestinationPort.Equals(this.DestinationPort))
                                {
                                    var packetPayload = transportPacket.PayloadData;
                                    if (packetPayload != null)
                                    {
                                        // ... payload length is greater 0 --> No ACK package
                                        if (packetPayload.Length == this.PingPacketSize)
                                        {
                                            var capturedPacket = new CapturedPacket(
                                                CapturedPacketDirection.PING,
                                                this.GetPacketIndex(packetPayload),
                                                packetPayload.Length,
                                                ipv4Packet.SourceAddress,
                                                transportPacket.SourcePort,
                                                ipv4Packet.DestinationAddress,
                                                transportPacket.DestinationPort,
                                                rawCapture.Timeval);

                                            if (this.KunbusTapConfiguration != null)
                                            {
                                                var kunbusTapData = this.ParseKunbusTapData(rawCapture.Data);
                                                capturedPacket.KunbusTapData = kunbusTapData;
                                            }

                                            this.AddCapturedPacket(capturedPacket);
                                            this.PacketArrived?.Invoke(this, new CapturedPacketArrivedEventArgs(capturedPacket));
                                        }
                                    }
                                }
                            }
                        }

                        // Packet from server to client
                        else if (ipv4Packet.SourceAddress.Equals(this.ServerIp))
                        {
                            if (ipv4Packet.PayloadPacket is TcpPacket || ipv4Packet.PayloadPacket is UdpPacket)
                            {
                                var transportPacket = (TransportPacket)ipv4Packet.PayloadPacket;

                                // ... is a Sockperf packet if source port matches Sockperf server port and ...
                                if (transportPacket.SourcePort.Equals(this.DestinationPort))
                                {
                                    try
                                    {
                                        var packetPayload = transportPacket.PayloadData;
                                        if (packetPayload != null)
                                        {
                                            // ... payload length is greater 0 --> No ACK package
                                            if (packetPayload.Length == this.PongPacketSize)
                                            {
                                                var capturedPacket = new CapturedPacket(
                                                    CapturedPacketDirection.PONG,
                                                    this.GetPacketIndex(packetPayload),
                                                    packetPayload.Length,
                                                    ipv4Packet.SourceAddress,
                                                    transportPacket.SourcePort,
                                                    ipv4Packet.DestinationAddress,
                                                    transportPacket.DestinationPort,
                                                    rawCapture.Timeval);

                                                if (this.KunbusTapConfiguration != null)
                                                {
                                                    var kunbusTapData = this.ParseKunbusTapData(rawCapture.Data);
                                                    capturedPacket.KunbusTapData = kunbusTapData;
                                                }

                                                this.AddCapturedPacket(capturedPacket);
                                                this.PacketArrived?.Invoke(this, new CapturedPacketArrivedEventArgs(capturedPacket));
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Log.Error($"{e.Message}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Called when new packet was captured.
        /// </summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="eventArgs">Event arguments.</param>
        protected void OnPacketArrival(object sender, CaptureEventArgs eventArgs)
        {
            if (this.capturing)
            {
                lock (this.packetQueueLock)
                {
                    this.packetQueue.Enqueue(eventArgs.Packet);
                }
            }
        }

        /// <summary>
        /// Parse <see cref="KunbusTapData"/> from packet data.
        /// </summary>
        /// <param name="packetData">The 20 bytes of the packet containg the additional information added by KUNBUS TAP CURIOUS.</param>
        /// <returns>The parsed data.</returns>
        protected KunbusTapData ParseKunbusTapData(byte[] packetData)
        {
            var tapRawData = new byte[20];
            Array.Copy(packetData, packetData.Length - 20, tapRawData, 0, 20);
            var kunbusTapData = new KunbusTapData(tapRawData);

            return kunbusTapData;
        }

        /// <summary>
        /// Add captured packet to store.
        /// </summary>
        /// <param name="capturedPacket">Captured packet to be added.</param>
        protected void AddCapturedPacket(CapturedPacket capturedPacket)
        {
            if (this.CapturedPackets.ContainsKey(capturedPacket.Index))
            {
                this.CapturedPackets[capturedPacket.Index].Add(capturedPacket);
            }
            else
            {
                var capturedPacketsForIndex = new List<CapturedPacket>();
                capturedPacketsForIndex.Add(capturedPacket);
                this.CapturedPackets[capturedPacket.Index] = capturedPacketsForIndex;
            }
        }

        /// <summary>
        /// Get index of the packet.
        /// </summary>
        /// <param name="packetPayload">Packet payload containg packet index.</param>
        /// <returns>The packet index.</returns>
        protected abstract long GetPacketIndex(byte[] packetPayload);
    }
}
