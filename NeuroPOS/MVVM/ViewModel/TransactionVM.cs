using NeuroPOS.MVVM.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class TransactionVM
    {
          

        public TransactionVM()
        {

            Transactions = new ObservableCollection<Transaction>
            {
                new Transaction
                {
                    Id = 12345,
                    Date = new DateTime(2024, 1, 15, 10, 30, 0),
                    TransactionType = "sell",
                    TotalAmount = 25.50,
                    ItemCount = 3,
                    IsPaid = true,
                    IsExpanded = false,
                    TransactionItems = new List<Product>
                    {
                        new Product
                        {
                            Name = "Fresh Organic Apples",
                            Price = 1.50,
                            Stock = 4,
                            CategoryName = "Fruits",
                            DateAdded = DateTime.Now.AddDays(-5)
                        },
                        new Product
                        {
                            Name = "Premium Bananas",
                            Price = 0.80,
                            Stock = 6,
                            CategoryName = "Fruits",
                            DateAdded = DateTime.Now.AddDays(-3)
                        },
                        new Product
                        {
                            Name = "Organic Honey",
                            Price = 8.99,
                            Stock = 2,
                            CategoryName = "Sweeteners",
                            DateAdded = DateTime.Now.AddDays(-10)
                        }
                    }
                },
                new Transaction
                {
                    Id = 12346,
                    Date = new DateTime(2024, 1, 15, 11, 15, 0),
                    TransactionType = "buy",
                    TotalAmount = 15.75,
                    ItemCount = 2,
                    IsPaid = true,
                    IsExpanded = false,
                    TransactionItems = new List<Product>
                    {
                        new Product
                        {
                            Name = "Whole Milk 1L",
                            Price = 3.50,
                            Stock = 2,
                            CategoryName = "Dairy",
                            DateAdded = DateTime.Now.AddDays(-1)
                        },
                        new Product
                        {
                            Name = "Artisan Bread Loaf",
                            Price = 2.75,
                            Stock = 3,
                            CategoryName = "Bakery",
                            ImageUrl = "bread.png",
                            DateAdded = DateTime.Now
                        }
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
                    IsExpanded = false,
                    TransactionItems = new List<Product>
                    {
                        new Product
                        {
                            Name = "Premium Ground Coffee",
                            Price = 12.99,
                            Stock = 1,
                            CategoryName = "Beverages",
                            ImageUrl = "coffee.png",
                            DateAdded = DateTime.Now.AddDays(-7)
                        },
                        new Product
                        {
                            Name = "Organic Pasta 500g",
                            Price = 4.25,
                            Stock = 2,
                            CategoryName = "Grains",
                            ImageUrl = "pasta.png",
                            DateAdded = DateTime.Now.AddDays(-4)
                        },
                        new Product
                        {
                            Name = "Extra Virgin Olive Oil",
                            Price = 15.50,
                            Stock = 1,
                            CategoryName = "Oils",
                            ImageUrl = "oliveoil.png",
                            DateAdded = DateTime.Now.AddDays(-2)
                        },
                        new Product
                        {
                            Name = "Fresh Tomatoes",
                            Price = 2.89,
                            Stock = 8,
                            CategoryName = "Vegetables",
                            ImageUrl = "tomato.png",
                            DateAdded = DateTime.Now.AddDays(-1)
                        }
                    }
                },
                new Transaction
                {
                    Id = 12348,
                    Date = new DateTime(2024, 1, 15, 12, 45, 0),
                    TransactionType = "buy",
                    TotalAmount = 5.25,
                    ItemCount = 1,
                    IsPaid = true,
                    IsExpanded = false,
                    TransactionItems = new List<Product>
                    {
                        new Product
                        {
                            Name = "Dark Chocolate Bar",
                            Price = 5.25,
                            Stock = 3,
                            CategoryName = "Confectionery",
                            ImageUrl = "chocolate.png",
                            DateAdded = DateTime.Now.AddHours(-2)
                        }
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
                    IsExpanded = false,
                    TransactionItems = new List<Product>
                    {
                        new Product
                        {
                            Name = "Greek Yogurt 500ml",
                            Price = 4.50,
                            Stock = 2,
                            CategoryName = "Dairy",
                            ImageUrl = "yogurt.png",
                            DateAdded = DateTime.Now.AddDays(-3)
                        },
                        new Product
                        {
                            Name = "Granola Mix 300g",
                            Price = 7.25,
                            Stock = 1,
                            CategoryName = "Cereals",
                            ImageUrl = "granola.png",
                            DateAdded = DateTime.Now.AddDays(-5)
                        },
                        new Product
                        {
                            Name = "Orange Juice 1L",
                            Price = 3.75,
                            Stock = 4,
                            CategoryName = "Beverages",
                            ImageUrl = "orangejuice.png",
                            DateAdded = DateTime.Now.AddDays(-1)
                        }
                    }
                }
            };
        }



        #region Properties
        public bool AnyExpanded { get; set; } = false;
        public ObservableCollection<Transaction> Transactions { get; set; }
        #endregion

        #region Methods
        private void OnToggleExpand(object parameter)
        {
            if (parameter is Transaction transaction)
            {
                transaction.IsExpanded = !transaction.IsExpanded;
                UpdateAnyExpandedState();
            }
        }

        private void OnCollapseAll()
        {
            foreach (var transaction in Transactions)
            {
                transaction.IsExpanded = false;
            }
            UpdateAnyExpandedState();
        }

        private void UpdateAnyExpandedState()
        {
            AnyExpanded = Transactions.Any(t => t.IsExpanded);
        }

        #endregion

        #region Commands
        public ICommand ToggleExpandCommand => new Command<object>(OnToggleExpand);
        public ICommand CollapseAllCommand => new Command(OnCollapseAll);
        #endregion


    }
}