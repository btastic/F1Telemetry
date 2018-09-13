using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using F1Telemetry;
using F1Telemetry.Models.Raw.F12018;
using F1TelemetryUi.Utility;

namespace F1TelemetryUi.ViewModels
{
    public class TimingOverlayViewModel : PropertyChangedBase, IShell
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly F1Manager _f1Manager;
        private readonly IWindowManager _windowManager;

        private SortedObservableCollection<CarTimingViewModel> _carData = new SortedObservableCollection<CarTimingViewModel>();

        public TimingOverlayViewModel(IWindowManager windowManager,
            IEventAggregator eventAggregator,
            F1Manager f1Manager)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _f1Manager = f1Manager;

            _f1Manager.LapPacketReceived += _f1Manager_LapPacketReceived;
            _f1Manager.CarTelemetryReceived += _f1Manager_CarTelemetryReceived;
            _f1Manager.CarStatusReceived += _f1Manager_CarStatusReceived;
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
            }
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
                    ? TimeSpan.Zero.ToString("ss':'fff")
                    : TimeSpan.FromSeconds((carInFront.Distance - currentCar.Distance) / Math.Max(currentCar.Speed, .1f)).ToString("ss':'fff");

                LastLapPacketCarData[i].TimeDistanceCarBehind =
                    carBehind == null
                    ? TimeSpan.Zero.ToString("ss':'fff")
                    : TimeSpan.FromSeconds((currentCar.Distance - carBehind.Distance) / Math.Max(carBehind.Speed, .1f)).ToString("ss':'fff");
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
            }

            NotifyOfPropertyChange("CarData");
        }
    }
}
