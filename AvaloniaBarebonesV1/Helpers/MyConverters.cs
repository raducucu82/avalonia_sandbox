using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace VisualTests.Helpers
{
    public class Not: IValueConverter
    {
        public static readonly Not Instance = new Not();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
                return !b;
            return value;
        }
    }
}
