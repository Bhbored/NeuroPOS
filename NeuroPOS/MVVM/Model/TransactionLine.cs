using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace NeuroPOS.MVVM.Model
{
    [AddINotifyPropertyChangedInterface]
    public class TransactionLine : Entity
    {

        public string Name { get; set; }
        public double Price { get; set; }
        public double Cost { get; set; }
        public int Stock { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

        public string CategoryName
        {
            get => string.IsNullOrWhiteSpace(categoryName) ? "Uncategorized" : categoryName;
            set => categoryName = value;
        }

        public string ImageUrl
        {
            get => string.IsNullOrWhiteSpace(imageUrl) ? "emptyproduct.png" : imageUrl;
            set => imageUrl = value;
        }

        #region Fields

        private string imageUrl;
        private string categoryName;
        #endregion
        #region Ignore Properties
        [Ignore]
        public string FormattedDate => DateAdded.ToString("dd/MM/yyyy");
        [Ignore]
        public double TotalPrice => Price * Stock;
        [Ignore]
        public double TotalCost => Cost * Stock;

        #endregion

        [ForeignKey(typeof(Transaction))]
        public int TransactionId { get; set; }

        [ForeignKey(typeof(Product))]
        public int ProductId { get; set; }

        [ForeignKey(typeof(Order))]
        public int OrderId { get; set; }

        [ManyToOne] public Transaction Transaction { get; set; }

        [ManyToOne] public Product Product { get; set; }

        [ManyToOne] public Order Order { get; set; }
    }
}
