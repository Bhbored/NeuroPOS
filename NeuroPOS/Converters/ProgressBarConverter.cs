using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Converters
{
    public class ProgressBarConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var stock = ((Label)parameter).Text;
            var stockValue = int.Parse(stock);
            if (stockValue >= 50)
            {
                return "#03fc45";

            }
            else if (stockValue >= 20 && stockValue < 50)
            {
                return "#fce803";
            }
            else if (stockValue < 20 && stockValue > 0)
            {
                return "#fc0303";
            }
            return "#000000"; // Default color if none of the conditions are met


        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
