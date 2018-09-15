using System;
using System.Collections.Generic;
using Caliburn.Micro;
using F1Telemetry.Models.Raw.F12018;

namespace F1TelemetryUi.ViewModels
{
    public class CarTimingViewModel : PropertyChangedBase
    {
        private float _totalDistance;
        public float TotalDistance
        {
            get { return _totalDistance; }
            set
            {
                if (_totalDistance == value)
                {
                    return;
                }

                _totalDistance = value;
                NotifyOfPropertyChange();
            }
        }

        private float _speed;
        public float Speed
        {
            get { return _speed; }
            set
            {
                if(_speed == value)
                {
                    return;
                }

                _speed = value;
                NotifyOfPropertyChange();
            }
        }

        private int _carIndex;
        public int CarIndex
        {
            get { return _carIndex; }
            set
            {
                if (_carIndex == value)
                {
                    return;
                }

                _carIndex = value;
                NotifyOfPropertyChange();
            }
        }

        private int _carPosition;
        public int CarPosition
        {
            get { return _carPosition; }
            set
            {
                if (_carPosition == value)
                {
                    return;
                }

                _carPosition = value;
                NotifyOfPropertyChange();
            }
        }

        private bool _isPlayer;
        public bool IsPlayer
        {
            get { return _isPlayer; }
            set
            {
                if (_isPlayer == value)
                {
                    return;
                }

                _isPlayer = value;
                NotifyOfPropertyChange();
            }
        }

        private string _timeDistanceCarAhead;
        public string TimeDistanceCarAhead
        {
            get { return _timeDistanceCarAhead; }
            set
            {
                if (_timeDistanceCarAhead == value)
                {
                    return;
                }

                _timeDistanceCarAhead = value;
                NotifyOfPropertyChange();
            }
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
            get { return _currentLap; }
            set
            {
                if (_currentLap == value)
                {
                    return;
                }

                _currentLap = value;
                NotifyOfPropertyChange();
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value)
                {
                    return;
                }

                _name = value;
                NotifyOfPropertyChange();
            }
        }

        private Team _team;
        public Team Team
        {
            get { return _team; }
            set
            {
                if (_team == value)
                {
                    return;
                }

                _team = value;
                NotifyOfPropertyChange();
            }
        }

        // Dictionary 1 = Lap & Dictionary 2
        // Dictionary 2 = Sector & Time
        private Dictionary<int, Dictionary<int, TimeSpan>> _deltaSectorTimes = new Dictionary<int, Dictionary<int, TimeSpan>>();
        public Dictionary<int, Dictionary<int, TimeSpan>> DeltaSectorTimes
        {
            get { return _deltaSectorTimes; }
            set
            {
                if (_deltaSectorTimes == value)
                {
                    return;
                }

                _deltaSectorTimes = value;
                NotifyOfPropertyChange();
            }
        }

        private int _currentDeltaSector;
        public int CurrentDeltaSector
        {
            get { return _currentDeltaSector; }
            set
            {
                if (_currentDeltaSector == value)
                {
                    return;
                }

                _currentDeltaSector = value;
                NotifyOfPropertyChange();
            }
        }

        private int _currentTrackSector;
        public int CurrentTrackSector
        {
            get { return _currentTrackSector; }
            set
            {
                if (_currentTrackSector == value)
                {
                    return;
                }

                _currentTrackSector = value;
                NotifyOfPropertyChange();
            }
        }


        private bool _pitting;
        public bool Pitting
        {
            get { return _pitting; }
            set
            {
                if (_pitting != value)
                {
                    _pitting = value;
                    NotifyOfPropertyChange(() => Pitting);
                }
            }
        }
        
    }
}
