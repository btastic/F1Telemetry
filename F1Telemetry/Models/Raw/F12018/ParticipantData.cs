using System.Runtime.InteropServices;

namespace F1Telemetry.Models.Raw.F12018
{
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
        public byte DriverId;

        /// <summary>
        /// Team id - see appendix
        /// </summary>
        public byte TeamId;

        /// <summary>
        /// Race number of the car
        /// </summary>
        public byte RaceNumber;

        /// <summary>
        /// Nationality of the driver
        /// </summary>
        public byte Nationality;

        /// <summary>
        /// Name of participant in UTF-8 format – null terminated
        /// Will be truncated with … (U+2026) if too long
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
        public char[] Name;
    }
}
