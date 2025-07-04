using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    public class Contact : Entity
    {
        public string Name { get; set; }
        public string Email { get; set; } // Optional, can be null
        public string PhoneNumber { get; set; } // Optional, can be null
        public DateTime DateAdded { get; set; } // Date when the contact was added
        public string Address { get; set; } // Optional, can be null
        public double creditAmount { get; set; } 
        public double AmountSold { get; set; } // Total amount sold to the contact

        
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Transaction>? Transactions { get; set; } // List of transactions associated with the contact

    }
}
