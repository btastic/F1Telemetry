﻿using System.Runtime.InteropServices;

namespace F1Telemetry.Models.Raw.F12018
{
    /// <summary>
    /// Frequency: 2 per second
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarStatusData
    {
        /// <summary>
        /// Header
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public PacketHeader Header;

        /// <summary>
        /// List of CarStatusData
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CarStatusData[] CarStatusData;
    }
}