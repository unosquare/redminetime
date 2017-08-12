using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Unosquare.RedmineTime.Models;

namespace Unosquare.RedmineTime.Helpers.Converters
{
    public class TimeEntryOriginColorConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new SolidColorBrush(Colors.Transparent);
            if (!(value is TimeEntryOrigin)) return result;
            switch ((TimeEntryOrigin)value)
            {
                case TimeEntryOrigin.RedmineService:
                    result = new SolidColorBrush(Colors.DarkGray);
                    break;
                case TimeEntryOrigin.Outlook:
                    result = new SolidColorBrush(Colors.LightBlue);
                    break;
                case TimeEntryOrigin.NewInLogger:
                    result = new SolidColorBrush(Colors.White);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}