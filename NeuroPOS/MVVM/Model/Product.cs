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
    public class Product : Entity
    {
        public string Name { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public DateTime DateAdded { get; set; }
        public string ImageUrl { get; set; }

        [ForeignKey(typeof(Category))]
        public int? CategoryId { get; set; }  // Keep this if you want to do queries based on the ID

        [ManyToOne]
        public Category Category { get; set; }

        [Ignore]
        public string CategoryName => Category?.Name ?? "Uncategorized";

        [ForeignKey(typeof(Transaction))]
        public int? TransactionId { get; set; }

        [ForeignKey(typeof(Cart))]
        public int? CartId { get; set; }
    }

}
