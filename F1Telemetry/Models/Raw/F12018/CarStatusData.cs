using System.Runtime.InteropServices;

namespace F1Telemetry.Models.Raw.F12018
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct CarStatusData
    {
        /// <summary>
        /// 0 (off) - 1 (medium) - 2 (high)
        /// </summary>
        public TractionControl TractionControl;

        /// <summary>
        /// 0 (off) - 1 (on)
        /// </summary>
        public byte AntiLockBrakes;

        /// <summary>
        /// Fuel mix - 0 = lean, 1 = standard, 2 = rich, 3 = max
        /// </summary>
        public FuelMix FuelMix;

        /// <summary>
        /// Front brake bias (percentage)
        /// </summary>
        public byte FrontBrakeBias;

        /// <summary>
        /// Pit limiter status - 0 = off, 1 = on
        /// </summary>
        public byte PitLimiterStatus;

        /// <summary>
        /// Current fuel mass
        /// </summary>
        public float FuelInTank;

        /// <summary>
        /// Fuel capacity
        /// </summary>
        public float FuelCapacity;

        /// <summary>
        /// Cars max RPM, point of rev limiter
        /// </summary>
        public ushort MaxRpm;

        /// <summary>
        /// Cars idle RPM
        /// </summary>
        public ushort IdleRpm;

        /// <summary>
        /// Maximum number of gears
        /// </summary>
        public byte MaxGears;

        /// <summary>
        /// 0 = not allowed, 1 = allowed, -1 = unknown
        /// </summary>
        public byte DRSAllowed;

        /// <summary>
        /// Tyre wear percentage
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] TyresWear;

        /// <summary>
        /// Modern - 0 = hyper soft, 1 = ultra soft
        /// 2 = super soft, 3 = soft, 4 = medium, 5 = hard
        /// 6 = super hard, 7 = inter, 8 = wet
        ///
        /// Classic - 0-6 = dry, 7-8 = wet
        /// </summary>
        public byte TyreCompound;

        /// <summary>
        /// Tyre damage (percentage)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] TyresDamage;

        /// <summary>
        /// Front left wing damage (percentage)
        /// </summary>
        public byte FrontLeftWingDamage;

        /// <summary>
        /// Front right wing damage (percentage)
        /// </summary>
        public byte FrontRightWingDamage;

        /// <summary>
        /// Rear wing damage (percentage)
        /// </summary>
        public byte RearWingDamage;

        /// <summary>
        /// Engine damage (percentage)
        /// </summary>
        public byte EngineDamage;

        /// <summary>
        /// Gear box damage (percentage)
        /// </summary>
        public byte GearBoxDamage;

        /// <summary>
        /// Exhaust damage (percentage)
        /// </summary>
        public byte ExhaustDamage;

        /// <summary>
        /// -1 = invalid/unknown, 0 = none, 1 = green
        /// 2 = blue, 3 = yellow, 4 = red
        /// </summary>
        public Flag VehicleFiaFlags;

        /// <summary>
        /// ERS energy store in Joules
        /// </summary>
        public float ERSStoreEnergy;

        /// <summary>
        /// ERS deployment mode, 0 = none, 1 = low, 2 = medium
        /// 3 = high, 4 = overtake, 5 = hotlap
        /// </summary>
        public ERSDeployMode ERSDeployMode;

        /// <summary>
        /// ERS energy harvested this lap by MGU-K
        /// </summary>
        public float ERSHarvestedThisLapMGUK;

        /// <summary>
        /// ERS energy harvested this lap by MGU-H
        /// </summary>
        public float ERSHarvestedThisLapMGUH;

        /// <summary>
        /// ERS energy deployed this lap
        /// </summary>
        public float ERSDeployedThisLap;
    }
}
