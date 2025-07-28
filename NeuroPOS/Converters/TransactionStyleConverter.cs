using NeuroPOS.MVVM.Model;
using System.Globalization;

namespace NeuroPOS.Converters
{
    public class TransactionStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            // The value should be the Transaction object
            if (value is not Transaction transaction)
                return null;

            string styleType = parameter.ToString()?.ToLower() ?? "border";
            string transactionType = transaction.TransactionType?.ToLower() ?? "sell";
            bool isPaid = transaction.IsPaid;

            if (styleType == "border")
            {
                if (transactionType == "buy")
                {
                    return Color.FromHex("#EF4444"); // Red border for buy transactions
                }
                else if (transactionType == "sell" && isPaid)
                {
                    return Color.FromHex("#10B981"); // Green border for sell transactions with completed status
                }
                else
                {
                    return Color.FromHex("#E5E7EB"); // Default gray border for other transactions
                }
            }
            else if (styleType == "background")
            {
                if (transactionType == "buy")
                {
                    return Color.FromHex("#FFF5F5"); // Light red background for buy transactions
                }
                else if (transactionType == "sell" && isPaid)
                {
                    return Color.FromHex("#F0FFF4"); // Light green background for sell transactions with completed status
                }
                else
                {
                    return Colors.Transparent; // No background for other transactions
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}