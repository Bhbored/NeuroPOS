using NeuroPOS.MVVM.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class HomeVM
    {
        
        public HomeVM() {
            
    }

        #region Properties

        public ObservableCollection<Product> Products { get; set; } = [
           new () {
                Name = "Laptop",
                Price = 999.99,
                Stock = 10,
                DateAdded = DateTime.Now.AddDays(-5)
            },
            new ()
            {
                Name = "Mouse",
                Price = 25.50,
                Stock = 50,
                DateAdded = DateTime.Now.AddDays(-2)
            },
            new()
            {
                Name = "Keyboard",
                Price = 45.99,
                Stock = 5,
                DateAdded = DateTime.Now
            },
            new()
            {
                Name = "Monitor",
                Price = 199.99,
                Stock = 15,
                DateAdded = DateTime.Now.AddDays(-1)
            },
            new()
            {
                Name = "Headphones",
                Price = 79.99,
                Stock = 3,
                DateAdded = DateTime.Now.AddDays(-3)
            },
        new()
            {
                Name = "Headphones",
                Price = 79.99,
                Stock = 25,
                DateAdded = DateTime.Now.AddDays(-3)
            },
        new()
            {
                Name = "Headphones",
                Price = 79.99,
                Stock = 25,
                DateAdded = DateTime.Now.AddDays(-3)
            },
        new()
            {
                Name = "Headphones",
                Price = 79.99,
                Stock = 25,
                DateAdded = DateTime.Now.AddDays(-3)
            },
        new()
            {
                Name = "Headphones",
                Price = 79.99,
                Stock = 25,
                DateAdded = DateTime.Now.AddDays(-3)
            },
        new()
            {
                Name = "Headphones",
                Price = 79.99,
                Stock = 25,
                DateAdded = DateTime.Now.AddDays(-3)
            },
        new()
            {
                Name = "Headphones",
                Price = 79.99,
                Stock = 25,
                DateAdded = DateTime.Now.AddDays(-3)
            },
        new()
            {
                Name = "Headphones",
                Price = 79.99,
                Stock = 25,
                DateAdded = DateTime.Now.AddDays(-3)
            },
        new()
            {
                Name = "Headphones",
                Price = 79.99,
                Stock = 25,
                DateAdded = DateTime.Now.AddDays(-3)
            }];
        #endregion
        #region Methods
        public void LoadDB()
        {

        }


        #endregion

    }


}
