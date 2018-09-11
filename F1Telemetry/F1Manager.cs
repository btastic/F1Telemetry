using System;
using F1Telemetry.Models.Raw.F12018;

namespace F1Telemetry
{
    public class F1Manager
    {
        private readonly TelemetryManager _telemetryManager;

        private DateTimeOffset _dataLastReceived;

        private readonly DateTimeOffset _dataLastSend = DateTimeOffset.MinValue;

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

        public event EventHandler<PacketReceivedEventArgs<PacketCarStatusData>> CarStatusReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketCarTelemetryData>> CarTelemetryReceived;

        public event EventHandler<PacketReceivedEventArgs<PacketLapData>> LapPacketReceived;

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

        protected virtual void OnLapPacketReceived(PacketReceivedEventArgs<PacketLapData> e)
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
                    
            OnCarStatusReceived(e.Packet, e.OldPacket);
        }

        private void _telemetryManager_CarTelemetryPacketReceived(object sender, PacketReceivedEventArgs<PacketCarTelemetryData> e)
        {
            if (e.OldPacket.Equals(default(PacketCarTelemetryData)))
            {
                return;
            }

            _dataLastReceived = DateTimeOffset.Now;

            OnCarTelemetryReceived(e.Packet, e.OldPacket);
        }

        private void _telemetryManager_EventPacketReceived(object sender, PacketReceivedEventArgs<EventPacket> e)
        {
            if (e.OldPacket.Equals(default(EventPacket)))
            {
                return;
            }

            _dataLastReceived = DateTimeOffset.Now;
        }

        private void _telemetryManager_LapPacketReceived(object sender, PacketReceivedEventArgs<PacketLapData> e)
        {
            if (e.OldPacket.Equals(default(PacketLapData)))
            {
                return;
            }
            
            _dataLastReceived = DateTimeOffset.Now;

            CheckLapChanged(e);
            OnLapPacketReceived(new PacketReceivedEventArgs<PacketLapData>(e.Packet, e.OldPacket));
        }

        private void _telemetryManager_ParticipantsPacketReceived(object sender, PacketReceivedEventArgs<PacketParticipantsData> e)
        {
            if (e.OldPacket.Equals(default(PacketParticipantsData)))
            {
                return;
            }

            _dataLastReceived = DateTimeOffset.Now;
        }

        private void _telemetryManager_SessionChanged(object sender, EventArgs e)
        {
            OnSessionChanged(e);
        }

        private void _telemetryManager_SessionPacketReceived(object sender, PacketReceivedEventArgs<PacketSessionData> e)
        {
            if (e.OldPacket.Equals(default(PacketSessionData)))
            {
                return;
            }

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

        private void OnCarStatusReceived(PacketCarStatusData oldCarStatusData, PacketCarStatusData newCarStatusData)
        {
            CarStatusReceived?.Invoke(this, new PacketReceivedEventArgs<PacketCarStatusData>(oldCarStatusData, newCarStatusData));
        }

        private void OnCarTelemetryReceived(PacketCarTelemetryData oldCarTelemetryData, PacketCarTelemetryData newCarTelemetryData)
        {
            CarTelemetryReceived?.Invoke(this, new PacketReceivedEventArgs<PacketCarTelemetryData>(oldCarTelemetryData, newCarTelemetryData));
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
