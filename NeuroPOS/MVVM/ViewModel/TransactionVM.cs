using NeuroPOS.MVVM.Model;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class TransactionVM
    {
        #region Properties
        public ObservableCollection<Transaction> Transactions { get; set; } = [];
        public ObservableCollection<Transaction> FilteredTransactions { get; set; } = [];
        #endregion

       
        public TransactionVM()
        {
            LoadTestData();
            ApplyFilters();
        }


        #region Methods
        
        private void LoadTestData()
        {
            Transactions = new ObservableCollection<Transaction>
            {
                new Transaction
                {
                    Id = 12345,
                    Date = new DateTime(2024, 1, 15, 10, 30, 0),
                    TransactionType = "Buy",
                    TotalAmount = 25.50,
                    ItemCount = 3,
                    IsPaid = true,
                    TransactionItems = new System.Collections.Generic.List<Product>
                    {
                        new Product { Name = "Laptop", Price = 999.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-5) },
                        new Product { Name = "Mouse", Price = 25.50, Stock = 2, DateAdded = DateTime.Now.AddDays(-2) },
                    }
                },
                new Transaction
                {
                    Id = 12346,
                    Date = new DateTime(2024, 1, 15, 11, 15, 0),
                    TransactionType = "sell",
                    TotalAmount = 15.75,
                    ItemCount = 2,
                    IsPaid = true,
                    TransactionItems = new System.Collections.Generic.List<Product>
                    {
                        new Product { Name = "Keyboard", Price = 45.99, Stock = 1, DateAdded = DateTime.Now },
                        new Product { Name = "Monitor", Price = 199.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-1) },
                    }
                },
                new Transaction
                {
                    Id = 12347,
                    Date = new DateTime(2024, 1, 15, 12, 0, 0),
                    TransactionType = "sell",
                    TotalAmount = 45.00,
                    ItemCount = 5,
                    IsPaid = true,
                    TransactionItems = new System.Collections.Generic.List<Product>
                    {
                        new Product { Name = "Headphones", Price = 79.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-3) },
                        new Product { Name = "Wireless Earbuds", Price = 59.99, Stock = 2, DateAdded = DateTime.Now.AddDays(-5) },
                        new Product { Name = "Smart Watch", Price = 199.99, Stock = 2, DateAdded = DateTime.Now.AddDays(-10) },
                    }
                },
                new Transaction
                {
                    Id = 12348,
                    Date = new DateTime(2024, 1, 15, 12, 45, 0),
                    TransactionType = "sell",
                    TotalAmount = 5.25,
                    ItemCount = 1,
                    IsPaid = true,
                    TransactionItems = new System.Collections.Generic.List<Product>
                    {
                        new Product { Name = "Bluetooth Speaker", Price = 89.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-2) },
                    }
                },
                new Transaction
                {
                    Id = 12349,
                    Date = new DateTime(2024, 1, 15, 13, 30, 0),
                    TransactionType = "sell",
                    TotalAmount = 35.00,
                    ItemCount = 4,
                    IsPaid = false,
                    TransactionItems = new System.Collections.Generic.List<Product>
                    {
                        new Product { Name = "USB-C Cable", Price = 12.99, Stock = 2, DateAdded = DateTime.Now.AddDays(-7) },
                        new Product { Name = "Power Bank", Price = 39.99, Stock = 2, DateAdded = DateTime.Now.AddDays(-14) },
                    }
                },
                new Transaction
                {
                    Id = 12350,
                    Date = new DateTime(2024, 1, 15, 14, 15, 0),
                    TransactionType = "sell",
                    TotalAmount = 18.50,
                    ItemCount = 2,
                    IsPaid = true,
                    TransactionItems = new System.Collections.Generic.List<Product>
                    {
                        new Product { Name = "Laptop Stand", Price = 29.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-1) },
                        new Product { Name = "Mechanical Keyboard", Price = 129.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-21) },
                    }
                },
                new Transaction
                {
                    Id = 12351,
                    Date = new DateTime(2024, 1, 15, 15, 0, 0),
                    TransactionType = "sell",
                    TotalAmount = 22.75,
                    ItemCount = 3,
                    IsPaid = false,
                    TransactionItems = new System.Collections.Generic.List<Product>
                    {
                        new Product { Name = "Gaming Mouse", Price = 49.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-4) },
                        new Product { Name = "Monitor", Price = 249.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-12) },
                    }
                },
                new Transaction
                {
                    Id = 12352,
                    Date = new DateTime(2024, 1, 15, 15, 45, 0),
                    TransactionType = "sell",
                    TotalAmount = 55.25,
                    ItemCount = 6,
                    IsPaid = true,
                    TransactionItems = new System.Collections.Generic.List<Product>
                    {
                        new Product { Name = "Desk Lamp", Price = 34.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-6) },
                        new Product { Name = "External SSD", Price = 119.99, Stock = 1, DateAdded = DateTime.Now.AddDays(-9) },
                    }
                },
            };
        }

        public void ApplyFilters()
        {
            FilteredTransactions = new ObservableCollection<Transaction>(Transactions);
        }

      
        #endregion

        #region Commands
      
        #endregion
    }
}
