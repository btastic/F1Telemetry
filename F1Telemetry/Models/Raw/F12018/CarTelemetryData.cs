using System.Runtime.InteropServices;

namespace F1Telemetry.Models.Raw.F12018
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarTelemetryData
    {
        /// <summary>
        /// Speed of car in kilometres per hour
        /// </summary>
        public ushort Speed;

        /// <summary>
        /// Amount of throttle applied (0 to 100)
        /// </summary>
        public byte Throttle;

        /// <summary>
        /// Steering (-100 (full lock left) to 100 (full lock right))
        /// </summary>
        public sbyte Steer;

        /// <summary>
        /// Amount of brake applied (0 to 100)
        /// </summary>
        public byte Brake;

        /// <summary>
        /// Amount of clutch applied (0 to 100)
        /// </summary>
        public byte Clutch;

        /// <summary>
        /// Gear selected (1-8, N=0, R=-1)
        /// </summary>
        public Gear Gear;

        /// <summary>
        /// Engine RPM
        /// </summary>
        public ushort EngineRpm;

        /// <summary>
        /// 0 = off, 1 = on
        /// </summary>
        public byte DRS;

        /// <summary>
        /// Rev lights indicator (percentage)
        /// </summary>
        public byte RevLightsPercent;

        /// <summary>
        /// Brakes temperature (celsius)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] BrakesTemperature;

        /// <summary>
        /// Tyres surface temperature (celsius)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] TyresSurfaceTemperature;

        /// <summary>
        /// Tyres inner temperature (celsius)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] TyresInnerTemperature;

        /// <summary>
        /// Engine temperature (celsius)
        /// </summary>
        public ushort EngineTemperature;

        /// <summary>
        /// Tyres pressure (PSI)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] TyresPressure;
    }
}
