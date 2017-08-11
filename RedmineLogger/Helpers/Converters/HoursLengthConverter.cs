using System;
using System.Globalization;
using System.Windows.Data;

namespace RedmineLogger.Helpers.Converters
{
    public class HoursLengthConverter: IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return 40;
            var totalLenght = (double)values[1];
            var hours = (decimal)values[0];

            // TODO: inject max hours in a day
            const double maxHoursInDay = 8.0;
            // TODO: calculate offset value in a better way (consider margins, etc.)
            const double offsetHeight = 100.0;

            return (totalLenght - offsetHeight)/maxHoursInDay*System.Convert.ToDouble(hours);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}