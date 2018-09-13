using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using F1Telemetry.Models.Raw.F12018;

namespace F1TelemetryUi.ValueConverters
{
    [ValueConversion(typeof(TyreCompound), typeof(ImageSource))]
    public class TyreCompoundToImageConverter : IValueConverter
    {
        private static readonly BitmapImage s_hyperSoftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/hypersoft-new.png"));
        private static readonly BitmapImage s_ultraSoftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/ultrasoft-new.png"));
        private static readonly BitmapImage s_superSoftImage = new BitmapImage(new Uri("pack://application:,,,/Resources/supersoft-new.png"));
        private static readonly BitmapImage s_softImage = new BitmapImage(new Uri("pack://application:,,,/Resources/soft-new.png"));
        private static readonly BitmapImage s_mediumImage = new BitmapImage(new Uri("pack://application:,,,/Resources/medium-new.png"));
        private static readonly BitmapImage s_hardImage = new BitmapImage(new Uri("pack://application:,,,/Resources/hard-new.png"));
        private static readonly BitmapImage s_superHardImage = new BitmapImage(new Uri("pack://application:,,,/Resources/superhard-new.png"));
        private static readonly BitmapImage s_intermediateImage = new BitmapImage(new Uri("pack://application:,,,/Resources/intermediate-new.png"));
        private static readonly BitmapImage s_wetImage = new BitmapImage(new Uri("pack://application:,,,/Resources/wet-new.png"));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((TyreCompound)value)
            {
                case TyreCompound.HyperSoft:
                    return s_hyperSoftImage;
                case TyreCompound.UltraSoft:
                    return s_ultraSoftImage;
                case TyreCompound.SuperSoft:
                    return s_superSoftImage;
                case TyreCompound.Soft:
                    return s_softImage;
                case TyreCompound.Medium:
                    return s_mediumImage;
                case TyreCompound.Hard:
                    return s_hardImage;
                case TyreCompound.SuperHard:
                    return s_superHardImage;
                case TyreCompound.Intermediate:
                    return s_intermediateImage;
                case TyreCompound.Wet:
                    return s_wetImage;
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
