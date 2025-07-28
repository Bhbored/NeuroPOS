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
        public double Tax { get; set; }
        public double Discount { get; set; }
        public double SubTotalAMount =>
        (Lines == null || Lines.Count == 0)
        ? 0
        : TransactionType?.Equals("buy", StringComparison.OrdinalIgnoreCase) == true
            ? Lines.Sum(p => p.Cost * p.Stock)
            : Lines.Sum(p => p.Price * p.Stock);

        public int ItemCount { get; set; }

        private string? transactionType = "sell";
        public string? TransactionType
        {
            get => transactionType;
            set => transactionType = value;
        }

        public bool IsExpanded { get; set; } = false;

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TransactionLine> Lines { get; set; } = new();

        public bool IsPaid { get; set; } = true;

        #region Ignore Properties

        [Ignore]
        public string HumanDate => Date.Humanize();

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