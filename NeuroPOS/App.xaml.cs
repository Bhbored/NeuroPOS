using NeuroPOS.Data;
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
        public static BaseRepository<Order>? OrderRepo { get; private set; }

        public static HomeVM? HomeRepo { get; set; }
        public static TransactionVM? TransactionRepoVM { get; set; }
        public static InventoryVM? InventoryRepo { get; set; }
        #endregion
        public App(BaseRepository<CashRegister> _cashregister,
            BaseRepository<Category> _category,
            BaseRepository<Contact> _contact,
            BaseRepository<Product> _product,
            BaseRepository<Transaction> _transaction, BaseRepository<Order> _order, HomeVM _homeVM,
            TransactionVM _transactionVM, InventoryVM _inventoryVM)
        {
            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXlceHRTQ2ZYWUN/XkFWYEk=");
            CashRegisterRepo = _cashregister;
            CategoryRepo = _category;
            ContactRepo = _contact;
            ProductRepo = _product;
            TransactionRepo = _transaction;
            OrderRepo = _order;
            _ = ProductTestData();
            _ = CategoryTestData();
            HomeRepo = _homeVM;
            TransactionRepoVM = _transactionVM;
            InventoryRepo = _inventoryVM;

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
                        ProductRepo?.SaveItem(product);
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
                        CategoryRepo?.SaveItem(category);
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

        #endregion


    }

}

