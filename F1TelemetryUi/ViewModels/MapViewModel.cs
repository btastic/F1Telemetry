using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Serialization;
using Caliburn.Micro;
using F1Telemetry.Models;
using F1Telemetry.Models.Raw;
using F1TelemetryUi.Events;
using F1TelemetryUi.Extensions;
using F1TelemetryUi.Views;
using MahApps.Metro.Controls;

namespace F1TelemetryUi.ViewModels
{
    [Export(typeof(MapViewModel))]
    public class MapViewModel :
        PropertyChangedBase,
        IShell,
        IHandle<DrawEvent>,
        IHandle<ClearCanvasEvent>,
        IViewAware
    {
        private Canvas _mapCanvas;
        public Canvas MapCanvas
        {
            get
            {
                return _mapCanvas;
            }
            set
            {
                _mapCanvas = value;
                NotifyOfPropertyChange();
            }
        }

        private PointCollection _points = new PointCollection();
        public PointCollection Points
        {
            get
            {
                return _points;
            }

            set
            {
                _points = value;
                NotifyOfPropertyChange();
            }
        }

        private bool _referencing;
        public bool Referencing
        {
            get
            {
                return _referencing;
            }
            set
            {
                _referencing = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => ReferenceIsVisible);
                NotifyOfPropertyChange(() => ReferencingString);
            }
        }

        public Point FirstReferencePoint { get; set; }
        public Point SecondReferencePoint { get; set; }

        public Tuple<Point, Point> ReferencePointsLine { get; set; }
        public Tuple<Point, Point> ReferencePointsMap { get; set; }

        private Track _track = Track.Tracks.FirstOrDefault(x => x.Name == "Silverstone");
        public Track Track
        {
            get
            {
                return _track;
            }
            set
            {
                _track = value;
                NotifyOfPropertyChange();
            }
        }

        public MapView View { get; private set; }

        public string ReferencingString
        {
            get
            {
                if (ReferencePointsLine == null)
                {
                    return "Bitte einen Referenzpunkt auf der Linie klicken";
                }

                if (FirstReferencePoint != new Point())
                {
                    return "Bitte einen 2. Punkt auf der Linie klicken";
                }

                if (ReferencePointsMap == null)
                {
                    return "Bitte einen Referenzpunkt auf der Karte klicken";
                }

                return "";
            }
        }

        public bool ReferenceIsVisible
        {
            get
            {
                return Referencing;
            }
        }

        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        public event EventHandler<ViewAttachedEventArgs> ViewAttached = delegate { };

        public MapViewModel(
            IWindowManager windowManager,
            IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;

            _eventAggregator.Subscribe(this);

            ViewAttached += MapViewModel_ViewAttached;
        }

        private void MapViewModel_ViewAttached(object sender, ViewAttachedEventArgs e)
        {
            DrawLatestTelemetry();
        }

        public void IncreaseYOffset()
        {
            Track.YOffset += 1;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void DecreaseYOffset()
        {
            Track.YOffset -= 1;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void IncreaseXOffset()
        {
            Track.XOffset += 1;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void DecreaseXOffset()
        {
            Track.YOffset -= 1;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void IncreaseScale()
        {
            Track.Scale += .0005;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void DecreaseScale()
        {
            Track.Scale -= .0005;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void IncreaseAngle()
        {
            Track.Angle += 2;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void DecreaseAngle()
        {
            Track.Angle -= 2;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void ToggleReferencing()
        {
            Referencing = !Referencing;
            DrawLatestTelemetry();
        }

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

                Handle(new DrawEvent(new Point(newPacket.X, newPacket.Z), Brushes.IndianRed));

                oldPacket = newPacket;
                i++;
            }
        }

        private static List<F12017TelemetryPacket> GetLatestData()
        {
            FileStream FileStream = File.Open(@"D:\\temp\\laphsilver.xml", FileMode.Open);
            var XmlSerializer = new XmlSerializer(typeof(List<F12017TelemetryPacket>));
            var latestData = (List<F12017TelemetryPacket>)XmlSerializer.Deserialize(FileStream);
            FileStream.Close();
            return latestData;
        }

        private Ellipse GetEllipseAtPoint(Point point)
        {            
            var ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Black,                
            };

            Canvas.SetLeft(ellipse, point.X);
            Canvas.SetTop(ellipse, point.Y);

            return ellipse;
        }

        public void MapClick(MouseButtonEventArgs e)
        {
            if(!Referencing)
            {
                return;
            }

            Point mousePosition = Mouse.GetPosition(MapCanvas);

            if (FirstReferencePoint == new Point())
            {
                FirstReferencePoint = mousePosition;
                MapCanvas.Children.Add(GetEllipseAtPoint(mousePosition));
                NotifyOfPropertyChange(() => ReferencingString);
                return;
            }

            if (SecondReferencePoint == new Point())
            {
                SecondReferencePoint = mousePosition;
                MapCanvas.Children.Add(GetEllipseAtPoint(mousePosition));
                NotifyOfPropertyChange(() => ReferencingString);
                return;
            }

            if (ReferencePointsLine == null)
            {
                ReferencePointsLine = Tuple.Create(FirstReferencePoint, SecondReferencePoint);
                FirstReferencePoint = new Point();
                SecondReferencePoint = new Point();
                NotifyOfPropertyChange(() => ReferencingString);
                return;
            }

            if (ReferencePointsMap == null)
            {
                ReferencePointsMap = Tuple.Create(FirstReferencePoint, SecondReferencePoint);
                FirstReferencePoint = new Point();
                SecondReferencePoint = new Point();
                NotifyOfPropertyChange(() => ReferencingString);
                return;
            }

            if (ReferencePointsLine != null && ReferencePointsMap != null)
            {
                ;
            }
        }

        public void Handle(DrawEvent message)
        {
            Point point = message.Point;

            point = MapCanvas.TranslatePoint(point, MapCanvas);

            Point point1 = point.Rotate((float)(Math.PI * Track.Angle / 180));

            point1.Offset(Track.XOffset, Track.YOffset);
            point1.X = point1.X * Track.Scale;
            point1.Y = point1.Y * Track.Scale;

            var pc = new PointCollection(_points)
            {
                new Point(point1.X, point1.Y)
            };

            _points = pc;
            NotifyOfPropertyChange(() => Points);
        }

        public void Handle(ClearCanvasEvent message)
        {
            Points = new PointCollection();
        }

        public void AttachView(object view, object context = null)
        {
            if (view is MapView mapView)
            {
                View = mapView;

                Canvas canvas = mapView.FindChildren<Canvas>().FirstOrDefault();
                if (canvas != null)
                {
                    MapCanvas = canvas;
                }

                ViewAttached(this, new ViewAttachedEventArgs { Context = context, View = mapView });
            }
        }

        public object GetView(object context = null)
        {
            return View;
        }
    }
}
