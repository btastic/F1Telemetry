using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml.Serialization;
using F1Telemetry;
using F1Telemetry.Models.Raw;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;

namespace F1TelemetryUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ViewModel ViewModel { get; set; } = new ViewModel();

        public List<F12017TelemetryPacket> TelemetryPackets { get; set; } = new List<F12017TelemetryPacket>();

        public MapWindow MapWindow;

        public MainWindow()
        {
            InitializeComponent();

            // init window top right
            this.Left = SystemParameters.PrimaryScreenWidth - this.Width;
            MapWindow = new MapWindow(this);
            MapWindow.Left = SystemParameters.PrimaryScreenWidth - MapWindow.Width;
            MapWindow.Top = SystemParameters.PrimaryScreenHeight - this.Height;
            MapWindow.Show();
            InitGraph();

            var telemetryManager = new TelemetryManager(20777);
            var manager = new F1Manager(telemetryManager, 100);
            manager.PacketReceived += Manager_PacketReceived;
            manager.NewLap += Manager_NewLap;
            manager.Start();

            DataContext = ViewModel;
        }        

        private void Manager_NewLap(object sender, NewLapEventArgs e)
        {
            ViewModel.SeriesCollection[0].Values.Clear();
            ViewModel.SeriesCollection[1].Values.Clear();
            ViewModel.SeriesCollection[2].Values.Clear();

            if (TelemetryPackets.Count > 1000) // probably more than a thousand to finish a real lap
            {
                //var serializer = new XmlSerializer(typeof(List<F12017TelemetryPacket>));

                //TextWriter writer = new StreamWriter("D:\\temp\\laphungaro.xml");
                //serializer.Serialize(writer, TelemetryPackets);

                TelemetryPackets.Clear();
            }

            Array.Clear(ViewModel.SectorTimes, 0, ViewModel.SectorTimes.Length);
        }

        private void Manager_PacketReceived(object sender, PacketReceivedEventArgs e)
        {
            if (e.OldPacket != null && e.NewPacket != null)
            {
                var telemetryPacket = e.NewPacket.RawPacket;

                TelemetryPackets.Add(telemetryPacket);

                var oldX = e.OldPacket.RawPacket.X;
                var newX = e.NewPacket.RawPacket.X;
                var oldY = e.OldPacket.RawPacket.Z;
                var newY = e.NewPacket.RawPacket.Z;

                MapWindow.DrawLine(new Point(newX, newY), Brushes.Blue);

                ViewModel.CurrentLapTime = e.NewPacket.CurrentLapTime;

                ViewModel.Sector = e.NewPacket.CurrentSectorIndex;

                //if (e.NewPacket.CurrentSectorIndex != e.OldPacket.CurrentSectorIndex)
                //{
                //    ViewModel.SectorTimes[e.OldPacket.CurrentSectorIndex] = e.NewPacket.CurrentLapTime - ViewModel.SectorTimes[ViewModel.Sector - 1];
                //}

                if (Math.Abs(e.NewPacket.SpeedKmh - e.OldPacket.SpeedKmh) > 0.1f)
                {
                    ViewModel.SeriesCollection[0].Values.Add(new DateModel(e.NewPacket.CurrentLapTime, e.OldPacket.SpeedKmh));
                }

                if (Math.Abs(e.NewPacket.Rpms - e.OldPacket.Rpms) > 1f)
                {
                    ViewModel.SeriesCollection[1].Values.Add(new DateModel(e.NewPacket.CurrentLapTime, e.NewPacket.Rpms));
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
                    ViewModel.SeriesCollection[2].Values.Add(new DateModel(e.NewPacket.CurrentLapTime, newGear));
                }
            }
        }

        private void ResetZoomOnClick(object sender, RoutedEventArgs e)
        {
            X.MinValue = double.NaN;
            X.MaxValue = double.NaN;
            Y.MinValue = 0;
            Y.MaxValue = 360;
            Y1.MinValue = 3000;
            Y1.MaxValue = 14000;
            Y2.MinValue = 0;
            Y2.MaxValue = 8;
        }

        private void InitGraph()
        {
            var dayConfig = Mappers.Xy<DateModel>()
              .X(dateModel =>
              {
                  return dateModel.TimeSpan.TotalMilliseconds / TimeSpan.FromMilliseconds(1).TotalMilliseconds;
              })
              .Y(dateModel =>
              {
                  return dateModel.Value;
              });

            ViewModel.SeriesCollection = new SeriesCollection(dayConfig)
            {
                new LineSeries
                {
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.CornflowerBlue,
                    PointGeometrySize = 0,
                    Values = new ChartValues<DateModel>() { },
                    ScalesYAt = 0,
                },
                new LineSeries
                {
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Green,
                    PointGeometrySize = 0,
                    Values = new ChartValues<DateModel>() { },
                    ScalesYAt = 1,
                },
                new StepLineSeries
                {
                    StrokeThickness = 1,
                    Fill = Brushes.Transparent,
                    Stroke = Brushes.Red,
                    PointGeometrySize = 0,
                    AlternativeStroke = Brushes.Red,
                    Values = new ChartValues<DateModel>() { },
                    ScalesYAt = 2,
                },
            };

            ViewModel.Formatter = value =>
            {
                var x = value > 0 ? value : 0;

                return new DateTime((long)(x * TimeSpan.FromMilliseconds(1).Ticks)).ToString("mm\\:ss\\:fff");
            };
        }

    }
}
