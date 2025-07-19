using Humanizer;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Transaction : Entity
    {
        private DateTime? date;

        public DateTime Date
        {
            get
            {
                return date ?? DateTime.Now;
            }
            set
            {
                date = value;
            }
        }
        public double TotalAmount { get; set; }

        public int ItemCount { get; set; }
        public string? TransactionType { get; set; } // "buy" or "sell"

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TransactionLine> Lines { get; set; } = new();

        public bool IsPaid { get; set; } = true;
        

        #region Ignore Properties

        [Ignore]
        public string HumanDate { get => Date.Humanize(); }

        [Ignore]
        public string FormattedDate
        {
            get => Date.ToString("dd/MM/yyyy");
        }

        [Ignore]
        public string FormattedTime
        {
            get => Date.ToString("hh:mm tt");
        }

        [Ignore]
        public string Status
        {
            get
            {
                return IsPaid ? "Completed" : "Pending";
            }
        }

        [Ignore]
        public bool IsExpanded { get; set; } = false;

        [Ignore]                                             
        public double CalculatedTotalAmount =>
           (Lines == null || Lines.Count == 0)
               ? 0
               : TransactionType?.Equals("buy", StringComparison.OrdinalIgnoreCase) == true
                   ? Lines.Sum(p => p.Cost)     // buy  → use cost
                   : Lines.Sum(p => p.Price);   // sell → use price

        [Ignore]                                             // not stored in DB
        public int CalculatedItemCount => Lines?.Count ?? 0;
        #endregion

        #region Foreign Keys
        [ForeignKey(typeof(Contact))]
        public int? ContactId { get; set; }
        [ForeignKey(typeof(CashRegister))]
        public int? CashRegisterId { get; set; }

        #endregion

    }
}