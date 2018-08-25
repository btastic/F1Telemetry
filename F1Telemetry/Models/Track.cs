using System.Collections.Generic;

namespace F1Telemetry.Models
{
    public class Track
    {
        public string Name { get; set; } = "UNDEFINED";
        public string MapFileName
        {
            get
            {
                return $"/F1TelemetryUi;component/Resources/{Name}.png";
            }
        }
        public int Angle { get; set; } = 50;
        public int XOffset { get; set; } = 500;
        public int YOffset { get; set; } = 500;
        public double Scale { get; set; } = 1;

        public static List<Track> Tracks
        {
            get
            {
                return new List<Track>
                {
                    new Track
                    {
                        Name = "Hungaroring",
                        Angle = 50,
                        YOffset = 581,
                        XOffset = 563,
                        Scale = 0.4252238
                    },
                    new Track
                    {
                        Name = "Silverstone",
                        Angle = 97,
                        YOffset = 1073,
                        XOffset = 1050,
                        Scale = 0.257612,
                    },
                };
            }
        }
    }
}
