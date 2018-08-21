using System;
using System.Windows;

namespace F1TelemetryUi.Extensions
{
    public static class PointExtensions
    {
        public static Point Rotate(this Point point, double theta)
        {
            double newX = (point.X * Math.Cos(theta)) - (point.Y * Math.Sin(theta));
            double newY = (point.Y * Math.Cos(theta)) + (point.X * Math.Sin(theta));
            return new Point(newX, newY);
        }
    }
}
