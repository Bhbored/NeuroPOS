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
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return 0.0;

            // The value should be the CashRegister object
            var cashRegister = value as dynamic;
            if (cashRegister == null)
                return 0.0;

            // Parameter should be the property name to use as the current value
            string propertyName = parameter.ToString();

            // Get the current value based on property name
            double currentValue = 0.0;
            double maxValue = 0.0;

            try
            {
                switch (propertyName)
                {
                    case "TotalIncome":
                        currentValue = (double)cashRegister.TotalIncome;
                        maxValue = (double)cashRegister.TotalIncome;
                        break;
                    case "TotalCreditSales":
                        currentValue = (double)cashRegister.TotalCreditSales;
                        maxValue = (double)cashRegister.TotalIncome;
                        break;
                    case "TotalExpenses":
                        currentValue = (double)cashRegister.TotalExpenses;
                        maxValue = (double)cashRegister.TotalIncome;
                        break;
                    default:
                        return 0.0;
                }

                // Calculate the progress as a percentage
                if (maxValue == 0)
                    return 0.0;

                double progress = currentValue / maxValue;

                // Ensure progress is between 0 and 1
                return Math.Max(0, Math.Min(1, progress));
            }
            catch
            {
                return 0.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
