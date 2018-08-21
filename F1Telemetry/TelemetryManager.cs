using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using F1Telemetry.Models;
using F1Telemetry.Models.Raw;

namespace F1Telemetry
{
    public class TelemetryManager
    {
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        public F12017TelemetryPacket OldPacket { get; set; }
        public F12017TelemetryPacket NewPacket { get; set; }

        private UdpClient _udpClient;
        private IPEndPoint _senderIp = new IPEndPoint(IPAddress.Any, 0);
        private Thread _captureThread;
        private bool _disposed;
        private int _port;

        public TelemetryManager(int port = 20777)
        {
            _port = port;
            InitUdp(port);
            Enable();
        }

        public void Enable()
        {
            if (this._captureThread == null)
            {
                this._captureThread = new Thread(new ThreadStart(this.UdpLoop));
                this._captureThread.IsBackground = true;
                this._captureThread.Start();
            }
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
                    catch
                    {
                    }
                    _captureThread = null;
                }
            }
            _disposed = true;
        }

        private void UdpLoop()
        {
            while (true)
            {
                InitUdp(_port);
                byte[] receiveBytes = _udpClient.Receive(ref _senderIp);
                var packet = ConvertToPacket<F12017TelemetryPacket>(receiveBytes);
                HandlePacket(packet);
            }
        }

        public static T ConvertToPacket<T>(byte[] bytes)
        {
            GCHandle gchandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T result = (T)Marshal.PtrToStructure(gchandle.AddrOfPinnedObject(), typeof(T));
            gchandle.Free();
            return result;
        }

        private void HandlePacket(F12017TelemetryPacket packet)
        {
            NewPacket = packet;

            var args = new PacketReceivedEventArgs
            {
                NewPacket = new F12017DisplayModel(NewPacket),
                OldPacket = new F12017DisplayModel(OldPacket)
            };

            OnPacketReceived(args);

            OldPacket = packet;
        }

        protected virtual void OnPacketReceived(PacketReceivedEventArgs e)
        {
            PacketReceived?.Invoke(this, e);
        }
    }

    public class PacketReceivedEventArgs : EventArgs
    {
        public F12017DisplayModel OldPacket { get; set; }
        public F12017DisplayModel NewPacket { get; set; }
    }
}
