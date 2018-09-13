using System;

namespace F1Telemetry.Events
{
    public class NewLapEventArgs : EventArgs
    {
        public int LastLap { get; }
        public int CurrentLap { get; }

        public NewLapEventArgs(int lastLap, int currentLap)
        {
            CurrentLap = currentLap;
            LastLap = lastLap;
        }
    }
}
