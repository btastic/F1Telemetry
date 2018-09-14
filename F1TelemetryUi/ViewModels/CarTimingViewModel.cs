using System;
using System.Collections.Generic;
using Caliburn.Micro;
using F1Telemetry.Models.Raw.F12018;

namespace F1TelemetryUi.ViewModels
{
    public class CarTimingViewModel : PropertyChangedBase
    {
        private float _distance;
        public float Distance
        {
            get { return _distance; }
            set { _distance = value; NotifyOfPropertyChange(); }
        }

        private float _speed;
        public float Speed
        {
            get { return _speed; }
            set { _speed = value; NotifyOfPropertyChange(); }
        }

        private int _carIndex;
        public int CarIndex
        {
            get { return _carIndex; }
            set { _carIndex = value; NotifyOfPropertyChange(); }
        }

        private int _carPosition;
        public int CarPosition
        {
            get { return _carPosition; }
            set { _carPosition = value; NotifyOfPropertyChange(); }
        }

        private bool _isPlayer;
        public bool IsPlayer
        {
            get { return _isPlayer; }
            set { _isPlayer = value; NotifyOfPropertyChange(); }
        }

        private string _timeDistanceCarAhead;
        public string TimeDistanceCarAhead
        {
            get { return _timeDistanceCarAhead; }
            set { _timeDistanceCarAhead = value; NotifyOfPropertyChange(); }
        }

        private TyreCompound _tyreCompound;
        public TyreCompound TyreCompound
        {
            get { return _tyreCompound; }
            set
            {
                if (value == _tyreCompound)
                {
                    return;
                }

                _tyreCompound = value;
                NotifyOfPropertyChange();
            }
        }

        private int _currentLap;
        public int CurrentLap
        {
            get => _currentLap;
            set
            {
                _currentLap = value;
                NotifyOfPropertyChange();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; NotifyOfPropertyChange(); }
        }

        private Team _team;
        public Team Team
        {
            get { return _team; }
            set { _team = value; NotifyOfPropertyChange(); }
        }

        // Dictionary 1 = Lap & Dictionary 2
        // Dictionary 2 = Sector & Time
        private Dictionary<int, Dictionary<int, TimeSpan>> _sectorTimes = new Dictionary<int, Dictionary<int, TimeSpan>>();

        public Dictionary<int, Dictionary<int, TimeSpan>> SectorTimes
        {
            get { return _sectorTimes; }
            set { _sectorTimes = value; NotifyOfPropertyChange(); }
        }

        private int _currentSector;
        public int CurrentSector
        {
            get { return _currentSector; }
            set { _currentSector = value; NotifyOfPropertyChange(); }
        }
    }
}
