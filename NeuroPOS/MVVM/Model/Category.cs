using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.Model
{
    public class Category : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }//optinal
        public string ImageUrl { get; set; } // URL to the category image
        public int ProductCount { get; set; }

        [Ignore]
        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Product> Products { get; set; }
    }
}
