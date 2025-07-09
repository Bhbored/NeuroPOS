using NeuroPOS.MVVM.ViewModel;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PropertyChanged;

namespace NeuroPOS.MVVM.Model
{
    [AddINotifyPropertyChangedInterface]
    public class Category : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; } // Optional
        public string ImageUrl { get; set; } // URL to the category image
        public int ProductCount { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Product>? Products { get; set; }

        // This will trigger UI updates automatically if Fody is active
        [Ignore]
        public string State { get; set; } = "Inactive Categorie";
    }
}
