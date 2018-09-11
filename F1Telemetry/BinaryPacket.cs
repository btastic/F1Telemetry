using System;
using F1Telemetry.Models.Raw.F12018;
using MessagePack;

namespace F1Telemetry
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class BinaryPacket
    {
        public readonly byte[] Data;
        public readonly PacketHeader PacketHeader;
        public readonly TimeSpan TimeSpan;

        [SerializationConstructor]
        public BinaryPacket(
            PacketHeader packetHeader,
            byte[] data,
            TimeSpan timeSpan)
        {
            PacketHeader = packetHeader;
            Data = data;
            TimeSpan = timeSpan;
        }

        internal object First()
        {
            throw new NotImplementedException();
        }
    }
}
