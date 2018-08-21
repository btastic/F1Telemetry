using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Caliburn.Micro;
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
            }
        }

        private int _angle = 50;
        public int Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
                NotifyOfPropertyChange();
            }
        }

        private int _yOffset = 581;
        public int YOffset
        {
            get
            {
                return _yOffset;
            }
            set
            {
                _yOffset = value;
                NotifyOfPropertyChange();
            }
        }

        private int _xOffset = 563;
        public int XOffset
        {
            get
            {
                return _xOffset;
            }
            set
            {
                _xOffset = value;
                NotifyOfPropertyChange();
            }
        }

        private double _scale = 0.4252238;
        public double Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                NotifyOfPropertyChange();
            }
        }

        public MapView View { get; private set; }

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
        }

        public void IncreaseYOffset()
        {
            YOffset += 1;
        }

        public void DecreaseYOffset()
        {
            YOffset -= 1;
        }

        public void IncreaseXOffset()
        {
            XOffset += 1;
        }

        public void DecreaseXOffset()
        {
            YOffset -= 1;
        }

        public void IncreaseScale()
        {
            Scale += .0005;
        }

        public void DecreaseScale()
        {
            Scale -= .0005;
        }

        public void IncreaseAngle()
        {
            Angle += 2;
        }

        public void DecreaseAngle()
        {
            Angle -= 2;
        }

        public void ToggleReferencing()
        {
            Referencing = !Referencing;
        }

        public void Handle(DrawEvent message)
        {
            Point point = message.Point;

            point = MapCanvas.TranslatePoint(point, MapCanvas);

            Point point1 = point.Rotate((float)(Math.PI * Angle / 180));

            point1.Offset(XOffset, YOffset);
            point1.X = point1.X * Scale;
            point1.Y = point1.Y * Scale;

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
