using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace NeuroPOS.Converters
{
    public class BuyingComponentVis : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not NeuroPOS.MVVM.Model.Transaction transaction)
                return Visibility.Collapsed;

            // If parameter is provided, check if it matches the transaction type
            if (parameter is string labelName)
            {
                // For the "Type" label, show only if transaction type is "buy"
                if (labelName == "Type")
                {
                    return transaction.TransactionType == "buy" ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            // Default behavior: show for buy transactions, hide for sell transactions
            return transaction.TransactionType == "buy" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
