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
        public string? Description { get; set; }
        public int ProductCount
        {
            get
            {
                return Products?.Count ?? 0;
            }
        }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Product>? Products =>
            App.ProductRepo?.GetItems()?.Where(p => p.CategoryName == Name).ToList();


        [Ignore]
        public string State { get; set; } = "Inactive Categorie";

        [Ignore]
        public bool IsBeingEdited { get; set; } = false;
    }
}
