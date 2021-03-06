﻿using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace F1TelemetryUi.ValueConverters
{
    public class GenericBoolConverter<T> : IValueConverter
    {
        public T FalseValue { get; set; }
        public T TrueValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return FalseValue;
            }
            else
            {
                return (bool)value ? TrueValue : FalseValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null && value.Equals(TrueValue);
        }
    }

    public class BoolToStringConverter : GenericBoolConverter<string> { }
    public class BoolToBrushConverter : GenericBoolConverter<Brush> { }
    public class BoolToVisibilityConverter : GenericBoolConverter<Visibility> { }
    public class BoolToObjectConverter : GenericBoolConverter<object> { }
}
