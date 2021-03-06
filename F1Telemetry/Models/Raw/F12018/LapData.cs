﻿using System.Runtime.InteropServices;
using MessagePack;

namespace F1Telemetry.Models.Raw.F12018
{
    [MessagePackObject(keyAsPropertyName: true)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LapData
    {
        /// <summary>
        /// Last lap time in seconds
        /// </summary>
        public float LastLapTime;

        /// <summary>
        /// Current time around the lap in seconds
        /// </summary>
        public float CurrentLapTime;

        /// <summary>
        /// Best lap time of the session in seconds
        /// </summary>
        public float BestLapTime;

        /// <summary>
        /// Sector 1 time in seconds
        /// </summary>
        public float Sector1Time;

        /// <summary>
        /// Sector 2 time in seconds
        /// </summary>
        public float Sector2Time;

        /// <summary>
        /// Distance vehicle is around current lap in metres – could
        /// be negative if line hasn’t been crossed yet
        /// </summary>
        public float LapDistance;

        /// <summary>
        /// Total distance travelled in session in metres – could
        /// be negative if line hasn’t been crossed yet
        /// </summary>
        public float TotalDistance;

        /// <summary>
        /// Delta in seconds for safety car
        /// </summary>
        public float SafetyCarDelta;

        /// <summary>
        /// Car race position
        /// </summary>
        public byte CarPosition;

        /// <summary>
        /// Current lap number
        /// </summary>
        public byte CurrentLapNum;

        /// <summary>
        /// 0 = none, 1 = pitting, 2 = in pit area
        /// </summary>
        public PitStatus PitStatus;

        /// <summary>
        /// 0 = sector1, 1 = sector2, 2 = sector3
        /// </summary>
        public Sector Sector;

        /// <summary>
        /// Current lap invalid - 0 = valid, 1 = invalid
        /// </summary>
        public byte CurrentLapInvalid;

        /// <summary>
        /// Accumulated time penalties in seconds to be added
        /// </summary>
        public byte Penalties;

        /// <summary>
        /// Grid position the vehicle started the race in
        /// </summary>
        public byte GridPosition;

        /// <summary>
        /// Status of driver - 0 = in garage, 1 = flying lap
        /// 2 = in lap, 3 = out lap, 4 = on track
        /// </summary>
        public DriverStatus DriverStatus;

        /// <summary>
        /// Result status - 0 = invalid, 1 = inactive, 2 = active
        /// 3 = finished, 4 = disqualified, 5 = not classified
        /// 6 = retired
        /// </summary>
        public ResultStatus ResultStatus;
    }
}
