﻿using System;
using System.Collections.Generic;
using System.Linq;
using Caliburn.Micro;
using F1Telemetry.Events;
using F1Telemetry.Manager;
using F1Telemetry.Models.Raw.F12018;
using F1TelemetryUi.Utility;

namespace F1TelemetryUi.ViewModels
{
    public class TimingOverlayViewModel : PropertyChangedBase, IShell
    {
        private SortedObservableCollection<CarTimingViewModel> _carData = new SortedObservableCollection<CarTimingViewModel>();
        private Dictionary<int, float> _trackSectors = new Dictionary<int, float>();

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
                LastLapPacketCarData[i].Name = e.Packet.Participants[i].GetThreeLetterName(_onlineSession);
                LastLapPacketCarData[i].Team = e.Packet.Participants[i].Team;
            }

            NotifyOfPropertyChange(() => CarData);
        }

        private void _f1Manager_SessionPacketReceived(object sender, PacketReceivedEventArgs<PacketSessionData> e)
        {
            UpdateSession(e);
        }

        private void _f1Manager_SessionChanged(object sender, PacketReceivedEventArgs<PacketSessionData> e)
        {
            NewSession();
        }

        private void NewSession()
        {
            LastLapPacketCarData.Clear();
            NotifyOfPropertyChange(() => CarData);
        }

        private void UpdateSession(PacketReceivedEventArgs<PacketSessionData> e)
        {
            MaxLaps = e.Packet.TotalLaps;
            _onlineSession = e.Packet.NetworkGame == NetworkGame.Online;
            CalculateTrackSectors(e.Packet.TrackLength);
            NotifyOfPropertyChange(() => CurrentLap);
        }

        private void CalculateTrackSectors(ushort trackLength)
        {
            if (trackLength == 0)
            {
                return;
            }

            if (_trackSectors.Count > 0)
            {
                return;
            }

            var sectorCount = trackLength * 5;
            var sectorLength = trackLength / (float)sectorCount;

            var dict = new Dictionary<int, float>();

            for (int i = 0; i < sectorCount; i++)
            {
                dict.Add(i + 1, sectorLength * i);
            }

            _trackSectors = dict;
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
                if (_carData == value)
                {
                    return;
                }

                _carData = value;
                NotifyOfPropertyChange(() => CarData);
                NotifyOfPropertyChange(() => CurrentLap);
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
            set
            {
                if (_maxLaps == value)
                {
                    return;
                }

                _maxLaps = value;
                NotifyOfPropertyChange();
            }
        }

        private bool _onlineSession;

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
            InitializeCarGrid(e.Packet);
            CalculateCarSectors(e.Packet);
            UpdateCarGrid(e.Packet);
        }

        private void CalculateCarSectors(PacketLapData packet)
        {
            if (LastLapPacketCarData == null && LastLapPacketCarData.Count == 0)
            {
                return;
            }

            if (_trackSectors.Count == 0)
            {
                return;
            }

            for (int i = 0; i < LastLapPacketCarData.Count; i++)
            {
                if (LastLapPacketCarData[i] == null)
                {
                    continue;
                }

                if (packet.LapData[i].Equals(default(LapData)))
                {
                    continue;
                }

                var sectorIndex = FindSectorIndexByLapDistance(packet.LapData[i].LapDistance);
                                
                if (sectorIndex == -1)
                {
                    continue;
                }

                LastLapPacketCarData[i].CurrentDeltaSector = sectorIndex;

                if (!LastLapPacketCarData[i].DeltaSectorTimes.ContainsKey(packet.LapData[i].CurrentLapNum))
                {
                    LastLapPacketCarData[i]
                        .DeltaSectorTimes.Add(packet.LapData[i].CurrentLapNum, new Dictionary<int, TimeSpan>());
                }

                if (!LastLapPacketCarData[i].DeltaSectorTimes[packet.LapData[i].CurrentLapNum].ContainsKey(sectorIndex))
                {
                    LastLapPacketCarData[i]
                        .DeltaSectorTimes[packet.LapData[i].CurrentLapNum]
                            .Add(sectorIndex, TimeSpan.FromSeconds(packet.Header.SessionTime));
                }
                else
                {
                    LastLapPacketCarData[i]
                        .DeltaSectorTimes[packet.LapData[i].CurrentLapNum][sectorIndex] = TimeSpan.FromSeconds(packet.Header.SessionTime);
                }
            }
        }

        private int FindSectorIndexByLapDistance(float lapDistance)
        {
            if (_trackSectors.Count == 0 || lapDistance < 0)
            {
                return -1;
            }

            return _trackSectors.Last(x => lapDistance >= x.Value).Key;
        }

        private void InitializeCarGrid(PacketLapData packetLapData)
        {
            DetectGridChanges(packetLapData);
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

        private void DetectGridChanges(PacketLapData packetLapData)
        {
            int validCount = 0;
            foreach (LapData item in packetLapData.LapData)
            {
                if (!item.Equals(default(LapData)))
                {
                    validCount++;
                }
            }

            if (validCount != LastLapPacketCarData.Count)
            {
                LastLapPacketCarData.Clear();
            }
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

            NotifyOfPropertyChange(() => CarData);
        }

        private void UpdateCarGrid(PacketCarTelemetryData e)
        {
            if (LastLapPacketCarData == null || LastLapPacketCarData.Count == 0)
            {
                return;
            }

            NotifyOfPropertyChange(() => CarData);
        }

        private void CalculateCarDeltas()
        {
            for (int i = 0; i < LastLapPacketCarData.Count; i++)
            {
                CarTimingViewModel currentCar = LastLapPacketCarData[i];
                CarTimingViewModel carInFront = LastLapPacketCarData.SingleOrDefault(x => x.CarPosition == currentCar.CarPosition - 1);

                if (currentCar.TotalDistance <= 0 && carInFront?.TotalDistance <= 0)
                {
                    continue;
                }

                if (carInFront == null)
                {
                    LastLapPacketCarData[i].TimeDistanceCarAhead = "Interval";
                    continue;
                }

                if (currentCar.DeltaSectorTimes.Count == 0)
                {
                    continue;
                }

                if (!carInFront.DeltaSectorTimes.ContainsKey(currentCar.CurrentLap))
                {
                    continue;
                }

                TimeSpan carInFrontTimeStamp;
                TimeSpan currentCarTimeStamp;
                KeyValuePair<int, TimeSpan> bestSectorCurrentCar;
                KeyValuePair<int, TimeSpan> bestSectorCarInFront;
                Dictionary<int, TimeSpan> deltaSectorsCarInFront;
                Dictionary<int, TimeSpan> deltaSectorsCurrentCar;

                // the current car does not have the sector of the car that is in front
                // Find most fitting sector to compare to
                if (!carInFront.DeltaSectorTimes[currentCar.CurrentLap].ContainsKey(currentCar.CurrentDeltaSector))
                {
                    deltaSectorsCarInFront =
                       carInFront.DeltaSectorTimes[currentCar.CurrentLap];

                    deltaSectorsCurrentCar =
                         currentCar.DeltaSectorTimes[currentCar.CurrentLap];

                    bestSectorCarInFront = deltaSectorsCarInFront.LastOrDefault(x => x.Key < currentCar.CurrentDeltaSector);
                    bestSectorCurrentCar = deltaSectorsCurrentCar.FirstOrDefault(x => x.Key > bestSectorCarInFront.Key);

                    if (bestSectorCurrentCar.Value == default(TimeSpan)
                        || bestSectorCarInFront.Value == default(TimeSpan))
                    {
                        return;
                    }

                    carInFrontTimeStamp = bestSectorCarInFront.Value;
                    currentCarTimeStamp = bestSectorCurrentCar.Value;
                }
                else
                {
                    carInFrontTimeStamp = carInFront.DeltaSectorTimes[currentCar.CurrentLap][currentCar.CurrentDeltaSector];
                    currentCarTimeStamp = currentCar.DeltaSectorTimes[currentCar.CurrentLap][currentCar.CurrentDeltaSector];
                }
                // take the front cars sector times and compare it to the car behinds sector
                // the car in front was definitely already in this sector

                if (carInFrontTimeStamp == currentCarTimeStamp)
                {
                    ;
                }
                LastLapPacketCarData[i].TimeDistanceCarAhead =
                    "+ " + (currentCarTimeStamp - carInFrontTimeStamp).Duration().ToString("s\\.fff");

                NotifyOfPropertyChange(() => CarData);
            }
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
                LastLapPacketCarData[i].TotalDistance = packetLapData.LapData[i].TotalDistance;
                LastLapPacketCarData[i].IsPlayer =
                    packetLapData.GetPlayerLapData().CarPosition == packetLapData.LapData[i].CarPosition;
                LastLapPacketCarData[i].CurrentLap = packetLapData.LapData[i].CurrentLapNum;
                LastLapPacketCarData[i].CurrentTrackSector = (int)packetLapData.LapData[i].Sector;
                LastLapPacketCarData[i].Pitting = packetLapData.LapData[i].PitStatus != PitStatus.None;
            }

            CalculateCarDeltas();

            NotifyOfPropertyChange(() => CarData);
        }
    }
}
