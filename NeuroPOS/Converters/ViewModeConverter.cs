using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace NeuroPOS.Converters
{
    public class ViewModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int viewMode && parameter is string mode)
            {
                switch (mode.ToLower())
                {
                    case "empty":
                        return viewMode == 0;
                    case "details":
                        return viewMode == 1;
                    case "edit":
                        return viewMode == 2;
                    default:
                        return false;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}