using System;
using F1Telemetry.Models.Raw.F12018;

namespace F1Telemetry
{
    public class F1Manager
    {
        private readonly TelemetryManager _telemetryManager;

        private DateTimeOffset _dataLastReceived;

        private DateTimeOffset _dataLastSend = DateTimeOffset.MinValue;

        public F1Manager(TelemetryManager telemetryManager)
        {
            _telemetryManager = telemetryManager;
            UpdateInterval = 60;
            _telemetryManager.CarSetupPacketReceived += _telemetryManager_CarSetupPacketReceived;
            _telemetryManager.CarStatusPacketReceived += _telemetryManager_CarStatusPacketReceived;
            _telemetryManager.CarTelemetryPacketReceived += _telemetryManager_CarTelemetryPacketReceived;
            _telemetryManager.EventPacketReceived += _telemetryManager_EventPacketReceived;
            _telemetryManager.LapPacketReceived += _telemetryManager_LapPacketReceived;
            _telemetryManager.ParticipantsPacketReceived += _telemetryManager_ParticipantsPacketReceived;
            _telemetryManager.SessionPacketReceived += _telemetryManager_SessionPacketReceived;
            _telemetryManager.SessionChanged += _telemetryManager_SessionChanged;
        }

        public event EventHandler<PacketReceivedEventArgs<CarStatusData>> CarStatusReceived;

        public event EventHandler<PacketReceivedEventArgs<CarTelemetryData>> CarTelemetryReceived;

        public event EventHandler<PacketReceivedEventArgs<LapData>> LapPacketReceived;

        public event EventHandler<NewLapEventArgs> NewLap;

        public event EventHandler<EventArgs> SessionChanged;

        public bool CanSend
        {
            get
            {
                if (_dataLastSend == DateTimeOffset.MinValue)
                {
                    return true;
                }

                return (DateTimeOffset.Now - _dataLastSend).Ticks >= UpdateInterval;
            }
        }

        public bool IsRunning
        {
            get
            {
                return (DateTimeOffset.Now - _dataLastReceived).TotalSeconds > 5;
            }
        }

        public int UpdateInterval { get; }

        public void Start()
        {
            _telemetryManager.Enable();
        }

        protected virtual void OnLapPacketReceived(PacketReceivedEventArgs<LapData> e)
        {
            LapPacketReceived?.Invoke(this, e);
        }

        private void _telemetryManager_CarSetupPacketReceived(object sender, PacketReceivedEventArgs<PacketCarSetupData> e)
        {
            _dataLastReceived = DateTimeOffset.Now;
        }

        private void _telemetryManager_CarStatusPacketReceived(object sender, PacketReceivedEventArgs<PacketCarStatusData> e)
        {
            if (e.OldPacket.Equals(default(PacketCarStatusData)))
            {
                return;
            }

            _dataLastReceived = DateTimeOffset.Now;

            var oldCarStatusData = e.Packet.CarStatusData[e.Packet.Header.PlayerCarIndex];
            var carStatusData = e.OldPacket.CarStatusData[e.Packet.Header.PlayerCarIndex];

            OnCarStatusReceived(oldCarStatusData, carStatusData);
        }

        private void _telemetryManager_CarTelemetryPacketReceived(object sender, PacketReceivedEventArgs<PacketCarTelemetryData> e)
        {
            if (e.OldPacket.Equals(default(PacketCarTelemetryData)))
            {
                return;
            }

            _dataLastReceived = DateTimeOffset.Now;

            var playerCarTelemetry = e.Packet.CarTelemetryData[e.Packet.Header.PlayerCarIndex];
            var oldPlayerCarTelemetry = e.OldPacket.CarTelemetryData[e.Packet.Header.PlayerCarIndex];

            OnCarTelemetryReceived(oldPlayerCarTelemetry, playerCarTelemetry);
        }

        private void _telemetryManager_EventPacketReceived(object sender, PacketReceivedEventArgs<EventPacket> e)
        {
            _dataLastReceived = DateTimeOffset.Now;
        }

        private void _telemetryManager_LapPacketReceived(object sender, PacketReceivedEventArgs<PacketLapData> e)
        {
            if (e.OldPacket.Equals(default(PacketLapData)))
            {
                return;
            }

            _dataLastReceived = DateTimeOffset.Now;
            var playerLapData = e.Packet.LapData[e.Packet.Header.PlayerCarIndex];
            var oldPlayerLapData = e.OldPacket.LapData[e.Packet.Header.PlayerCarIndex];

            CheckLapChanged(e);
            OnLapPacketReceived(new PacketReceivedEventArgs<LapData>(oldPlayerLapData, playerLapData));
        }

        private void _telemetryManager_ParticipantsPacketReceived(object sender, PacketReceivedEventArgs<PacketParticipantsData> e)
        {
            _dataLastReceived = DateTimeOffset.Now;
        }

        private void _telemetryManager_SessionChanged(object sender, EventArgs e)
        {
            OnSessionChanged(e);
        }

        private void _telemetryManager_SessionPacketReceived(object sender, PacketReceivedEventArgs<PacketSessionData> e)
        {
            _dataLastReceived = DateTimeOffset.Now;
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

        private void OnCarStatusReceived(CarStatusData oldCarStatusData, CarStatusData newCarStatusData)
        {
            if (oldCarStatusData.Equals(newCarStatusData))
            {
                return;
            }

            CarStatusReceived?.Invoke(this, new PacketReceivedEventArgs<CarStatusData>(oldCarStatusData, newCarStatusData));
        }

        private void OnCarTelemetryReceived(CarTelemetryData oldCarTelemetryData, CarTelemetryData newCarTelemetryData)
        {
            if (oldCarTelemetryData.Equals(newCarTelemetryData))
            {
                return;
            }

            CarTelemetryReceived?.Invoke(this, new PacketReceivedEventArgs<CarTelemetryData>(oldCarTelemetryData, newCarTelemetryData));
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
