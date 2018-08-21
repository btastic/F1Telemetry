using System;

namespace F1TelemetryUi.ViewModels
{
    public class TimeSpanValue
    {
        public TimeSpan TimeSpan { get; set; }
        public double Value { get; set; }

        public TimeSpanValue(TimeSpan timeSpan, double value)
        {
            TimeSpan = timeSpan;
            Value = value;
        }
    }
}
