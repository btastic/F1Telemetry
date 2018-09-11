using System.Runtime.InteropServices;
using MessagePack;

namespace F1Telemetry.Models.Raw.F12018
{
    [MessagePackObject(keyAsPropertyName: true)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ParticipantData
    {
        /// <summary>
        /// Whether the vehicle is AI (1) or Human (0) controlled
        /// </summary>
        public byte AIControlled;

        /// <summary>
        /// Driver id - see appendix
        /// </summary>
        public Driver Driver;

        /// <summary>
        /// Team id - see appendix
        /// </summary>
        public Team Team;

        /// <summary>
        /// Race number of the car
        /// </summary>
        public byte RaceNumber;

        /// <summary>
        /// Nationality of the driver
        /// </summary>
        public Nationality Nationality;

        /// <summary>
        /// Name of participant in UTF-8 format – null terminated
        /// Will be truncated with … (U+2026) if too long
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public char[] Name;
    }
}
