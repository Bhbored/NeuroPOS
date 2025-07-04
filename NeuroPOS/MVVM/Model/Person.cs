using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    public class Person : Entity
    {
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }
        public double AmountOwed { get; set; }

   
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Transaction>? PersonTransactions { get; set; }
    }
}
