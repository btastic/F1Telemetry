using System;
using System.Runtime.InteropServices;

namespace F1Telemetry.Models.Raw.F12018
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketHeader
    {
        /// <summary>
        /// 2018
        /// </summary>
        public ushort PacketFormat;

        /// <summary>
        /// Version of this packet type, all start from 1
        /// </summary>
        public sbyte PacketVersion;

        /// <summary>
        /// Identifier for the packet type, see below
        /// </summary>
        public PacketType PacketType;

        /// <summary>
        /// Unique identifier for the session
        /// </summary>
        public UInt64 SessionUId;

        /// <summary>
        /// Session timestamp
        /// </summary>
        public float SessionTime;

        /// <summary>
        /// Identifier for the frame the data was retrieved on
        /// </summary>
        public uint FrameIdentifier;

        /// <summary>
        /// Index of player's car in the array
        /// </summary>
        public sbyte PlayerCarIndex;
    }
}
