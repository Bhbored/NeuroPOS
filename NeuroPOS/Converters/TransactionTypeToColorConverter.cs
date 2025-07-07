using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace NeuroPOS.Converters
{
    // Converts TransactionType to a background color for the DataGrid column
    public class TransactionTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string type)
            {
                switch (type)
                {
                   
                    case "Buy":
                        return Colors.Red;
                    default:
                        return Colors.LightGray;
                }
            }
            return Colors.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
