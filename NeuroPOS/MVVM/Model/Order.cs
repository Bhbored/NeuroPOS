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

        public int ContactId { get; set; } = 0;

        public double Tax { get; set; }
        public double Discount { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<TransactionLine> Lines { get; set; } = new();
        public double SubTotalAmount =>
        (Lines == null || Lines.Count == 0)
        ? 0 : Lines.Sum(p => p.Price * p.Stock);

        // Computed property for item count to ensure consistency
        [Ignore]
        public int ComputedItemCount => Lines?.Count ?? 0;

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
            (Lines == null ||Lines.Count == 0)
                ? 0
                : SubTotalAmount - Discount + (SubTotalAmount * Tax / 100);
        #endregion


    }
}
