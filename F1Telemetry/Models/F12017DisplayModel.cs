using System;
using F1Telemetry.Models.Raw;

namespace F1Telemetry.Models
{
    public class F12017DisplayModel
    {
        public TimeSpan LapTime { get; }
        public F12017TelemetryPacket RawPacket { get; }
        public int CurrentLap { get; }
        public TimeSpan CurrentLapTime { get; }
        public int CurrentSectorIndex { get; }
        public float SpeedKmh { get; }
        public float Rpms { get; }
        public string Gear { get; }

        public F12017DisplayModel(F12017TelemetryPacket telemetryPacket)
        {
            RawPacket = telemetryPacket;
            LapTime = TimeSpan.FromSeconds(telemetryPacket.LapTime);
            CurrentLap = Convert.ToUInt16(telemetryPacket.Lap);
            CurrentLapTime = TimeSpan.FromSeconds(telemetryPacket.LapTime);
            CurrentSectorIndex = Convert.ToUInt16(telemetryPacket.Sector);
            SpeedKmh = telemetryPacket.Speed * 3.6f;
            Rpms = telemetryPacket.EngineRate;
            Gear = GetGear(Convert.ToUInt16(telemetryPacket.Gear));
        }

        private string GetGear(uint gear)
        {
            if (gear == 0)
            {
                return "R";
            }
            if (gear == 1)
            {
                return "N";
            }

            return (gear - 1).ToString();
        }
    }
}
