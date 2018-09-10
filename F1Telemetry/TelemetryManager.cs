using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using F1Telemetry.Models;
using F1Telemetry.Models.Raw.F12018;

namespace F1Telemetry
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public F12017DisplayModel NewPacket { get; set; }
        public F12017DisplayModel OldPacket { get; set; }
    }

    public class TelemetryManager : IDisposable
    {
        private readonly int _port;

        private Thread _captureThread;

        private bool _disposed;

        private IPEndPoint _senderIp = new IPEndPoint(IPAddress.Any, 0);

        private UdpClient _udpClient;

        public TelemetryManager(int port = 20777)
        {
            _port = port;
            InitUdp(port);
            Enable();
        }

        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        public PacketHeader NewPacket { get; set; }
        public PacketHeader OldPacket { get; set; }

        public static T ConvertToPacket<T>(byte[] bytes)
        {
            GCHandle gchandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T result = (T)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(T));
            gchandle.Free();
            return result;
        }

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

        protected virtual void OnPacketReceived(PacketReceivedEventArgs e)
        {
            PacketReceived?.Invoke(this, e);
        }

        private void HandlePacket(PacketHeader packet, byte[] bytes)
        {
            NewPacket = packet;

            switch (packet.PacketType)
            {
                case PacketType.Motion:
                    var motionPacket = ConvertToPacket<PacketMotionData>(bytes);
                    break;

                case PacketType.Session:
                    var sessionPacket = ConvertToPacket<PacketSessionData>(bytes);
                    break;

                case PacketType.LapData:
                    var lapPacket = ConvertToPacket<PacketLapData>(bytes);
                    break;

                case PacketType.Event:
                    var eventPacket = ConvertToPacket<EventPacket>(bytes);
                    break;

                case PacketType.Participants:
                    var participantPacket = ConvertToPacket<PacketParticipantsData>(bytes);
                    break;

                case PacketType.CarSetups:
                    var carSetupPacket = ConvertToPacket<PacketCarSetupData>(bytes);
                    break;

                case PacketType.CarTelemetry:
                    var carTelemetryPacket = ConvertToPacket<PacketCarTelemetryData>(bytes);
                    break;

                case PacketType.CarStatus:
                    var carStatusPacket = ConvertToPacket<PacketCarStatusData>(bytes);
                    break;
            }

            //var args = new PacketReceivedEventArgs
            //{
            //    NewPacket = new F12017DisplayModel(NewPacket),
            //    OldPacket = new F12017DisplayModel(OldPacket)
            //};

            //OnPacketReceived(args);

            OldPacket = packet;
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
                    var packet = ConvertToPacket<PacketHeader>(receiveBytes);
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
