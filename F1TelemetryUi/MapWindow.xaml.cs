using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;
using F1Telemetry.Models.Raw;

namespace F1TelemetryUi
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? Brushes.Red : Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapWindow : Window, INotifyPropertyChanged
    {
        public int RotationDegree { get; set; } = 50;
        public int YOffset { get; set; } = 582;
        public int XOffset { get; set; } = 563;
        public double Scale { get; set; } = 0.85066437875896184;
        public Dictionary<int, Tuple<Point, Point>> ReferencePoints { get; set; } = new Dictionary<int, Tuple<Point, Point>>();
        public int CurrentValue { get; set; } = 1;
        public Point LastClickedPosition { get; set; }
        public Point LastDrawnPoint { get; set; }
        PointCollection points;

        public PointCollection Points
        {
            get
            {
                return points;
            }

            set
            {
                points = value;
                OnPropertyChanged();
            }
        }

        private bool referencingMode;

        public bool ReferencingMode
        {
            get
            {
                return referencingMode;
            }
            set
            {
                referencingMode = value;
                OnPropertyChanged();
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private readonly MainWindow _owner;

        public event PropertyChangedEventHandler PropertyChanged;

        public MapWindow(MainWindow owner)
        {
            InitializeComponent();
            _owner = owner;

            DrawLatestTelemetry();
            DataContext = this;
        }

        private Point Rotate(Point p, float theta)
        {
            int rx = System.Convert.ToInt32((p.X * Math.Cos(theta)) - (p.Y * Math.Sin(theta)));
            int ry = System.Convert.ToInt32((p.Y * Math.Cos(theta)) + (p.X * Math.Sin(theta)));
            return new Point(rx, ry);
        }

        public void DrawLine(Point point, SolidColorBrush brush)
        {
            this.Dispatcher.Invoke(() =>
            {
                if(LastDrawnPoint == new Point())
                {
                    LastDrawnPoint = point;
                    return;
                }

                var point1 = Rotate(LastDrawnPoint, (float)(Math.PI * RotationDegree / 180));
                var point2 = Rotate(point, (float)(Math.PI * RotationDegree / 180));

                point1.Offset(XOffset, YOffset);
                point2.Offset(XOffset, YOffset);

                MapCanvas.Children.Add(new Line()
                {
                    X1 = point1.X * Scale,
                    Y1 = point1.Y * Scale,
                    X2 = point2.X * Scale,
                    Y2 = point2.Y * Scale,
                    Stroke = brush,
                    StrokeThickness = 2,
                });

                LastDrawnPoint = new Point();
            });

        }

        private void zoomBorder_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.F)
            {
                zoomBorder.Fill();
            }

            if (e.Key == Key.U)
            {
                zoomBorder.Uniform();
            }

            if (e.Key == Key.R)
            {
                zoomBorder.Reset();
            }

            if (e.Key == Key.T)
            {
                zoomBorder.ToggleStretchMode();
                zoomBorder.AutoFit();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RotationDegree += 2;
            DrawLatestTelemetry();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            RotationDegree -= 2;
            DrawLatestTelemetry();
        }

        public void DrawLatestTelemetry()
        {
            var latestTelemetry = GetLatestData();
            var nextTelemetry = latestTelemetry.Skip(1).Take(1);

            F12017TelemetryPacket oldPacket = latestTelemetry.First();
            F12017TelemetryPacket newPacket;

            int i = 0;

            MapCanvas.Children.Clear();

            foreach (var item in latestTelemetry.Skip(1))
            {
                newPacket = item;

                DrawLine(new Point(newPacket.X, newPacket.Z), Brushes.IndianRed);

                oldPacket = newPacket;
                i++;
            }
        }

        private static List<F12017TelemetryPacket> GetLatestData()
        {
            FileStream FileStream = File.Open(@"D:\\temp\\laphungaro.xml", FileMode.Open);
            var XmlSerializer = new XmlSerializer(typeof(List<F12017TelemetryPacket>));
            var latestData = (List<F12017TelemetryPacket>)XmlSerializer.Deserialize(FileStream);
            FileStream.Close();
            return latestData;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Scale += .0005;
            DrawLatestTelemetry();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Scale -= .0005;
            DrawLatestTelemetry();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            XOffset -= 1;
            DrawLatestTelemetry();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            XOffset += 1;
            DrawLatestTelemetry();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            YOffset -= 1;
            DrawLatestTelemetry();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            YOffset += 1;
            DrawLatestTelemetry();
        }

        private void MapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!ReferencingMode)
            {
                return;
            }

            var mousePosition = Mouse.GetPosition(MapCanvas);

            if (CurrentValue % 2 == 0)
            {
                ReferencePoints.Add(CurrentValue, Tuple.Create(LastClickedPosition, mousePosition));
                CurrentValue++;
                if (CurrentValue == 4)
                {
                    var v1 = (ReferencePoints[2].Item1 - ReferencePoints[2].Item2).Length;
                    var v2 = (ReferencePoints[4].Item1 - ReferencePoints[4].Item2).Length;

                    var difference = ((v2 - v1) / Math.Abs(v1)) * 100;

                    var x = Math.Abs((float)5 - (float)7);
                    var y = (float)5 + (float)7;
                    var scale = (((float)y / 2) / (float)x) * 100;
                    Scale -= 0.0260561;
                    Scale += 0.6339439;
                    //var scale = ((ReferencePoints[4].Item1 - ReferencePoints[4].Item2).Length / 100) * baseLength;
                }
                LastClickedPosition = new Point();
                return;
            }

            LastClickedPosition = mousePosition;
            CurrentValue++;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            ReferencingMode = !ReferencingMode;
        }
    }
}
