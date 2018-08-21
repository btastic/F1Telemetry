using System;

namespace F1Telemetry
{
    public class F1Manager
    {
        public event EventHandler<NewLapEventArgs> NewLap;
        public event EventHandler<PacketReceivedEventArgs> PacketReceived;

        private readonly TelemetryManager _telemetryManager;
        private DateTimeOffset _dataLastReceived;
        private DateTimeOffset _dataLastSend = DateTimeOffset.MinValue;

        public bool IsRunning
        {
            get
            {
                return (DateTimeOffset.Now - _dataLastReceived).TotalSeconds > 5;
            }
        }

        public bool CanSend
        {
            get
            {
                if (_dataLastSend == DateTimeOffset.MinValue)
                {
                    return true;
                }

                return (DateTimeOffset.Now - _dataLastSend).TotalMilliseconds >= UpdateInterval;
            }
        }

        public int UpdateInterval { get; }

        public F1Manager(TelemetryManager telemetryManager, int updateInterval)
        {
            _telemetryManager = telemetryManager;
            UpdateInterval = updateInterval;
            _telemetryManager.PacketReceived += _telemetryManager_PacketReceived;
        }

        private void _telemetryManager_PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            _dataLastReceived = DateTimeOffset.Now;
            HandlePacket(e);
        }

        private void HandlePacket(PacketReceivedEventArgs e)
        {
            CheckLapChanged(e);
            if (CanSend)
            {
                OnPacketReceived(e);
                _dataLastSend = DateTimeOffset.Now;
            }
        }

        private void CheckLapChanged(PacketReceivedEventArgs e)
        {
            if (e.OldPacket != null && e.OldPacket.LapTime > e.NewPacket.LapTime)
            {
                OnNewLap(e.OldPacket.CurrentLap, e.NewPacket.CurrentLap);
            }
        }

        private void OnNewLap(int lastLap, int currentLap)
        {
            NewLap?.Invoke(this, new NewLapEventArgs(lastLap, currentLap));
        }

        private void OnPacketReceived(PacketReceivedEventArgs packetReceivedEventArgs)
        {
            PacketReceived?.Invoke(this, packetReceivedEventArgs);
        }

        public void Start()
        {
            _telemetryManager.Enable();
        }
    }
}
