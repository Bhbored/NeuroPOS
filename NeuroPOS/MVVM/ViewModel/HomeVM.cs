using NeuroPOS.MVVM.Model;
using PropertyChanged;
using Syncfusion.Maui.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using ListSortDirection = Syncfusion.Maui.DataSource.ListSortDirection;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class HomeVM
    {
        public HomeVM()
        {
          SortProduct();
        }

        #region Enums

        public enum SortDirectionState
        {
            None,
            Ascending,
            Descending
        }

        #endregion

        #region Properties
        public ObservableCollection<object> SelectedItems { get; set; } = [];
        public IList<object> SelectedProducts { get; set; } = [];
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>
        {
            new Product() { Id = 1, Name = "Laptop", Price = 999.99, Stock = 10, DateAdded = DateTime.Now.AddDays(-5) },
            new Product() { Id = 2, Name = "Mouse", Price = 25.50, Stock = 50, DateAdded = DateTime.Now.AddDays(-2) },
            new Product() { Id = 3, Name = "Keyboard", Price = 45.99, Stock = 5, DateAdded = DateTime.Now },
            new Product() { Id = 4, Name = "Monitor", Price = 199.99, Stock = 15, DateAdded = DateTime.Now.AddDays(-1) },
            new Product() { Id = 5, Name = "Headphones", Price = 79.99, Stock = 3, DateAdded = DateTime.Now.AddDays(-3) },
            new Product() { Id = 6, Name = "Wireless Earbuds", Price = 59.99, Stock = 50, DateAdded = DateTime.Now.AddDays(-5) },
            new Product() { Id = 7, Name = "Smart Watch", Price = 199.99, Stock = 15, DateAdded = DateTime.Now.AddDays(-10) },
            new Product() { Id = 8, Name = "Bluetooth Speaker", Price = 89.99, Stock = 30, DateAdded = DateTime.Now.AddDays(-2) },
            new Product() { Id = 9, Name = "USB-C Cable", Price = 12.99, Stock = 100, DateAdded = DateTime.Now.AddDays(-7) },
            new Product() { Id = 10, Name = "Power Bank", Price = 39.99, Stock = 40, DateAdded = DateTime.Now.AddDays(-14) },
            new Product() { Id = 11, Name = "Laptop Stand", Price = 29.99, Stock = 25, DateAdded = DateTime.Now.AddDays(-1) },
            new Product() { Id = 12, Name = "Mechanical Keyboard", Price = 129.99, Stock = 10, DateAdded = DateTime.Now.AddDays(-21) },
            new Product() { Id = 13, Name = "Gaming Mouse", Price = 49.99, Stock = 35, DateAdded = DateTime.Now.AddDays(-4) },
            new Product() { Id = 14, Name = "Monitor", Price = 249.99, Stock = 8, DateAdded = DateTime.Now.AddDays(-12) },
            new Product() { Id = 15, Name = "Desk Lamp", Price = 34.99, Stock = 20, DateAdded = DateTime.Now.AddDays(-6) },
            new Product() { Id = 16, Name = "External SSD", Price = 119.99, Stock = 12, DateAdded = DateTime.Now.AddDays(-9) },
            new Product() { Id = 17, Name = "Wireless Charger", Price = 24.99, Stock = 45, DateAdded = DateTime.Now.AddDays(-3) },
            new Product() { Id = 18, Name = "Noise Cancelling Headphones", Price = 299.99, Stock = 5, DateAdded = DateTime.Now.AddDays(-15) },
            new Product() { Id = 19, Name = "Webcam", Price = 79.99, Stock = 18, DateAdded = DateTime.Now.AddDays(-8) },
            new Product() { Id = 20, Name = "Microphone", Price = 149.99, Stock = 7, DateAdded = DateTime.Now.AddDays(-11) },
            new Product() { Id = 21, Name = "Smartphone Holder", Price = 9.99, Stock = 60, DateAdded = DateTime.Now.AddDays(-1) },
            new Product() { Id = 22, Name = "HDMI Cable", Price = 14.99, Stock = 75, DateAdded = DateTime.Now.AddDays(-5) },
            new Product() { Id = 23, Name = "Router", Price = 129.99, Stock = 9, DateAdded = DateTime.Now.AddDays(-17) },
            new Product() { Id = 24, Name = "Fitness Tracker", Price = 79.99, Stock = 22, DateAdded = DateTime.Now.AddDays(-4) },
            new Product() { Id = 25, Name = "Tablet Stand", Price = 19.99, Stock = 35, DateAdded = DateTime.Now.AddDays(-2) }
        };

        public DataSource DataSource { get; set; } = new DataSource()
        {
            Source = new ObservableCollection<Product>()
        };

        private SortDirectionState _sortState = SortDirectionState.None;
        public SortDirectionState SortState
        {
            get => _sortState;
            set
            {
                if (_sortState != value)
                {
                    _sortState = value;
                    ApplySorting();
                }
            }
        }

        public string SortLabel => SortState switch
        {
            SortDirectionState.Ascending => "Sort: A → Z",
            SortDirectionState.Descending => "Sort: Z → A",
            SortDirectionState.None => "Sort: None",
            _ => "Sort"
        };

        #endregion

        #region Methods

        public void ApplySorting()
        {
            DataSource.SortDescriptors.Clear();

            if (SortState == SortDirectionState.Ascending)
            {
                DataSource.SortDescriptors.Add(new SortDescriptor()
                {
                    PropertyName = "Name",
                    Direction = ListSortDirection.Ascending
                });
            }
            else if (SortState == SortDirectionState.Descending)
            {
                DataSource.SortDescriptors.Add(new SortDescriptor()
                {
                    PropertyName = "Name",
                    Direction = ListSortDirection.Descending
                });
            }
            // If SortState == None, keep it unsorted.
        }

        public void LoadDB()
        {
            // Implement DB loading here if needed
        }

        #endregion

        #region Commands

        public ICommand ToggleSortCommand => new Command(() =>
        {
            SortState = SortState switch
            {
                SortDirectionState.None => SortDirectionState.Ascending,
                SortDirectionState.Ascending => SortDirectionState.Descending,
                SortDirectionState.Descending => SortDirectionState.None,
                _ => SortDirectionState.None
            };
        });

        #endregion

        #region Tasks

        public void SortProduct()
        {
            DataSource = new DataSource() { Source = Products };
            DataSource.Refresh();

        }
        #endregion
    }
}