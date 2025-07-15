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
    public class Product : Entity
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



        #region Fields

        private string imageUrl;
        private string categoryName;
        #endregion

        #region Ignore Properties
        [Ignore]
        public string FormattedDate => DateAdded.ToString("dd/MM/yyyy");

        [Ignore]
        public string ImageUrl
        {
            get => string.IsNullOrWhiteSpace(imageUrl) ? "emptyproduct.png" : imageUrl;
            set => imageUrl = value;
        }

        #endregion

        #region foreign keys
        [ForeignKey(typeof(Category))]
        public int? CategoryId { get; set; }  // Keep this if you want to do queries based on the ID

        [ForeignKey(typeof(Transaction))]
        public int? TransactionId { get; set; }

        [ForeignKey(typeof(Order))]
        public int? OrderId { get; set; }


        #endregion
    }

}
