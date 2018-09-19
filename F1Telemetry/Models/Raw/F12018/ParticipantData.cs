using System.Runtime.InteropServices;
using System.Text;
using MessagePack;

namespace F1Telemetry.Models.Raw.F12018
{
    public static class ParticipantDataExtensions
    {
        public static string GetThreeLetterName(this ParticipantData participantData, bool isOnlineRace)
        {
            if (isOnlineRace)
            {
                return Encoding.UTF8.GetString(participantData.Name);
            }

            switch (participantData.Driver)
            {
                case Driver.CarlosSainz:
                    return "SAI";
                case Driver.DanielRicciardo:
                    return "RIC";
                case Driver.FernandoAlonso:
                    return "ALO";
                case Driver.KimiRaikkonen:
                    return "RAI";
                case Driver.LewisHamilton:
                    return "HAM";
                case Driver.MarcusEricsson:
                    return "ERI";
                case Driver.MaxVerstappen:
                    return "VER";
                case Driver.NicoHulkenberg:
                    return "HUL";
                case Driver.KevinMagnussen:
                    return "MAG";
                case Driver.RomainGrosjean:
                    return "GRO";
                case Driver.SebastianVettel:
                    return "VET";
                case Driver.SergioPerez:
                    return "PER";
                case Driver.ValtteriBottas:
                    return "BOT";
                case Driver.EstebanOcon:
                    return "OCO";
                case Driver.StoffelVandoorne:
                    return "VAN";
                case Driver.LanceStroll:
                    return "STR";
                default:
                    return Encoding.UTF8.GetString(participantData.Name);
            }
        }
    }

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
        public byte[] Name;
    }
}
