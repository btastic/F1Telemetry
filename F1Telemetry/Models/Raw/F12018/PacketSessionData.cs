using System.Runtime.InteropServices;

namespace F1Telemetry.Models.Raw.F12018
{
    /// <summary>
    /// Frequency: 2 per second
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PacketSessionData
    {
        /// <summary>
        /// Header
        /// </summary>
        [MarshalAs(UnmanagedType.Struct)]
        public PacketHeader PacketHeader;

        /// <summary>
        /// Weather - 0 = clear, 1 = light cloud, 2 = overcast
        /// 3 = light rain, 4 = heavy rain, 5 = storm
        /// </summary>
        public WeatherType WeatherType;

        /// <summary>
        /// Track temp. in degrees celsius
        /// </summary>
        public sbyte TrackTemperature;

        /// <summary>
        /// Air temp. in degrees celsius
        /// </summary>
        public sbyte AirTemperature;

        /// <summary>
        /// Total number of laps in this race
        /// </summary>
        public byte TotalLaps;

        /// <summary>
        /// Track length in metres
        /// </summary>
        public ushort TrackLength;

        /// <summary>
        /// 0 = unknown, 1 = P1, 2 = P2, 3 = P3, 4 = Short P
        /// 5 = Q1, 6 = Q2, 7 = Q3, 8 = Short Q, 9 = OSQ
        /// 10 = R, 11 = R2, 12 = Time Trial
        /// </summary>
        public SessionType SessionType;

        /// <summary>
        /// -1 for unknown, 0-21 for tracks
        /// </summary>
        public Track TrackId;

        /// <summary>
        /// Era, 0 = modern, 1 = classic
        /// </summary>
        public Era Era;

        /// <summary>
        /// Time left in session in seconds
        /// </summary>
        public ushort SessionTimeLeft;

        /// <summary>
        /// Session duration in seconds
        /// </summary>
        public ushort SessionDuration;

        /// <summary>
        /// Pit speed limit in kilometres per hour
        /// </summary>
        public byte PitSpeedLimit;

        /// <summary>
        /// Whether the game is paused
        /// </summary>
        public byte GamePaused;

        /// <summary>
        /// Whether the player is spectating
        /// </summary>
        public byte IsSpectating;

        /// <summary>
        /// Index of the car being spectated
        /// </summary>
        public byte SpectatorCarIndex;

        /// <summary>
        /// SLI Pro support, 0 = inactive, 1 = active
        /// </summary>
        public SliProNativeSupport SliProNativeSupport;

        /// <summary>
        /// Number of marshal zones to follow
        /// </summary>
        public byte NumMarshalZones;

        /// <summary>
        /// List of marshal zones – max 21
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
        public MarshalZoneData[] MarshalZones;

        /// <summary>
        /// 0 = no safety car, 1 = full safety car
        /// 2 = virtual safety car
        /// </summary>
        public SafetyCarStatus SafetyCarStatus;

        /// <summary>
        /// 0 = offline, 1 = online
        /// </summary>
        public NetworkGame NetworkGame;
    }
}
