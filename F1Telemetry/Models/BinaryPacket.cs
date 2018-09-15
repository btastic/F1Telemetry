using System;
using F1Telemetry.Models.Raw.F12018;
using MessagePack;

namespace F1Telemetry
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class BinaryPacket
    {
        public readonly PacketHeader PacketHeader;
        public readonly uint FrameIdentifier;

        public byte[] Data { get; set; }

        [SerializationConstructor]
        public BinaryPacket(
            PacketHeader packetHeader,
            byte[] data)
        {
            PacketHeader = packetHeader;
            Data = data;
            FrameIdentifier = packetHeader.FrameIdentifier;
        }
    }
}
