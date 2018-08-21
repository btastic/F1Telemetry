using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using Caliburn.Micro;
using F1Telemetry.Models.Raw;
using MahApps.Metro.Controls;

namespace F1TelemetryUi.Views
{
    /// <summary>
    /// Interaction logic for MapWindow.xaml
    /// </summary>
    public partial class MapView : MetroWindow
    {
        public Dictionary<int, Tuple<Point, Point>> ReferencePoints { get; set; } = new Dictionary<int, Tuple<Point, Point>>();
        public int CurrentValue { get; set; } = 1;
        public Point LastClickedPosition { get; set; }
        public Point LastDrawnPoint { get; set; }

        public MapView()
        {
            InitializeComponent();
        }

        public void ClearLine()
        {
            //this.Dispatcher.Invoke(() =>
            //{
            //    Points = new PointCollection();
            //});
        }

        //public void DrawLine(Point point, SolidColorBrush brush)
        //{
            //this.Dispatcher.Invoke(() =>
            //{
            //    point = MapCanvas.TranslatePoint(point, MapCanvas);

            //    var point1 = point.Rotate((float)(Math.PI * RotationDegree / 180));

            //    point1.Offset(XOffset, YOffset);
            //    point1.X = point1.X * Scale;
            //    point1.Y = point1.Y * Scale;

            //    //MapCanvas.Children.Add(new Line()
            //    //{
            //    //    X1 = point1.X * Scale,
            //    //    Y1 = point1.Y * Scale,
            //    //    X2 = point2.X * Scale,
            //    //    Y2 = point2.Y * Scale,
            //    //    Stroke = brush,
            //    //    StrokeThickness = 2,
            //    //});

            //    PointCollection pc = new PointCollection(points)
            //    {
            //        new Point(point1.X, point1.Y)
            //    };

            //    points = pc;
            //    OnPropertyChanged("Points");
            //});
        //}

        //private void zoomBorder_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        //{
        //    if (e.Key == Key.F)
        //    {
        //        zoomBorder.Fill();
        //    }

        //    if (e.Key == Key.U)
        //    {
        //        zoomBorder.Uniform();
        //    }

        //    if (e.Key == Key.R)
        //    {
        //        zoomBorder.Reset();
        //    }

        //    if (e.Key == Key.T)
        //    {
        //        zoomBorder.ToggleStretchMode();
        //        zoomBorder.AutoFit();
        //    }
        //}

        //private void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    RotationDegree += 2;
        //    DrawLatestTelemetry();
        //}

        //private void Button_Click_1(object sender, RoutedEventArgs e)
        //{
        //    RotationDegree -= 2;
        //    DrawLatestTelemetry();
        //}

        public void DrawLatestTelemetry()
        {
            List<F12017TelemetryPacket> latestTelemetry = GetLatestData();
            IEnumerable<F12017TelemetryPacket> nextTelemetry = latestTelemetry.Skip(1).Take(1);

            F12017TelemetryPacket oldPacket = latestTelemetry.First();
            F12017TelemetryPacket newPacket;

            int i = 0;

            //MapCanvas.Children.Clear();

            foreach (F12017TelemetryPacket item in latestTelemetry.Skip(1))
            {
                newPacket = item;

                //DrawLine(new Point(newPacket.X, newPacket.Z), Brushes.IndianRed);

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

        //private void Button_Click_2(object sender, RoutedEventArgs e)
        //{
        //    Scale += .0005;
        //    DrawLatestTelemetry();
        //}

        //private void Button_Click_3(object sender, RoutedEventArgs e)
        //{
        //    Scale -= .0005;
        //    DrawLatestTelemetry();
        //}

        //private void Button_Click_4(object sender, RoutedEventArgs e)
        //{
        //    XOffset -= 1;
        //    DrawLatestTelemetry();
        //}

        //private void Button_Click_5(object sender, RoutedEventArgs e)
        //{
        //    XOffset += 1;
        //    DrawLatestTelemetry();
        //}

        //private void Button_Click_6(object sender, RoutedEventArgs e)
        //{
        //    YOffset -= 1;
        //    DrawLatestTelemetry();
        //}

        //private void Button_Click_7(object sender, RoutedEventArgs e)
        //{
        //    YOffset += 1;
        //    DrawLatestTelemetry();
        //}

        //private void MapCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //if (!ReferencingMode)
        //{
        //    return;
        //}

        //var mousePosition = Mouse.GetPosition(MapCanvas);

        //if (CurrentValue % 2 == 0)
        //{
        //    ReferencePoints.Add(CurrentValue, Tuple.Create(LastClickedPosition, mousePosition));
        //    CurrentValue++;
        //    if (CurrentValue == 4)
        //    {
        //        var v1 = (ReferencePoints[2].Item1 - ReferencePoints[2].Item2).Length;
        //        var v2 = (ReferencePoints[4].Item1 - ReferencePoints[4].Item2).Length;

        //        var difference = ((v2 - v1) / Math.Abs(v1)) * 100;
        //    }
        //    LastClickedPosition = new Point();
        //    return;
        //}

        //LastClickedPosition = mousePosition;
        //CurrentValue++;
        //}

        //private void Button_Click_8(object sender, RoutedEventArgs e)
        //{
        //ReferencingMode = !ReferencingMode;
        //}
    }
}
