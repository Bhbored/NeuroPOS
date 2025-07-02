using Humanizer;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    public class Transaction : Entity
    {
        public DateTime Date { get; set; }
        [Ignore]
        public string HumanDate { get => Date.Humanize(); }
        public string TransactionType { get; set; } // "buy" or "sell"
        public double TotalAmount { get; set; }
        public double TotalQuantity { get; set; } // Total quantity of items in the transaction

        [ForeignKey(typeof(Person))]
        public int? PersonId { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Product> TransactionItems { get; set; }
        public bool IsPaid { get; set; } // Indicates if the transaction has been paid to add to the cashRegister
        [Ignore]
        public string Icon => TransactionType == "buy" ? "redcart.png" : "greencart.png";

        [ForeignKey(typeof(Contact))]
        public int? ContactId { get; set; }

        [ForeignKey(typeof(Cart))]
        public int? CartId { get; set; } // Nullable if the transaction is not linked to a cart

    }
}
