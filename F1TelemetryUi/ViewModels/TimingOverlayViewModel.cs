using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using F1Telemetry;
using F1Telemetry.Events;
using F1Telemetry.Manager;
using F1Telemetry.Models.Raw.F12018;
using F1TelemetryUi.Utility;

namespace F1TelemetryUi.ViewModels
{
    public class TimingOverlayViewModel : PropertyChangedBase, IShell
    {
        private SortedObservableCollection<CarTimingViewModel> _carData = new SortedObservableCollection<CarTimingViewModel>();

        public TimingOverlayViewModel(IWindowManager windowManager,
            IEventAggregator eventAggregator,
            F1Manager f1Manager)
        {
            F1Manager _f1Manager = f1Manager;

            _f1Manager.LapPacketReceived += _f1Manager_LapPacketReceived;
            _f1Manager.CarTelemetryReceived += _f1Manager_CarTelemetryReceived;
            _f1Manager.CarStatusReceived += _f1Manager_CarStatusReceived;
            _f1Manager.SessionChanged += _f1Manager_SessionChanged;
            _f1Manager.SessionPacketReceived += _f1Manager_SessionPacketReceived;
            _f1Manager.ParticipantsPacketReceived += _f1Manager_ParticipantsPacketReceived;
        }

        private void _f1Manager_ParticipantsPacketReceived(object sender, PacketReceivedEventArgs<PacketParticipantsData> e)
        {
            UpdateParticipantInfo(e);
        }

        private void UpdateParticipantInfo(PacketReceivedEventArgs<PacketParticipantsData> e)
        {
            if (LastLapPacketCarData == null || LastLapPacketCarData.Count == 0)
            {
                return;
            }

            for (int i = 0; i < LastLapPacketCarData.Count; i++)
            {
                LastLapPacketCarData[i].Name = e.Packet.Participants[i].GetThreeLetterName();
                LastLapPacketCarData[i].Team = e.Packet.Participants[i].Team;
            }

            NotifyOfPropertyChange("CarData");
        }

        private void _f1Manager_SessionPacketReceived(object sender, PacketReceivedEventArgs<PacketSessionData> e)
        {
            UpdateSession(e);
        }

        private void _f1Manager_SessionChanged(object sender, PacketReceivedEventArgs<PacketSessionData> e)
        {
            UpdateSession(e);
        }

        private void UpdateSession(PacketReceivedEventArgs<PacketSessionData> e)
        {
            LastLapPacketCarData = null;
            CarData = null;
            MaxLaps = e.Packet.TotalLaps;
            NotifyOfPropertyChange("CurrentLap");
        }

        public SortedObservableCollection<CarTimingViewModel> CarData
        {
            get
            {
                _carData.Sort(p => p.CarPosition);
                return _carData;
            }

            set
            {
                _carData = value;
                NotifyOfPropertyChange("CarData");
                NotifyOfPropertyChange("CurrentLap");
            }
        }

        public int CurrentLap
        {
            get
            {
                if (CarData == null || CarData.Count == 0)
                {
                    return 0;
                }

                CarTimingViewModel leadingCar = CarData.SingleOrDefault(x => x.CarPosition == 1);

                if (leadingCar == null)
                {
                    return 0;
                }

                return leadingCar.CurrentLap;
            }
        }

        private int _maxLaps;
        public int MaxLaps
        {
            get { return _maxLaps; }
            set { _maxLaps = value; NotifyOfPropertyChange(); }
        }


        public List<CarTimingViewModel> LastLapPacketCarData { get; private set; } = new List<CarTimingViewModel>();

        private void _f1Manager_CarStatusReceived(object sender, PacketReceivedEventArgs<PacketCarStatusData> e)
        {
            UpdateCarGrid(e.Packet);
        }

        private void _f1Manager_CarTelemetryReceived(object sender, PacketReceivedEventArgs<PacketCarTelemetryData> e)
        {
            UpdateCarGrid(e.Packet);
        }

        private void _f1Manager_LapPacketReceived(object sender, PacketReceivedEventArgs<PacketLapData> e)
        {
            CollectCarGrid(e.Packet);
            UpdateCarGrid(e.Packet);
        }

        private void CollectCarGrid(PacketLapData packetLapData)
        {
            if (LastLapPacketCarData != null && LastLapPacketCarData.Count > 0)
            {
                return;
            }

            for (int i = 0; i < packetLapData.LapData.Length; i++)
            {
                if (packetLapData.LapData[i].Equals(default(LapData)))
                {
                    // don't add cars that don't have a value
                    continue;
                }

                LastLapPacketCarData.Add(new CarTimingViewModel
                {
                    CarIndex = i,
                });
            }

            CarData = new SortedObservableCollection<CarTimingViewModel>(LastLapPacketCarData);
        }

        private void UpdateCarGrid(PacketCarStatusData packet)
        {
            if (LastLapPacketCarData == null || LastLapPacketCarData.Count == 0)
            {
                return;
            }

            for (int i = 0; i < LastLapPacketCarData.Count; i++)
            {
                LastLapPacketCarData[i].TyreCompound = packet.CarStatusData[i].TyreCompound;
            }

            NotifyOfPropertyChange("CarData");
        }

        private void UpdateCarGrid(PacketCarTelemetryData packet)
        {
            if (LastLapPacketCarData == null || LastLapPacketCarData.Count == 0)
            {
                return;
            }

            for (int i = 0; i < LastLapPacketCarData.Count; i++)
            {
                LastLapPacketCarData[i].Speed = packet.CarTelemetryData[i].Speed * (5.0f / 18.0f);
            }

            for (int i = 0; i < LastLapPacketCarData.Count; i++)
            {
                CarTimingViewModel currentCar = LastLapPacketCarData[i];
                CarTimingViewModel carInFront = LastLapPacketCarData.SingleOrDefault(x => x.CarPosition == currentCar.CarPosition - 1);
                CarTimingViewModel carBehind = LastLapPacketCarData.SingleOrDefault(x => x.CarPosition == currentCar.CarPosition + 1);

                if (currentCar.Distance <= 0 && carInFront?.Distance <= 0 && carBehind?.Distance <= 0)
                {
                    continue;
                }

                LastLapPacketCarData[i].TimeDistanceCarAhead =
                    carInFront == null
                    ? "Interval"
                    : "+ " + TimeSpan.FromSeconds((carInFront.Distance - currentCar.Distance) / Math.Max(currentCar.Speed, .1f)).ToString("s\\.fff");

                LastLapPacketCarData[i].TimeDistanceCarBehind =
                    carBehind == null
                    ? TimeSpan.Zero.ToString("s\\.fff")
                    : "+ " + TimeSpan.FromSeconds((currentCar.Distance - carBehind.Distance) / Math.Max(carBehind.Speed, .1f)).ToString("s\\.fff");
            }

            NotifyOfPropertyChange("CarData");
        }

        private void UpdateCarGrid(PacketLapData packetLapData)
        {
            if (LastLapPacketCarData == null || LastLapPacketCarData.Count == 0)
            {
                return;
            }

            for (int i = 0; i < LastLapPacketCarData.Count; i++)
            {
                LastLapPacketCarData[i].CarPosition = packetLapData.LapData[i].CarPosition;
                LastLapPacketCarData[i].Distance = packetLapData.LapData[i].TotalDistance;
                LastLapPacketCarData[i].IsPlayer =
                    packetLapData.GetPlayerLapData().CarPosition == packetLapData.LapData[i].CarPosition;
                LastLapPacketCarData[i].CurrentLap = packetLapData.LapData[i].CurrentLapNum;
            }

            NotifyOfPropertyChange("CarData");
        }
    }
}
