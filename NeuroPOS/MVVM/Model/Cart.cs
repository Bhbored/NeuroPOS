using SQLite;
using SQLiteNetExtensions.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace NeuroPOS.MVVM.Model
{
    public class Cart : Entity
    {
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Product>? Products { get; set; } = new List<Product>();
        public bool Confirmed { get; set; }

       
        public double Subtotal => Products?.Sum(p => p.Price * p.Stock) ?? 0;
        public double Tax
        {
            get => tax;
            set => tax = value >= 0 ? value : 0;
        }

        public double Discount
        {
            get => discount;
            set => discount = value >= 0 ? value : 0;
        }
        public double Total => Subtotal + Tax - Discount;

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Transaction? Transaction { get; set; }

        #region Foreign Keys
        [ForeignKey(typeof(Order))]
        public int OrderId { get; set; }
        #endregion

        #region Private Fields
        private double tax;
        private double discount;
        #endregion
    }
}
