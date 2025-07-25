using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace NeuroPOS.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isPaid)
            {
                return isPaid ? Color.FromArgb("#1DB954") : Color.FromArgb("#F9A825");
            }
            else if (value is string status)
            {
                return status == "Completed" ? Color.FromArgb("#1DB954") : Color.FromArgb("#F9A825");
            }
            return Color.FromArgb("#B0B0B0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}