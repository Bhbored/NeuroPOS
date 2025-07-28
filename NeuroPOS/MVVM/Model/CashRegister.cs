using PropertyChanged;
using SQLite;
using Humanizer;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NeuroPOS.MVVM.Model
{

    public class CashRegister : Entity, INotifyPropertyChanged
    {
        private List<Transaction> _transactions = new List<Transaction>();

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Transaction> Transactions
        {
            get => _transactions;
            set
            {
                _transactions = value;
                OnPropertyChanged(nameof(Transactions));
                OnPropertyChanged(nameof(TotalIncome));
                OnPropertyChanged(nameof(TotalExpenses));
                OnPropertyChanged(nameof(NetProfit));
                OnPropertyChanged(nameof(TotalTransactions));
                OnPropertyChanged(nameof(TotalItemsSold));
                OnPropertyChanged(nameof(TotalItemsBought));
                OnPropertyChanged(nameof(TotalCreditSales));
            }
        }

        #region Ignore Properties
        [Ignore]
        public double TotalIncome => Transactions?
           .Where(t => t.TransactionType == "sell" && t.IsPaid)
           .Sum(t => t.TotalAmount) ?? 0;

        [Ignore]
        public double TotalSales => Transactions?
           .Where(t => t.TransactionType == "sell")
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

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
