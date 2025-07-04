using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    public class Order: Entity
    {
        public DateTime Date { get; set; } = DateTime.Now;

        public bool IsConfirmed { get; set; } = false;

        public double TotalAmount { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.All)]
        public Cart? Cart { get; set; }
    }
}
