using System.Runtime.InteropServices;
using MessagePack;

namespace F1Telemetry.Models.Raw.F12018
{
    /// <summary>
    /// Frequency: Every 5 seconds
    /// </summary>
    [MessagePackObject(keyAsPropertyName: true)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketParticipantsData
    {
        /// <summary>
        /// Header
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public PacketHeader Header;

        /// <summary>
        /// Number of cars in the data
        /// </summary>
        public byte NumCars;

        /// <summary>
        /// List of Participants
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public ParticipantData[] Participants;
    }
}
