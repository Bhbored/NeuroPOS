using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    public class CashRegister : Entity
    {
        public double TotalAmount { get; set; }
        public DateTime DateAdded { get; set; }
        public double EntryAmount { get; set; } // Amount added to the cash register
    }
}
