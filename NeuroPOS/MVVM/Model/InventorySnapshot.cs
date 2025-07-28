using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    public class InventorySnapshot :Entity
    {
        public DateTime Date { get; set; }         
        public double TotalValue { get; set; }
    }
}
