using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace RedmineLogger.Helpers.Converters
{
    public class DayActivityColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new SolidColorBrush(Colors.Transparent);
            if (!(value is DateTime)) return result;
            var day = ((DateTime)value).DayOfWeek;
            return (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                ? new SolidColorBrush(Colors.DimGray)
                : new SolidColorBrush(Colors.CadetBlue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}