﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using F1Telemetry.Events;
using F1Telemetry.Models.Raw.F12018;

namespace F1Telemetry.Manager
{
    public sealed class TelemetryManager : IDisposable
    {
        private readonly int _port;
        private readonly TelemetryRecorder _telemetryRecorder;
        private Thread _captureThread;
        private bool _disposed;

        private PacketCarSetupData _oldCarSetupData;
        private PacketCarStatusData _oldCarStatusData;
        private PacketCarTelemetryData _oldCarTelemetryData;
        private EventPacket _oldEventPacket;
        private uint _oldFrameIdentifier;
        private PacketLapData _oldLapData;
        private PacketMotionData _oldMotionData;
        private PacketParticipantsData _oldParticipantsData;
        private PacketSessionData _oldSessionData;
        private ulong _oldSessionId;
        private IPEndPoint _senderIp = new IPEndPoint(IPAddress.Any, 0);
        private UdpClient _udpClient;

        public TelemetryManager(TelemetryRecorder telemetryRecorder, int port = 20777)
        {
            _telemetryRecorder = telemetryRecorder;
            _port = port;
            InitUdp(port);
            Enable();

            //_telemetryRecorder.Start();
        }

        public event EventHandler<PacketReceivedEventArgs<PacketCarSetupData>> CarSetupPacketReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketCarStatusData>> CarStatusPacketReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketCarTelemetryData>> CarTelemetryPacketReceived;

        public event EventHandler<PacketReceivedEventArgs<EventPacket>> EventPacketReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketLapData>> LapPacketReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketMotionData>> MotionPacketReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketParticipantsData>> ParticipantsPacketReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketSessionData>> SessionChanged;

        public event EventHandler<PacketReceivedEventArgs<PacketSessionData>> SessionPacketReceived;

        public void Dispose()
        {
            Dispose(true);
        }

        public void Enable()
        {
            if (_captureThread == null)
            {
                _captureThread = new Thread(new ThreadStart(UdpLoop))
                {
                    IsBackground = true
                };
                _captureThread.Start();
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _disposed = true;
                if (_captureThread != null)
                {
                    _captureThread.Abort();
                    _captureThread = null;
                }
                if (_udpClient != null)
                {
                    try
                    {
                        _udpClient.Close();
                    }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                    catch (Exception)
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                    {
                        // can be ignored
                    }
                    _captureThread = null;
                }
            }
            _disposed = true;
        }

        private void HandlePacket(PacketHeader packet, byte[] bytes)
        {
            _telemetryRecorder.RecordPacket(packet, bytes);

            switch (packet.PacketType)
            {
                case PacketType.Motion:
                    var motionPacket = StructUtility.ConvertToPacket<PacketMotionData>(bytes);
                    OnMotionPacketReceived(new PacketReceivedEventArgs<PacketMotionData>(_oldMotionData, motionPacket));
                    _oldMotionData = motionPacket;
                    break;

                case PacketType.Session:
                    var sessionPacket = StructUtility.ConvertToPacket<PacketSessionData>(bytes);
                    OnSessionPacketReceived(new PacketReceivedEventArgs<PacketSessionData>(_oldSessionData, sessionPacket));
                    _oldSessionData = sessionPacket;
                    break;

                case PacketType.LapData:
                    var lapPacket = StructUtility.ConvertToPacket<PacketLapData>(bytes);
                    OnLapPacketReceivedReceived(new PacketReceivedEventArgs<PacketLapData>(_oldLapData, lapPacket));
                    _oldLapData = lapPacket;
                    break;

                case PacketType.Event:
                    var eventPacket = StructUtility.ConvertToPacket<EventPacket>(bytes);
                    OnEventPacketReceived(new PacketReceivedEventArgs<EventPacket>(_oldEventPacket, eventPacket));
                    _oldEventPacket = eventPacket;
                    break;

                case PacketType.Participants:
                    var participantPacket = StructUtility.ConvertToPacket<PacketParticipantsData>(bytes);
                    OnParticipantsPacketReceived(new PacketReceivedEventArgs<PacketParticipantsData>(_oldParticipantsData, participantPacket));
                    _oldParticipantsData = participantPacket;
                    break;

                case PacketType.CarSetups:
                    var carSetupsPacket = StructUtility.ConvertToPacket<PacketCarSetupData>(bytes);
                    OnCarSetupPacketReceived(new PacketReceivedEventArgs<PacketCarSetupData>(_oldCarSetupData, carSetupsPacket));
                    _oldCarSetupData = carSetupsPacket;
                    break;

                case PacketType.CarTelemetry:
                    var carTelemetryPacket = StructUtility.ConvertToPacket<PacketCarTelemetryData>(bytes);
                    OnCarTelemetryPacketReceived(new PacketReceivedEventArgs<PacketCarTelemetryData>(_oldCarTelemetryData, carTelemetryPacket));
                    _oldCarTelemetryData = carTelemetryPacket;
                    break;

                case PacketType.CarStatus:
                    var carStatusPacket = StructUtility.ConvertToPacket<PacketCarStatusData>(bytes);
                    OnCarStatusPacketReceived(new PacketReceivedEventArgs<PacketCarStatusData>(_oldCarStatusData, carStatusPacket));
                    _oldCarStatusData = carStatusPacket;
                    break;
            }

            _oldSessionId = packet.SessionUId;
            _oldFrameIdentifier = packet.FrameIdentifier;
        }

