using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using F1Telemetry.Models.Raw.F12018;

namespace F1Telemetry
{
    public class PacketReceivedEventArgs<T> : EventArgs
    {
        public PacketReceivedEventArgs(T oldPacket, T packet)
        {
            OldPacket = oldPacket;
            Packet = packet;
        }

        public T OldPacket { get; set; }
        public T Packet { get; set; }
    }

    public class TelemetryManager : IDisposable
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
        private UInt64 _oldSessionId;
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

        public event EventHandler<EventArgs> SessionChanged;

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

        protected virtual void Dispose(bool disposing)
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

        protected virtual void OnCarSetupPacketReceived(PacketReceivedEventArgs<PacketCarSetupData> e)
        {
            CarSetupPacketReceived?.Invoke(this, e);
        }

        protected virtual void OnCarStatusPacketReceived(PacketReceivedEventArgs<PacketCarStatusData> e)
        {
            CarStatusPacketReceived?.Invoke(this, e);
        }

        protected virtual void OnCarTelemetryPacketReceived(PacketReceivedEventArgs<PacketCarTelemetryData> e)
        {
            CarTelemetryPacketReceived?.Invoke(this, e);
        }

        protected virtual void OnEventPacketReceived(PacketReceivedEventArgs<EventPacket> e)
        {
            EventPacketReceived?.Invoke(this, e);
        }

        protected virtual void OnLapPacketReceivedReceived(PacketReceivedEventArgs<PacketLapData> e)
        {
            LapPacketReceived?.Invoke(this, e);
        }

        protected virtual void OnMotionPacketReceived(PacketReceivedEventArgs<PacketMotionData> e)
        {
            MotionPacketReceived?.Invoke(this, e);
        }

        protected virtual void OnParticipantsPacketReceived(PacketReceivedEventArgs<PacketCarSetupData> e)
        {
            CarSetupPacketReceived?.Invoke(this, e);
        }

        protected virtual void OnParticipantsPacketReceived(PacketReceivedEventArgs<PacketParticipantsData> e)
        {
            ParticipantsPacketReceived?.Invoke(this, e);
        }

        protected virtual void OnSessionChanged(EventArgs e)
        {
            SessionChanged?.Invoke(this, e);
        }

        protected virtual void OnSessionPacketReceived(PacketReceivedEventArgs<PacketSessionData> e)
        {
            SessionPacketReceived?.Invoke(this, e);
        }

        private void HandlePacket(PacketHeader packet, byte[] bytes)
        {
            Debug.WriteLine($"Received {packet.PacketType}");

            _telemetryRecorder.RecordPacket(packet, bytes);

            if ((_oldSessionId != 0 && packet.SessionUId != _oldSessionId) || packet.FrameIdentifier < _oldFrameIdentifier)
            {
                OnSessionChanged(new EventArgs());
            }

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
                    IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
                    _udpClient.Client.Bind(localEP);
                }
            }
            catch
            {
                _udpClient = null;
            }
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
