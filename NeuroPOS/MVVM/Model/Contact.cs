using Humanizer;
using PropertyChanged;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeuroPOS.MVVM.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Contact : Entity
    {
        public string Name { get; set; }
        public string? Email { get; set; } // Optional, can be null
        public string? PhoneNumber { get; set; } // Optional, can be null
        public DateTime DateAdded { get; set; } // Date when the contact was added
        public string? Address { get; set; } // Optional, can be null

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Transaction>? Transactions { get; set; } // List of transactions associated with the contact

        #region Ignore Properties
        [Ignore]
        public double AmountSold =>
           Transactions?.Sum(t => t.TotalAmount) ?? 0;

        // Total of the ones that are still unpaid
        [Ignore]
        public double CreditAmount =>
            Transactions?.Where(t => !t.IsPaid)
                         .Sum(t => t.TotalAmount) ?? 0;

        [Ignore]
        public double PaidAMount => AmountSold - CreditAmount;

        [Ignore]
        public string HumanDate { get => DateAdded.Humanize(); }

        [Ignore]
        public string FormattedDate
        {
            get => DateAdded.ToString("dd/MM/yyyy");
        }

        [Ignore]
        public string FormattedTime
        {
            get => DateAdded.ToString("hh:mm tt");
        }

        [Ignore]
        public bool IsSelected { get; set; }
        #endregion


    }
}
