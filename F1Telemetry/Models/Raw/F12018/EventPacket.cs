using System.Runtime.InteropServices;

namespace F1Telemetry.Models.Raw.F12018
{
    /// <summary>
    /// Frequency: When the event occurs
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EventPacket
    {
        /// <summary>
        /// Header
        /// </summary>
        public PacketHeader Header;

        /// <summary>
        /// SSTA = Sent when the session starts
        /// SEND = Sent when the session ends
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] EventStringCode;
    }
}
