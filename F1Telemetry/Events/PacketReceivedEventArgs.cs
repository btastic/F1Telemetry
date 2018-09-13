using System;

namespace F1Telemetry.Events
{
    public class PacketReceivedEventArgs<T> : EventArgs
    {
        public PacketReceivedEventArgs(T oldPacket, T packet)
        {
            OldPacket = oldPacket;
            Packet = packet;
        }

        public T OldPacket { get; set; }
        public T Packet { get; set; }
    }
}
