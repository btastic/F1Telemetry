using System.Runtime.InteropServices;
using MessagePack;

namespace F1Telemetry.Models.Raw.F12018
{
    [MessagePackObject(keyAsPropertyName: true)]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MarshalZoneData
    {
        /// <summary>
        /// Fraction (0..1) of way through the lap the marshal zone starts
        /// </summary>
        public float ZoneStart;

        /// <summary>
        /// -1 = invalid/unknown, 0 = none, 1 = green, 2 = blue, 3 = yellow, 4 = red
        /// </summary>
        public Flag ZoneFlag;

        public override string ToString()
        {
            return $"{ZoneStart} - {ZoneFlag}";
        }
    }
}
