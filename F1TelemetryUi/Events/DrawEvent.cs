using System.Windows;
using System.Windows.Media;

namespace F1TelemetryUi.Events
{
    public class DrawEvent
    {
        public DrawEvent(Point point, Brush brush)
        {
            Point = point;
            Brush = brush;
        }

        public Point Point { get; }
        public Brush Brush { get; }
    }
}
