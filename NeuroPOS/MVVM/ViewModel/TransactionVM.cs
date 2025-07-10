using NeuroPOS.MVVM.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NeuroPOS.MVVM.ViewModel
{
    public class TransactionVM
    {
        public ObservableCollection<Transaction> Transactions { get; set; }

        public TransactionVM()
        {
            Transactions = new ObservableCollection<Transaction>
            {
                new Transaction
                {
                    Id = 1,
                    Date = DateTime.Now.AddDays(-2),
                    TransactionType = "buy",
                    TotalAmount = 200.00,
                    ItemCount = 10,
                    IsPaid = true,
                    TransactionItems = new List<Product>
                    {
                        new Product { Name = "Apples", Price = 1.5, Stock = 4 },
                        new Product { Name = "Bananas", Price = 0.8, Stock = 6 }
                    }
                },
                new Transaction
                {
                    Id = 2,
                    Date = DateTime.Now.AddDays(-1),
                    TransactionType = "sell",
                    TotalAmount = 150.00,
                    ItemCount = 5,
                    IsPaid = false,
                    TransactionItems = new List<Product>
                    {
                        new Product { Name = "Milk", Price = 3.5, Stock = 2 },
                        new Product { Name = "Bread", Price = 2.0, Stock = 3 }
                    }
                }
            };
        }
    }
}