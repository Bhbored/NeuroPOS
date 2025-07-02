using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    public class Cart :Entity
    {
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Product> Products { get; set; }
        public bool Confirmed { get; set; } // Indicates if the cart has been confirmed for checkout

        public double Subtotal { get; set; }
        public double Tax { get; set; } = 0; // Total tax applied to the cart
        public double Discount { get; set; } // Total discount applied to the cart
        public double Total { get; set; } // Total amount after applying discounts or taxes

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Transaction Transaction { get; set; } // Associated transaction for the cart


    }
}