        private void InitUdp(int port)
        {
            try
            {
                if (_udpClient == null && !_disposed)
                {
                    _udpClient = new UdpClient();
                    _udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _udpClient.ExclusiveAddressUse = false;
                    var localEP = new IPEndPoint(IPAddress.Any, port);
                    _udpClient.Client.Bind(localEP);
                }
            }
            catch
            {
                _udpClient = null;
            }
        }

        private void OnCarSetupPacketReceived(PacketReceivedEventArgs<PacketCarSetupData> e)
        {
            CarSetupPacketReceived?.Invoke(this, e);
        }

        private void OnCarStatusPacketReceived(PacketReceivedEventArgs<PacketCarStatusData> e)
        {
            CarStatusPacketReceived?.Invoke(this, e);
        }

        private void OnCarTelemetryPacketReceived(PacketReceivedEventArgs<PacketCarTelemetryData> e)
        {
            CarTelemetryPacketReceived?.Invoke(this, e);
        }

        private void OnEventPacketReceived(PacketReceivedEventArgs<EventPacket> e)
        {
            EventPacketReceived?.Invoke(this, e);
        }

        private void OnLapPacketReceivedReceived(PacketReceivedEventArgs<PacketLapData> e)
        {
            LapPacketReceived?.Invoke(this, e);
        }

        private void OnMotionPacketReceived(PacketReceivedEventArgs<PacketMotionData> e)
        {
            MotionPacketReceived?.Invoke(this, e);
        }

        private void OnParticipantsPacketReceived(PacketReceivedEventArgs<PacketParticipantsData> e)
        {
            ParticipantsPacketReceived?.Invoke(this, e);
        }

        private void OnSessionChanged(PacketReceivedEventArgs<PacketSessionData> e)
        {
            SessionChanged?.Invoke(this, e);
        }

        private void OnSessionPacketReceived(PacketReceivedEventArgs<PacketSessionData> e)
        {
            if ((_oldSessionId != 0 && e.Packet.PacketHeader.SessionUId != _oldSessionId) ||
                e.Packet.PacketHeader.FrameIdentifier < _oldFrameIdentifier)
            {
                OnSessionChanged(e);
            }

            SessionPacketReceived?.Invoke(this, e);
        }

        private void UdpLoop()
        {
            while (true)
            {
                InitUdp(_port);
                try
                {
                    _udpClient.Client.ReceiveTimeout = 5000;
                    byte[] receiveBytes = _udpClient.Receive(ref _senderIp);
                    var packet = StructUtility.ConvertToPacket<PacketHeader>(receiveBytes);
                    HandlePacket(packet, receiveBytes);
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch (Exception)
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                    // can be ignored (timeout etc)
                }
            }
        }
    }
}
