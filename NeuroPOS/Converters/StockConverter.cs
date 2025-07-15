using NeuroPOS.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Converters
{
    public class StockConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not int stock)
                return null;

            // Return color if target type is Color
            if (targetType == typeof(Color))
            {
                return stock <= 0 ? Colors.Red : // Red for out of stock
                       stock <= 5 ? Colors.Orange : // Orange for low stock
                       Colors.Green; // Green for in stock
            }

            // Return progress value (0-1) for ProgressBar
            if (targetType == typeof(double))
            {
                // Normalize stock to a 0-1 range (assuming max stock of 200 for progress calculation)
                var maxStock = 200.0;
                return Math.Min(stock / maxStock, 1.0);
            }

            // Return string for text display
            return stock <= 0 ? "Out of Stock" :
                   stock <= 5 ? $"({stock} Items) Low Stock" :
                   $"({stock} Items) In Stock";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
