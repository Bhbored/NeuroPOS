﻿using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using Syncfusion.Licensing;
using System.Diagnostics;
using Contact = NeuroPOS.MVVM.Model.Contact;

namespace NeuroPOS
{
    public partial class App : Application
    {
        #region injection
        public static BaseRepository<CashRegister>? CashRegisterRepo { get; private set; }
        public static BaseRepository<Category>? CategoryRepo { get; private set; }
        public static BaseRepository<Contact>? ContactRepo { get; private set; }
        public static BaseRepository<Product>? ProductRepo { get; private set; }
        public static BaseRepository<Transaction>? TransactionRepo { get; private set; }
        public static BaseRepository<TransactionLine>? TransactionLineRepo { get; private set; }
        public static BaseRepository<Order>? OrderRepo { get; private set; }

        public static HomeVM? HomeVM { get; set; }
        public static TransactionVM? TransactionVM { get; set; }
        public static InventoryVM? InventoryVM { get; set; }
        public static OrderVM? OrderVM { get; set; }
        public static ContactVM? ContactVM { get; set; }
        #endregion
        public App(BaseRepository<CashRegister> _cashregister,
            BaseRepository<Category> _category,
            BaseRepository<Contact> _contact,
            BaseRepository<Product> _product,
            BaseRepository<Transaction> _transaction, BaseRepository<TransactionLine> _transactionLine, BaseRepository<Order> _order, HomeVM _homeVM,
            TransactionVM _transactionVM, InventoryVM _inventoryVM, ContactVM _contactVM, OrderVM _orderVM)
        {
            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXlceHRTQ2ZYWUN/XkFWYEk=");
            CashRegisterRepo = _cashregister;
            CategoryRepo = _category;
            ContactRepo = _contact;
            ProductRepo = _product;
            TransactionRepo = _transaction;
            TransactionLineRepo = _transactionLine;
            OrderRepo = _order;
            _ = ProductTestData();
            _ = CategoryTestData();
            _= ContactTestData();
            HomeVM = _homeVM;
            TransactionVM = _transactionVM;
            InventoryVM = _inventoryVM;
            OrderVM = _orderVM;
            ContactVM = _contactVM;

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        #region Test Data
        public async Task ProductTestData()
        {
            try
            {
                var Products = ProductRepo?.GetItems();
                Debug.WriteLine($"Current products in database: {Products?.Count ?? 0}");

                // Only add test data if database is empty
                if (Products == null || Products.Count == 0)
                {
                    var testProducts = new List<Product>
                    {
                        new Product
                        {
                            Name = "Coca-Cola 500ml",
                            Price = 1.5,
                            Cost = 0.8,
                            Stock = 50,
                            CategoryName = "Beverages",
                            CategoryId = 1,
                            DateAdded = DateTime.Now.AddDays(-3)
                        },
                        new Product
                        {
                            Name = "Bread - Whole Wheat",
                            Price = 2.0,
                            Cost = 1.0,
                            Stock = 30,
                            CategoryName = "Bakery",
                            CategoryId = 2,
                            DateAdded = DateTime.Now.AddDays(-1)
                        },
                        new Product
                        {
                            Name = "Eggs - Dozen",
                            Price = 3.2,
                            Cost = 2.4,
                            Stock = 60,
                            CategoryName = "Dairy",
                            CategoryId = 3,
                            DateAdded = DateTime.Now
                        },
                        new Product
                        {
                            Name = "Toothpaste - Mint",
                            Price = 1.2,
                            Cost = 0.6,
                            Stock = 100,
                            CategoryName = "Personal Care",
                            CategoryId = 4,
                            DateAdded = DateTime.Now.AddDays(-5)
                        },
                        new Product
                        {
                            Name = "Notebook A5 - 100 pages",
                            Price = 2.5,
                            Cost = 1.5,
                            Stock = 20,
                            CategoryName = "Stationery",
                            CategoryId = 5,
                            DateAdded = DateTime.Now.AddDays(-2)
                        }
                    };

                    foreach (var product in testProducts)
                    {
                        ProductRepo?.InsertItem(product);
                        Debug.WriteLine($"Product {product.Name} added to the database.");
                    }

                    Debug.WriteLine($"Added {testProducts.Count} test products to database.");
                }
                else
                {
                    Debug.WriteLine($"Database already contains {Products.Count} products. Skipping test data insertion.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in fillData: {ex.Message}");
            }
        }

        public async Task CategoryTestData()
        {
            try
            {
                var categories = CategoryRepo?.GetItems();
                Debug.WriteLine($"Current categories in database: {categories?.Count ?? 0}");

                // Only add test data if database is empty
                if (categories == null || categories.Count == 0)
                {
                    var testCategories = new List<Category>
            {
                new Category
                {
                    Name = "Beverages",
                    Description = "Soft drinks, juices, and water",
                    ProductCount = 0,
                },
                new Category
                {
                    Name = "Bakery",
                    Description = "Breads, cakes, and pastries",
                    ProductCount = 0,
                },
                new Category
                {
                    Name = "Dairy",
                    Description = "Milk, eggs, cheese, and yogurt",
                    ProductCount = 0,
                },
                new Category
                {
                    Name = "Personal Care",
                    Description = "Hygiene and grooming products",
                    ProductCount = 0,
                },
               
            };

                    foreach (var category in testCategories)
                    {
                        CategoryRepo?.InsertItem(category);
                        Debug.WriteLine($"Category {category.Name} added to the database.");
                    }

                    Debug.WriteLine($"Added {testCategories.Count} test categories to database.");
                }
                else
                {
                    Debug.WriteLine($"Database already contains {categories.Count} categories. Skipping test data insertion.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CategoryTestData: {ex.Message}");
            }
        }

        public async Task ContactTestData()
        {
            try
            {
                var contacts = App.ContactRepo?.GetItemsWithChildren();
                Debug.WriteLine($"Current contacts in database: {contacts?.Count ?? 0}");

                if (contacts == null || contacts.Count == 0)
                {
                    var testContacts = new List<Contact>();

                    for (int i = 1; i <= 5; i++)
                    {
                        var contact = new Contact
                        {
                            Name = $"Contact {i}",
                            Email = $"contact{i}@example.com",
                            PhoneNumber = $"555000{i:D3}",
                            Address = $"Street {i}, City",
                            DateAdded = DateTime.Now.AddDays(-i),
                            Transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            Date = DateTime.Now.AddDays(-i),
                            IsPaid = true,
                            TransactionType = "sell",
                            TotalAmount = 50 * i,
                            ItemCount = 2,
                            Lines = new List<TransactionLine>
                            {
                                new TransactionLine
                                {
                                    Name = $"Product A{i}",
                                    Price = 10 * i,
                                    Cost = 6 * i,
                                    Stock = 2,
                                    CategoryName = "General",
                                    ImageUrl = "emptyproduct.png",
                                    DateAdded = DateTime.Now.AddDays(-i)
                                },
                                new TransactionLine
                                {
                                    Name = $"Product B{i}",
                                    Price = 15 * i,
                                    Cost = 9 * i,
                                    Stock = 1,
                                    CategoryName = "General",
                                    ImageUrl = "emptyproduct.png",
                                    DateAdded = DateTime.Now.AddDays(-i)
                                }
                            }
                        },
                        new Transaction
                        {
                            Date = DateTime.Now.AddDays(-i + 1),
                            IsPaid = false,
                            TransactionType = "sell",
                            TotalAmount = 60 * i,
                            ItemCount = 2,
                            Lines = new List<TransactionLine>
                            {
                                new TransactionLine
                                {
                                    Name = $"Product C{i}",
                                    Price = 20 * i,
                                    Cost = 12 * i,
                                    Stock = 1,
                                    CategoryName = "Uncategorized",
                                    ImageUrl = "emptyproduct.png",
                                    DateAdded = DateTime.Now.AddDays(-i + 1)
                                },
                                new TransactionLine
                                {
                                    Name = $"Product D{i}",
                                    Price = 10 * i,
                                    Cost = 7 * i,
                                    Stock = 2,
                                    CategoryName = "Uncategorized",
                                    ImageUrl = "emptyproduct.png",
                                    DateAdded = DateTime.Now.AddDays(-i + 1)
                                }
                            }
                        }
                    }
                        };

                        testContacts.Add(contact);
                    }

                    foreach (var contact in testContacts)
                    {
                        App.ContactRepo?.InsertItemWithChildren(contact, true);
                        Debug.WriteLine($"Inserted contact: {contact.Name} with {contact.Transactions?.Count ?? 0} transactions.");
                    }

                    Debug.WriteLine($"✅ Added {testContacts.Count} test contacts with transactions and transaction lines.");
                }
                else
                {
                    Debug.WriteLine($"ℹ️ Database already contains {contacts.Count} contacts. Skipping insertion.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Error in ContactTestData: {ex.Message}");
            }
        }






        #endregion


    }

}

