using NeuroPOS.MVVM.Model;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.Converters
{
    public class TransactionTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is not Transaction transaction)
                return null;

            // Try multiple approaches to get the transaction type
            string transactionType = "sell"; // Default to sell

            // First, try to get from the transaction type
            if (!string.IsNullOrEmpty(transaction.TransactionType))
            {
                transactionType = transaction.TransactionType.ToLower();
            }

            // Determine which template to use based on transaction type and status
            string templateKey;

            if (transactionType == "buy")
            {
                if (transaction.IsPaid)
                {
                    templateKey = "BuyCompletedTransactionTemplate";
                }
                else
                {
                    templateKey = "BuyTransactionTemplate";
                }
            }
            else
            {
                templateKey = "DefaultTransactionTemplate";
            }

            // Try to get the template from resources
            if (Application.Current?.Resources != null)
            {
                if (Application.Current.Resources.TryGetValue(templateKey, out var template))
                {
                    return template as DataTemplate;
                }
            }

            // If template not found, return null to use default behavior
            return null;
        }
    }
}