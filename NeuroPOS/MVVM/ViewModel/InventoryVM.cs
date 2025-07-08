using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuroPOS.MVVM.Model;

namespace NeuroPOS.MVVM.ViewModel
{

    
    public class InventoryVM
    {

        public ObservableCollection<Product> Products { get; set; } = [];
        public InventoryVM()
        {
            LoadData();
        }
        public ObservableCollection<Product> LoadData()
        {
             Products = new ObservableCollection<Product>
        {
            new Product { Name = "Organic Apples", Price = 1.99, Stock = 150, DateAdded = DateTime.Now.AddDays(-10), CategoryName = "Produce" },
            new Product { Name = "Whole Wheat Bread", Price = 3.49, Stock = 75, DateAdded = DateTime.Now.AddDays(-8), CategoryName = "Bakery" },
            new Product { Name = "Cheddar Cheese", Price = 4.99, Stock = 100, DateAdded = DateTime.Now.AddDays(-6), CategoryName = "Dairy" },
            new Product { Name = "Ground Beef", Price = 5.99, Stock = 50, DateAdded = DateTime.Now.AddDays(-4), CategoryName = "Meat" },
            new Product { Name = "Salmon Fillet", Price = 9.99, Stock = 30, DateAdded = DateTime.Now.AddDays(-2), CategoryName = "Seafood" },
        };
            return Products;
        }
      
    }
}
