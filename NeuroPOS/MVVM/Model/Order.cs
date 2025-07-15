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
    public class Order: Entity
    {
        public DateTime Date { get; set; } = DateTime.Now;

        public bool IsConfirmed { get; set; } = false;

        public double TotalAmount { get; set; }

        public int ItemCount { get; set; }
        public string CustomerName { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Product> OrderItems { get; set; } = new List<Product>();

        #region ignore Properties

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
                return IsConfirmed ? "Completed" : "Pending";
            }
        }
        [Ignore]
        public double CalculatedTotalAmount =>
            (OrderItems == null || OrderItems.Count == 0)
                ? 0
                : OrderItems.Sum(item => item.Price * item.Stock);
        #endregion


    }
}
