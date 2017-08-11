using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RedmineLogger.Helpers.Converters
{
    public class BoolVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueAsBool = false;
            if (value is bool)
                valueAsBool = (bool) value;
            return valueAsBool ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = value as Visibility?;
            return visibility == Visibility.Visible;
        }
    }
}