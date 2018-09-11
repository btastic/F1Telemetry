using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Caliburn.Micro;
using F1Telemetry;
using F1Telemetry.Models.Raw.F12017;
using F1Telemetry.Models.Raw.F12018;
using F1TelemetryUi.Events;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace F1TelemetryUi.ViewModels
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : PropertyChangedBase, IShell
    {
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
        private int _sector;
        private TimeSpan[] _sectorTimes = new TimeSpan[3];
        private SeriesCollection _seriesCollection = new SeriesCollection();
        private List<F12017TelemetryPacket> _telemetryPackets = new List<F12017TelemetryPacket>();

        private double? _timeMax;

        private double? _timeMin;

        public MainViewModel(
            IWindowManager windowManager,
            IEventAggregator eventAggregator,
            F1Manager f1Manager)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;

            _f1Manager = f1Manager;
            _f1Manager.NewLap += _f1Manager_NewLap;
            _f1Manager.LapPacketReceived += _f1Manager_LapPacketReceived;
            _f1Manager.CarTelemetryReceived += _f1Manager_CarTelemetryReceived;
            _f1Manager.SessionChanged += _f1Manager_SessionChanged;
            _f1Manager.CarStatusReceived += _f1Manager_CarStatusReceived;

            _f1Manager.Start();

            InitGraphSettings();

            _windowManager.ShowWindow(new TimingOverlayViewModel(_windowManager, _eventAggregator, _f1Manager));
        }

        public TimeSpan CurrentLapTime
        {
            get { return _currentLapTime; }

            set
            {
                if (_currentLapTime != value)
                {
                    _currentLapTime = value;
                    NotifyOfPropertyChange();
                }
            }
        }

        public Func<double, string> Formatter { get; set; }

        private bool _graphInitialized;

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
                NotifyOfPropertyChange();
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

        private void _f1Manager_CarStatusReceived(object sender, PacketReceivedEventArgs<PacketCarStatusData> e)
        {
            GearMax = e.Packet.GetPlayerLapData().MaxGears - 1;
            RpmMax = e.Packet.GetPlayerLapData().MaxRpm;
            RpmMin = e.Packet.GetPlayerLapData().IdleRpm;
        }

        private void _f1Manager_CarTelemetryReceived(object sender, PacketReceivedEventArgs<PacketCarTelemetryData> e)
        {
            if (!_graphInitialized)
            {
                return;
            }

            if (!e.OldPacket.Equals(default(PacketCarTelemetryData)) && !e.Packet.Equals(default(PacketCarTelemetryData)))
            {
                if (Math.Abs(e.Packet.GetPlayerLapData().Speed - e.OldPacket.GetPlayerLapData().Speed) > 0.1f)
                {
                    SeriesCollection[0].Values.Add(new TimeSpanValue(CurrentLapTime, e.Packet.GetPlayerLapData().Speed));
                }

                if (Math.Abs(e.Packet.GetPlayerLapData().EngineRpm - e.OldPacket.GetPlayerLapData().EngineRpm) > 1f)
                {
                    SeriesCollection[1].Values.Add(new TimeSpanValue(CurrentLapTime, e.Packet.GetPlayerLapData().EngineRpm));
                }

                if (e.Packet.GetPlayerLapData().Gear != e.OldPacket.GetPlayerLapData().Gear)
                {
                    SeriesCollection[2].Values.Add(new TimeSpanValue(CurrentLapTime, (int)e.Packet.GetPlayerLapData().Gear));
                }

                NotifyOfPropertyChange(() => SeriesCollection);
            }
        }

        private void _f1Manager_LapPacketReceived(object sender, PacketReceivedEventArgs<PacketLapData> e)
        {
            if (!e.OldPacket.Equals(default(PacketLapData)) && !e.Packet.Equals(default(PacketLapData)))
            {
                CurrentLapTime = TimeSpan.FromSeconds(e.Packet.GetPlayerLapData().CurrentLapTime);
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

        private void _f1Manager_SessionChanged(object sender, EventArgs e)
        {
            SeriesCollection[0].Values.Clear();
            SeriesCollection[1].Values.Clear();
            SeriesCollection[2].Values.Clear();
            TelemetryPackets.Clear();

            NotifyOfPropertyChange(() => SeriesCollection);
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

            _graphInitialized = true;
        }
    }
}
