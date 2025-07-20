using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace NeuroPOS.MVVM.Model
{
    [AddINotifyPropertyChangedInterface]
    public class CashRegister : Entity
    {
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        #region Ignore Properties
        [Ignore]
        public double TotalIncome => Transactions?
           .Where(t => t.TransactionType == "sell" && t.IsPaid)
           .Sum(t => t.TotalAmount) ?? 0;

        [Ignore]
        public double TotalExpenses => Transactions?
            .Where(t => t.TransactionType == "buy" && t.IsPaid)
            .Sum(t => t.TotalAmount) ?? 0;

        [Ignore]
        public double NetProfit => TotalIncome - TotalExpenses;

        [Ignore]
        public int TotalTransactions => Transactions?.Count ?? 0;

        [Ignore]
        public int TotalItemsSold => Transactions?
            .Where(t => t.TransactionType == "sell")
            .SelectMany(t => t.Lines)
            .Count() ?? 0;

        [Ignore]
        public int TotalItemsBought => Transactions?
            .Where(t => t.TransactionType == "buy")
            .SelectMany(t => t.Lines)
            .Count() ?? 0;

        [Ignore]
        public double TotalCreditSales =>
    Transactions?
        .Where(t => t.TransactionType == "sell" && !t.IsPaid)
        .Sum(t => t.TotalAmount) ?? 0;

        [Ignore]
        public string Summary =>
            $"Sales: {TotalIncome:C}, Expenses: {TotalExpenses:C}, Profit: {NetProfit:C}";

        #endregion
    }
}
