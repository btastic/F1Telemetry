using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using F1Telemetry.Models.Raw.F12018;

namespace F1TelemetryUi.ValueConverters
{
    [ValueConversion(typeof(Team), typeof(Brush))]
    public class TeamIdToBrushConverter : IValueConverter
    {
        private static readonly SolidColorBrush s_mercedesBrush = new SolidColorBrush(Color.FromRgb(0, 210, 190));
        private static readonly SolidColorBrush s_ferrariBrush = new SolidColorBrush(Color.FromRgb(220, 0, 0));
        private static readonly SolidColorBrush s_redBullBrush = new SolidColorBrush(Color.FromRgb(30, 65, 255));
        private static readonly SolidColorBrush s_forceIndiaBrush = new SolidColorBrush(Color.FromRgb(245, 150, 200));
        private static readonly SolidColorBrush s_renaultBrush = new SolidColorBrush(Color.FromRgb(255, 245, 0));
        private static readonly SolidColorBrush s_toroRossoBrush = new SolidColorBrush(Color.FromRgb(70, 155, 255));
        private static readonly SolidColorBrush s_haasBrush = new SolidColorBrush(Color.FromRgb(130, 130, 130));
        private static readonly SolidColorBrush s_mcLarenBrush = new SolidColorBrush(Color.FromRgb(255, 135, 0));
        private static readonly SolidColorBrush s_sauberBrush = new SolidColorBrush(Color.FromRgb(155, 0, 0));
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((Team)value)
            {
                case Team.Mercedes:
                    return s_mercedesBrush;
                case Team.Ferrari:
                    return s_ferrariBrush;
                case Team.RedBull:
                    return s_redBullBrush;
                case Team.Williams:
                    return Brushes.White;
                case Team.ForceIndia:
                    return s_forceIndiaBrush;
                case Team.Renault:
                    return s_renaultBrush;
                case Team.ToroRosso:
                    return s_toroRossoBrush;
                case Team.Haas:
                    return s_haasBrush;
                case Team.McLaren:
                    return s_mcLarenBrush;
                case Team.Sauber:
                    return s_sauberBrush;
                default:
                    return Brushes.Gold;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
