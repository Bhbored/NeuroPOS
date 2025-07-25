using System;
using System.Globalization;

namespace NeuroPOS.Converters
{
    public class FilterButtonTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string selectedFilter && parameter is string buttonFilter)
            {
                return selectedFilter == buttonFilter ? "White" : "#2D7FF9";
            }
            return "#2D7FF9";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 