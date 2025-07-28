using NeuroPOS.MVVM.Model;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Converters
{
    public class LineDTSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is not TransactionLine transactionLine)
                return null;

            // Try multiple approaches to get the transaction type
            string transactionType = "sell"; // Default to sell

            // First, try to get from the container's binding context
            if (container?.BindingContext is Transaction transaction)
            {
                transactionType = transaction.TransactionType ?? "sell";
            }
            // Second, try to get from the TransactionLine's Transaction property
            else if (transactionLine.Transaction != null)
            {
                transactionType = transactionLine.Transaction.TransactionType ?? "sell";
            }

            var key = transactionType == "sell" ? "Selling Lines" : "Buying Lines";
            Application.Current.Resources.TryGetValue(key, out var template);
            return template as DataTemplate;
        }
    }
}
