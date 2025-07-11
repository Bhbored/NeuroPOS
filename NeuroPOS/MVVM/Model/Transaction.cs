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
        public DateTime Date { get; set; }

        public string? TransactionType { get; set; } // "buy" or "sell"
        public double TotalAmount { get; set; }
        public double ItemCount { get; set; } // Total quantity of items in the transaction

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Product>? TransactionItems { get; set; }
        public bool IsPaid { get; set; } // Indicates if the transaction has been paid to add to the cashRegister

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
        #endregion

        #region Foreign Keys
        [ForeignKey(typeof(Person))]
        public int? PersonId { get; set; }
        [ForeignKey(typeof(Contact))]
        public int? ContactId { get; set; }

        [ForeignKey(typeof(Cart))]
        public int? CartId { get; set; } // Nullable if the transaction is not linked to a cart
        #endregion

    }
}