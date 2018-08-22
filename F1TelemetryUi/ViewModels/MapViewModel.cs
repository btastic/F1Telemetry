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
using F1TelemetryUi.Referencing;
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
        private readonly IEventAggregator _eventAggregator;
        private readonly ReferencingStateMachine _referencingStateMachine;
        private readonly IWindowManager _windowManager;
        private Canvas _mapCanvas;
        private PointCollection _points = new PointCollection();

        private bool _referencing;

        private Track _track = Track.Tracks.FirstOrDefault(x => x.Name == "Silverstone");

        public MapViewModel(
            IWindowManager windowManager,
            IEventAggregator eventAggregator,
            ReferencingStateMachine referencingStateMachine)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.Subscribe(this);

            _referencingStateMachine = referencingStateMachine;
            _referencingStateMachine.StateChanged += _referencingStateMachine_StateChanged;

            ViewAttached += MapViewModel_ViewAttached;
        }

        public event EventHandler<ViewAttachedEventArgs> ViewAttached = delegate { };

        public Point FirstReferencePoint { get; set; }

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

        public bool ReferenceIsVisible
        {
            get
            {
                return _referencingStateMachine.CurrentState != ReferencingState.Disabled;
            }
        }

        public Tuple<Point, Point> ReferencePointsLine { get; set; }

        public Tuple<Point, Point> ReferencePointsMap { get; set; }

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

        public ReferencingState ReferencingState
        {
            get
            {
                return _referencingStateMachine.CurrentState;
            }
        }

        public string ReferencingString
        {
            get
            {
                switch (_referencingStateMachine.CurrentState)
                {
                    case ReferencingState.Disabled:
                        return string.Empty;

                    case ReferencingState.TakingFirstPoint:
                        return "Bitte einen Referenzpunkt auf der Linie klicken";

                    case ReferencingState.TakingSecondPoint:
                        return "Bitte einen 2. Punkt auf der Linie klicken";

                    case ReferencingState.TakingThirdPoint:
                        return "Bitte einen Referenzpunkt auf der Karte klicken";

                    case ReferencingState.TakingFourthPoint:
                        return "Bitte einen 2. Punkt auf der Karte klicken";
                }

                return string.Empty;
            }
        }

        public Point SecondReferencePoint { get; set; }

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

        public void DecreaseAngle()
        {
            Track.Angle -= 2;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void DecreaseScale()
        {
            Track.Scale -= .0005;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void DecreaseXOffset()
        {
            Track.YOffset -= 1;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void DecreaseYOffset()
        {
            Track.YOffset -= 1;
            NotifyOfPropertyChange(() => Track);
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

        public object GetView(object context = null)
        {
            return View;
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

        public void IncreaseAngle()
        {
            Track.Angle += 2;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void IncreaseScale()
        {
            Track.Scale += .0005;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void IncreaseXOffset()
        {
            Track.XOffset += 1;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void IncreaseYOffset()
        {
            Track.YOffset += 1;
            NotifyOfPropertyChange(() => Track);
            DrawLatestTelemetry();
        }

        public void MapClick(MouseButtonEventArgs e)
        {
            Point mousePosition = Mouse.GetPosition(MapCanvas);

            switch (_referencingStateMachine.CurrentState)
            {
                case ReferencingState.Disabled:
                    return;

                case ReferencingState.TakingFirstPoint:
                    FirstReferencePoint = mousePosition;
                    MapCanvas.Children.Add(GetEllipseAtPoint(mousePosition));
                    _referencingStateMachine.Next();
                    return;

                case ReferencingState.TakingSecondPoint:
                    SecondReferencePoint = mousePosition;
                    MapCanvas.Children.Add(GetEllipseAtPoint(mousePosition));
                    ReferencePointsLine = Tuple.Create(FirstReferencePoint, SecondReferencePoint);
                    _referencingStateMachine.Next();
                    return;

                case ReferencingState.TakingThirdPoint:
                    FirstReferencePoint = mousePosition;
                    MapCanvas.Children.Add(GetEllipseAtPoint(mousePosition));
                    _referencingStateMachine.Next();
                    return;

                case ReferencingState.TakingFourthPoint:
                    SecondReferencePoint = mousePosition;
                    MapCanvas.Children.Add(GetEllipseAtPoint(mousePosition));
                    ReferencePointsMap = Tuple.Create(FirstReferencePoint, SecondReferencePoint);
                    _referencingStateMachine.Disable();
                    NotifyOfPropertyChange(() => Referencing);
                    return;
            }
        }

        public void ToggleReferencing()
        {
            _referencingStateMachine.Toggle();
            NotifyOfPropertyChange(() => Referencing);
        }

        private static List<F12017TelemetryPacket> GetLatestData()
        {
            FileStream FileStream = File.Open(@"C:\development\F1Telemetry\F1TelemetryUi\Resources\silverstone.xml", FileMode.Open);
            var XmlSerializer = new XmlSerializer(typeof(List<F12017TelemetryPacket>));
            var latestData = (List<F12017TelemetryPacket>)XmlSerializer.Deserialize(FileStream);
            FileStream.Close();
            return latestData;
        }

        private void _referencingStateMachine_StateChanged(object sender, ReferencingStateChangedArgs e)
        {
            NotifyOfPropertyChange(() => ReferencingState);
            NotifyOfPropertyChange(() => Referencing);
            NotifyOfPropertyChange(() => ReferencingString);
            NotifyOfPropertyChange(() => ReferenceIsVisible);
        }

        private Ellipse GetEllipseAtPoint(Point point)
        {
            var ellipse = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Black,
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetTop(ellipse, point.Y - ellipse.Height / 2);

            return ellipse;
        }

        private void MapViewModel_ViewAttached(object sender, ViewAttachedEventArgs e)
        {
            //DrawLatestTelemetry();
        }
    }
}
