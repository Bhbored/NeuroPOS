using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace NeuroPOS.Converters
{
    // Converts TransactionType string to a background color for type badges
    public class TransactionTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string transactionType)
            {
                switch (transactionType.ToLower())
                {
                    case "sell":
                        return Color.FromArgb("#1DB954"); // Green for sell
                    case "buy":
                        return Color.FromArgb("#E53935"); // Red for buy
                    default:
                        return Color.FromArgb("#B0B0B0"); // Gray for unknown
                }
            }

            // Fallback for boolean values (IsPaid)
            if (value is bool isPaid)
            {
                return isPaid ? Color.FromArgb("#1DB954") : Color.FromArgb("#F9A825");
            }

            return Color.FromArgb("#B0B0B0");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
