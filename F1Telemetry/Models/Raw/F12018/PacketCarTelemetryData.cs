using System;
using System.Runtime.InteropServices;

namespace F1Telemetry.Models.Raw.F12018
{
    /// <summary>
    /// Frequency: Rate as specified in menus
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarTelemetryData
    {
        /// <summary>
        /// Header
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public PacketHeader Header;

        /// <summary>
        /// List of CarTelemetryData
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CarTelemetryData[] CarTelemetryData;

        /// <summary>
        /// Bit flags specifying which buttons are being
        /// pressed currently - see appendices
        /// http://forums.codemasters.com/discussion/136948/f1-2018-udp-specification
        /// </summary>
        public UInt32 ButtonStatus;
    }
}
