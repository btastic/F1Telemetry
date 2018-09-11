using System.Runtime.InteropServices;
using MessagePack;

namespace F1Telemetry.Models.Raw.F12018
{
    /// <summary>
    /// Frequency: Rate as specified in menus
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketLapData
    {
        /// <summary>
        /// Header
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public PacketHeader Header;

        /// <summary>
        /// Lap data for all cars on track
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public LapData[] LapData;
    }
}
