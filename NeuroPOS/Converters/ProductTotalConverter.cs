using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using NeuroPOS.MVVM.Model;

namespace NeuroPOS.Converters
{
    public class ProductTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Product product)
            {
                var total = product.Price * product.Stock;
                return total.ToString("C2", culture);
            }
            else if (value is TransactionLine line)
            {
                var total = line.Price * line.Stock;
                return total.ToString("C2", culture);
            }
            return "$0.00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}