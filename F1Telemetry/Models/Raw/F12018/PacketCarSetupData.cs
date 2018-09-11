using System.Runtime.InteropServices;
using MessagePack;

namespace F1Telemetry.Models.Raw.F12018
{
    /// <summary>
    /// Frequency: Every 5 seconds
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketCarSetupData
    {
        /// <summary>
        /// Header
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public PacketHeader Header;

        /// <summary>
        /// List of CarSetups
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public CarSetupData[] CarSetups;
    }
}
