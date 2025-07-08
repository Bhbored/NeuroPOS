using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace NeuroPOS.Converters
{
    public class IntToInverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue == 0;
            }
            return true; // Default to showing empty state if value is not an int
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}