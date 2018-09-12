using System;
using F1Telemetry.Models.Raw.F12018;

namespace F1Telemetry
{
    public class F1Manager
    {
        private readonly TelemetryManager _telemetryManager;
        private DateTimeOffset _lastSent = DateTimeOffset.MinValue;

        public F1Manager(TelemetryManager telemetryManager)
        {
            _telemetryManager = telemetryManager;
            UpdateInterval = 500;
            _telemetryManager.CarStatusPacketReceived += _telemetryManager_CarStatusPacketReceived;
            _telemetryManager.CarTelemetryPacketReceived += _telemetryManager_CarTelemetryPacketReceived;
            _telemetryManager.LapPacketReceived += _telemetryManager_LapPacketReceived;
            _telemetryManager.SessionChanged += _telemetryManager_SessionChanged;
        }

        public event EventHandler<PacketReceivedEventArgs<PacketCarStatusData>> CarStatusReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketCarTelemetryData>> CarTelemetryReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketLapData>> LapPacketReceived;

        public event EventHandler<NewLapEventArgs> NewLap;

        public event EventHandler<EventArgs> SessionChanged;

        public bool CanSend
        {
            get
            {
                if (_lastSent == DateTimeOffset.MinValue)
                {
                    return true;
                }

                return (DateTimeOffset.Now - _lastSent).Milliseconds >= UpdateInterval;
            }
        }

        public int UpdateInterval { get; }

        public void Start()
        {
            _telemetryManager.Enable();
        }

        private void _telemetryManager_CarStatusPacketReceived(object sender, PacketReceivedEventArgs<PacketCarStatusData> e)
        {
            if (e.OldPacket.Equals(default(PacketCarStatusData)))
            {
                return;
            }

            OnCarStatusReceived(e.Packet, e.OldPacket);
        }

        private void _telemetryManager_CarTelemetryPacketReceived(object sender, PacketReceivedEventArgs<PacketCarTelemetryData> e)
        {
            if (e.OldPacket.Equals(default(PacketCarTelemetryData)))
            {
                return;
            }

            OnCarTelemetryReceived(e.Packet, e.OldPacket);
        }

        private void _telemetryManager_LapPacketReceived(object sender, PacketReceivedEventArgs<PacketLapData> e)
        {
            if (e.OldPacket.Equals(default(PacketLapData)))
            {
                return;
            }

            CheckLapChanged(e);
            OnLapPacketReceived(new PacketReceivedEventArgs<PacketLapData>(e.Packet, e.OldPacket));
        }

        private void _telemetryManager_SessionChanged(object sender, EventArgs e)
        {
            OnSessionChanged(e);
        }

        private void CheckLapChanged(PacketReceivedEventArgs<PacketLapData> e)
        {
            var oldLapNum = e.OldPacket.LapData[e.OldPacket.Header.PlayerCarIndex].CurrentLapNum;
            var currentLapNum = e.Packet.LapData[e.Packet.Header.PlayerCarIndex].CurrentLapNum;

            if (currentLapNum > oldLapNum)
            {
                OnNewLap(oldLapNum, currentLapNum);
            }
        }

        protected virtual void OnLapPacketReceived(PacketReceivedEventArgs<PacketLapData> e)
        {
            if (CanSend)
            {
                LapPacketReceived?.Invoke(this, e);
            }
        }

        private void OnCarStatusReceived(PacketCarStatusData oldCarStatusData, PacketCarStatusData newCarStatusData)
        {
            CarStatusReceived?.Invoke(this, new PacketReceivedEventArgs<PacketCarStatusData>(oldCarStatusData, newCarStatusData));
        }

        private void OnCarTelemetryReceived(PacketCarTelemetryData oldCarTelemetryData, PacketCarTelemetryData newCarTelemetryData)
        {
            if (CanSend)
            {
                CarTelemetryReceived?.Invoke(this, new PacketReceivedEventArgs<PacketCarTelemetryData>(oldCarTelemetryData, newCarTelemetryData));
                _lastSent = DateTimeOffset.Now;
            }
        }

        private void OnNewLap(int lastLap, int currentLap)
        {
            NewLap?.Invoke(this, new NewLapEventArgs(lastLap, currentLap));
        }

        private void OnSessionChanged(EventArgs e)
        {
            SessionChanged?.Invoke(this, e);
        }
    }
}
