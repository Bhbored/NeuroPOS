using PropertyChanged;
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
    [AddINotifyPropertyChangedInterface]
    public class Product : Entity
    {
      
        public string Name { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public DateTime DateAdded { get; set; }

        [Ignore]
        public string FormattedDate => DateAdded.ToString("dd/MM/yyyy");
        public string ImageUrl
        {
            get => string.IsNullOrWhiteSpace(imageUrl) ? "emptyproduct.png" : imageUrl;
            set => imageUrl = value;
        }


        [ForeignKey(typeof(Category))]
        public int? CategoryId { get; set; }  // Keep this if you want to do queries based on the ID

        public string CategoryName
        {
            get
            {
                return CategoryName ?? "Uncategorized";
            }
        }

        [ForeignKey(typeof(Transaction))]
        public int? TransactionId { get; set; }

        [ForeignKey(typeof(Cart))]
        public int? CartId { get; set; }





        private string imageUrl;
    }

}
