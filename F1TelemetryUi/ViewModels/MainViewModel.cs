using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media;
using Caliburn.Micro;
using F1Telemetry;
using F1Telemetry.Models.Raw;
using F1TelemetryUi.Events;
using F1TelemetryUi.Referencing;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace F1TelemetryUi.ViewModels
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : PropertyChangedBase, IShell
    {
        public int _sector;
        public TimeSpan[] _sectorTimes = new TimeSpan[3];
        public SeriesCollection _seriesCollection;
        private readonly IEventAggregator _eventAggregator;
        private readonly F1Manager _f1Manager;
        private readonly IWindowManager _windowManager;
        private TimeSpan _currentLapTime;
        private int _gearMax = 8;

        private int _gearMin = 1;

        private int _kmhMax = 360;

        private int _kmhMin = 1;

        private int _rpmMax = 14000;

        private int _rpmMin = 3000;

        private List<F12017TelemetryPacket> _telemetryPackets = new List<F12017TelemetryPacket>();

        private double? _timeMax = null;

        private double? _timeMin = null;

        public MainViewModel(
            IWindowManager windowManager,
            IEventAggregator eventAggregator,
            F1Manager f1Manager,
            ReferencingStateMachine referencingStateMachine)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;

            _f1Manager = f1Manager;
            _f1Manager.PacketReceived += _f1Manager_PacketReceived;
            _f1Manager.NewLap += _f1Manager_NewLap;
            _f1Manager.Start();

            InitGraphSettings();

            //_windowManager.ShowWindow(new MapViewModel(_windowManager, _eventAggregator, referencingStateMachine));
        }

        public TimeSpan CurrentLapTime
        {
            get { return _currentLapTime; }

            set
            {
                _currentLapTime = value;
                NotifyOfPropertyChange();
            }
        }

        public Func<double, string> Formatter { get; set; }

        public int GearMax
        {
            get
            {
                return _gearMax;
            }

            set
            {
                _gearMax = value;
                NotifyOfPropertyChange();
            }
        }

        public int GearMin
        {
            get
            {
                return _gearMin;
            }

            set
            {
                _gearMin = value;
                NotifyOfPropertyChange();
            }
        }

        public int KmhMax
        {
            get
            {
                return _kmhMax;
            }

            set
            {
                _kmhMax = value;
                NotifyOfPropertyChange();
            }
        }

        public int KmhMin
        {
            get
            {
                return _kmhMin;
            }

            set
            {
                _kmhMin = value;
                NotifyOfPropertyChange();
            }
        }

        public int RpmMax
        {
            get
            {
                return _rpmMax;
            }

            set
            {
                _rpmMax = value;
                NotifyOfPropertyChange();
            }
        }

        public int RpmMin
        {
            get
            {
                return _rpmMin;
            }

            set
            {
                _rpmMin = value;
                NotifyOfPropertyChange();
            }
        }

        public int Sector
        {
            get { return _sector; }

            set
            {
                _sector = value;
                NotifyOfPropertyChange();
            }
        }

        public TimeSpan[] SectorTimes
        {
            get { return _sectorTimes; }

            set
            {
                _sectorTimes = value;
                NotifyOfPropertyChange();
            }
        }

        public SeriesCollection SeriesCollection
        {
            get { return _seriesCollection; }

            set
            {
                _seriesCollection = value;
                NotifyOfPropertyChange();
            }
        }

        public List<F12017TelemetryPacket> TelemetryPackets
        {
            get
            {
                return _telemetryPackets;
            }

            set
            {
                _telemetryPackets = value;
            }
        }

        public double? TimeMax
        {
            get
            {
                return _timeMax;
            }

            set
            {
                _timeMax = value;
                NotifyOfPropertyChange();
            }
        }

        public double? TimeMin
        {
            get
            {
                return _timeMin;
            }

            set
            {
                _timeMin = value;
                NotifyOfPropertyChange();
            }
        }

        private void _f1Manager_NewLap(object sender, NewLapEventArgs e)
        {
            SeriesCollection[0].Values.Clear();
            SeriesCollection[1].Values.Clear();
            SeriesCollection[2].Values.Clear();

            _eventAggregator.PublishOnUIThread(new ClearCanvasEvent());
            if (TelemetryPackets.Count > 1000) // probably more than a thousand to finish a real lap
            {
                //var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<F12017TelemetryPacket>));

                //System.IO.TextWriter writer = new System.IO.StreamWriter("D:\\temp\\laphsilver.xml");
                //serializer.Serialize(writer, TelemetryPackets);

                TelemetryPackets.Clear();
            }

            NotifyOfPropertyChange(() => SeriesCollection);
        }

        private void _f1Manager_PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            if (e.OldPacket != null && e.NewPacket != null)
            {
                F12017TelemetryPacket telemetryPacket = e.NewPacket.RawPacket;

                TelemetryPackets.Add(telemetryPacket);

                var oldX = e.OldPacket.RawPacket.X;
                var newX = e.NewPacket.RawPacket.X;
                var oldY = e.OldPacket.RawPacket.Z;
                var newY = e.NewPacket.RawPacket.Z;

                _eventAggregator.PublishOnUIThread(new DrawEvent(new Point(newX, newY), Brushes.Blue));

                CurrentLapTime = e.NewPacket.CurrentLapTime;

                //ViewModel.Sector = e.NewPacket.CurrentSectorIndex;

                //if (e.NewPacket.CurrentSectorIndex != e.OldPacket.CurrentSectorIndex)
                //{
                //    ViewModel.SectorTimes[e.OldPacket.CurrentSectorIndex] = e.NewPacket.CurrentLapTime - ViewModel.SectorTimes[ViewModel.Sector - 1];
                //}

                if (Math.Abs(e.NewPacket.SpeedKmh - e.OldPacket.SpeedKmh) > 0.1f)
                {
                    SeriesCollection[0].Values.Add(new TimeSpanValue(e.NewPacket.CurrentLapTime, e.OldPacket.SpeedKmh));
                }

                if (Math.Abs(e.NewPacket.Rpms - e.OldPacket.Rpms) > 1f)
                {
                    SeriesCollection[1].Values.Add(new TimeSpanValue(e.NewPacket.CurrentLapTime, e.NewPacket.Rpms));
                }

                int newGear = 0;
                int oldGear = 0;

                if (e.NewPacket.Gear != "N" && e.NewPacket.Gear != "R")
                {
                    newGear = int.Parse(e.NewPacket.Gear);
                }

                if (e.OldPacket.Gear != "N" && e.OldPacket.Gear != "R")
                {
                    oldGear = int.Parse(e.OldPacket.Gear);
                }

                if (newGear != oldGear)
                {
                    SeriesCollection[2].Values.Add(new TimeSpanValue(e.NewPacket.CurrentLapTime, newGear));
                    Console.WriteLine(newGear);
                }

                NotifyOfPropertyChange(() => SeriesCollection);
            }
        }

        private void InitGraphSettings()
        {
            CartesianMapper<TimeSpanValue> dayConfig = Mappers.Xy<TimeSpanValue>()
              .X(dateModel =>
              {
                  return dateModel.TimeSpan.TotalMilliseconds / TimeSpan.FromMilliseconds(1).TotalMilliseconds;
              })
              .Y(dateModel =>
              {
                  return dateModel.Value;
              });

            SeriesCollection = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.CornflowerBlue,
                    PointGeometrySize = 0,
                    Values = new ChartValues<TimeSpanValue>() { },
                    ScalesYAt = 0,
                },
                new LineSeries
                {
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Green,
                    PointGeometrySize = 0,
                    Values = new ChartValues<TimeSpanValue>() { },
                    ScalesYAt = 1,
                },
                new StepLineSeries
                {
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Red,
                    PointGeometrySize = 0,
                    AlternativeStroke = Brushes.Red,
                    Values = new ChartValues<TimeSpanValue>() { },                    
                    ScalesYAt = 2,
                },
            };

            Formatter = value =>
            {
                var x = value > 0 ? value : 0;
                return new DateTime((long)(x * TimeSpan.FromMilliseconds(1).Ticks)).ToString("mm\\:ss\\:fff");
            };
        }
    }
}
