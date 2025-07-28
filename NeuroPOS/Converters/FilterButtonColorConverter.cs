using System;
using System.Globalization;

namespace NeuroPOS.Converters
{
    public class FilterButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string selectedFilter && parameter is string buttonFilter)
            {
                return selectedFilter == buttonFilter ? "#2D7FF9" : "Transparent";
            }
            return "Transparent";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 