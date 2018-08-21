using System;

namespace F1TelemetryUi
{
    public class DateModel
    {
        public TimeSpan TimeSpan { get; set; }
        public double Value { get; set; }

        public DateModel(TimeSpan timeSpan, double value)
        {
            TimeSpan = timeSpan;
            Value = value;
        }
    }
}
