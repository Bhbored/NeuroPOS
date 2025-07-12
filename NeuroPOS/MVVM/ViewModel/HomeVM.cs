using NeuroPOS.MVVM.Model;
using PropertyChanged;
using Syncfusion.Maui.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using ListSortDirection = Syncfusion.Maui.DataSource.ListSortDirection;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class HomeVM : INotifyPropertyChanged
    {
        public HomeVM()
        {
            LoadDB();
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

        private ObservableCollection<Product> _currentOrderItems = new ObservableCollection<Product>();

        public ObservableCollection<Product> CurrentOrderItems
        {
            get => _currentOrderItems;
            set
            {
                if (_currentOrderItems != null)
                {
                    _currentOrderItems.CollectionChanged -= OnCurrentOrderItemsChanged;
                    // Unsubscribe from individual item property changes
                    foreach (var item in _currentOrderItems)
                    {
                        if (item is INotifyPropertyChanged notifyItem)
                            notifyItem.PropertyChanged -= OnOrderItemPropertyChanged;
                    }
                }

                _currentOrderItems = value;

                if (_currentOrderItems != null)
                {
                    _currentOrderItems.CollectionChanged += OnCurrentOrderItemsChanged;
                    // Subscribe to individual item property changes
                    foreach (var item in _currentOrderItems)
                    {
                        if (item is INotifyPropertyChanged notifyItem)
                            notifyItem.PropertyChanged += OnOrderItemPropertyChanged;
                    }
                }

                OnPropertyChanged();
                NotifyCalculatedPropertiesChanged();
            }
        }
        public ObservableCollection<object> SelectedItems { get; set; } = [];
        public IList<object> SelectedProducts { get; set; } = [];

        // Backup collection to persist selections across ListView refreshes
        private ObservableCollection<int> _persistentSelectedIds = new ObservableCollection<int>();

        private ObservableCollection<Category> _categories = new ObservableCollection<Category>
        {
            new Category { Id = 1, Name = "Fruits"},
            new Category { Id = 2, Name = "Vegetables" },
            new Category { Id = 3, Name = "Drinks" },
            new Category { Id = 4, Name = "Snacks" }
        };


        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged(); // This notifies the UI that Categories itself changed
                }
            }
        }

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>
{
    new Product() { Id = 1, Name = "Laptop", CategoryName = "Fruits", Price = 999.99, Stock = 0, DateAdded = DateTime.Now.AddDays(-5), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 2, Name = "Mouse", CategoryName = "Fruits", Price = 25.50, Stock = 0, DateAdded = DateTime.Now.AddDays(-2), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 3, Name = "Keyboard", CategoryName = "Vegetables", Price = 45.99, Stock = 5, DateAdded = DateTime.Now, ImageUrl = "emptyproduct.png" },
    new Product() { Id = 4, Name = "Monitor", CategoryName = "Vegetables", Price = 199.99, Stock = 15, DateAdded = DateTime.Now.AddDays(-1), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 5, Name = "Headphones", CategoryName = "Drinks", Price = 79.99, Stock = 3, DateAdded = DateTime.Now.AddDays(-3), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 6, Name = "Wireless Earbuds", CategoryName = "Drinks", Price = 59.99, Stock = 50, DateAdded = DateTime.Now.AddDays(-5), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 7, Name = "Smart Watch", CategoryName = "Snacks", Price = 199.99, Stock = 15, DateAdded = DateTime.Now.AddDays(-10), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 8, Name = "Bluetooth Speaker", CategoryName = "Snacks", Price = 89.99, Stock = 30, DateAdded = DateTime.Now.AddDays(-2), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 9, Name = "Tablet", CategoryName = "Fruits", Price = 499.99, Stock = 20, DateAdded = DateTime.Now.AddDays(-7), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 10, Name = "Smartphone", CategoryName = "Fruits", Price = 799.99, Stock = 25, DateAdded = DateTime.Now.AddDays(-4), ImageUrl = "emptyproduct.png" },
      new Product() { Id = 11, Name = "Apple", CategoryName = "Fruits", Price = 0.99, Stock = 50, DateAdded = DateTime.Now.AddDays(-2) },
    new Product() { Id = 12, Name = "Banana", CategoryName = "Fruits", Price = 0.49, Stock = 100, DateAdded = DateTime.Now.AddDays(-1) },
    new Product() { Id = 13, Name = "Orange", CategoryName = "Fruits", Price = 0.79, Stock = 75, DateAdded = DateTime.Now.AddDays(-3) },
    
    // Vegetables (CategoryId = 2)
    new Product() { Id = 14, Name = "Carrot", CategoryName = "Vegetables", Price = 0.89, Stock = 60, DateAdded = DateTime.Now.AddDays(-4) },
    new Product() { Id = 15, Name = "Broccoli", CategoryName = "Vegetables", Price = 1.29, Stock = 40, DateAdded = DateTime.Now.AddDays(-5) },
    new Product() { Id = 16, Name = "Spinach", CategoryName = "Vegetables", Price = 1.99, Stock = 30, DateAdded = DateTime.Now.AddDays(-1) },
    
    // Drinks (CategoryId = 3)
    new Product() { Id = 17, Name = "Water", CategoryName = "Drinks", Price = 1.49, Stock = 200, DateAdded = DateTime.Now.AddDays(-7) },
    new Product() { Id = 18, Name = "Orange Juice", CategoryName = "Drinks", Price = 2.99, Stock = 50, DateAdded = DateTime.Now.AddDays(-2) },
    new Product() { Id = 19, Name = "Soda", CategoryName = "Drinks", Price = 1.79, Stock = 80, DateAdded = DateTime.Now.AddDays(-3) },
    
    // Snacks (CategoryId = 4)
    new Product() { Id = 20, Name = "Chips", CategoryName = "Snacks", Price = 2.49, Stock = 45, DateAdded = DateTime.Now.AddDays(-4) },
    new Product() { Id = 21, Name = "Cookies", CategoryName = "Snacks", Price = 3.99, Stock = 35, DateAdded = DateTime.Now.AddDays(-6) },
    new Product() { Id = 22, Name = "Nuts", CategoryName = "Snacks", Price = 4.49, Stock = 25, DateAdded = DateTime.Now.AddDays(-1) },

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

        // Calculated properties for order summary
        public double Subtotal
        {
            get
            {
                var result = CurrentOrderItems?.Sum(item => item.Price * item.Stock) ?? 0;
                return result;
            }
        }

        public double Tax
        {
            get
            {
                var result = Subtotal * 0.05;
                return result;
            }
        }

        public double Discount { get; set; } = 0; // Allow user to set discount

        public double Total
        {
            get
            {
                var result = Subtotal + Tax - Discount;
                return result;
            }
        }

        public string OrderDateTime => DateTime.Now.ToString("MMM dd, yyyy - hh:mm tt");

        #endregion

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Methods


        private void OnCurrentOrderItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

            // Handle subscription/unsubscription for added/removed items
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged notifyItem)
                    {
                        notifyItem.PropertyChanged += OnOrderItemPropertyChanged;
                    }
                }
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged notifyItem)
                        notifyItem.PropertyChanged -= OnOrderItemPropertyChanged;
                }
            }

            // Trigger property change notifications for calculated properties
            NotifyCalculatedPropertiesChanged();
            OnPropertyChanged(nameof(HasSelectedItems));
        }

        private void OnOrderItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // When any property of an order item changes (especially Stock), update calculated properties
            if (e.PropertyName == nameof(Product.Stock) || e.PropertyName == nameof(Product.Price))
            {

                NotifyCalculatedPropertiesChanged();
            }
        }

        public void NotifyCalculatedPropertiesChanged()
        {

            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(Total));
        }

        public void NotifySelectionChanged()
        {
            OnPropertyChanged(nameof(HasSelectedItems));
        }

        public void AddToCurrentOrder(Product product, bool fromListViewSelection = false)
        {
            if (product != null)
            {


                // Check if product is already in the order
                var existingItem = CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                if (existingItem == null)
                {
                    // Add new product to order
                    var newOrderItem = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Stock = 1, // Set quantity to 1 for order
                        ImageUrl = product.ImageUrl,
                        DateAdded = product.DateAdded,
                        CategoryName = product.CategoryName
                    };

                    CurrentOrderItems.Add(newOrderItem);

                    // Store original stock for this product
                    UpdateProductDisplayStock(product.Id);

                    // Add to persistent selection
                    AddToPersistentSelection(product.Id);

                    // FORCE notification for calculated properties after adding item
                    NotifyCalculatedPropertiesChanged();


                    // Only add to ListView selection if this wasn't called from ListView selection event
                    if (!fromListViewSelection && PageReference is ContentPage page)
                    {
                        var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                        if (listView != null)
                        {
                            var originalProduct = DataSource.DisplayItems?.Cast<Product>().FirstOrDefault(p => p.Id == product.Id);
                            if (originalProduct != null && !listView.SelectedItems.Contains(originalProduct))
                            {
                                listView.SelectedItems.Add(originalProduct);




                                // Add to SelectedItems collection for HasSelectedItems property
                                if (!SelectedItems.Contains(originalProduct))
                                {
                                    SelectedItems.Add(originalProduct);
                                    OnPropertyChanged(nameof(HasSelectedItems));

                                }
                            }
                        }
                    }

                    // Force property change notification
                    OnPropertyChanged(nameof(CurrentOrderItems));
                }
                else
                {
                    // Increase quantity if already exists (with stock validation)
                    var originalStock = GetOriginalStock(product.Id);
                    var currentCartQuantity = existingItem.Stock;

                    // Only increment if there's still available stock
                    if (currentCartQuantity < originalStock)
                    {
                        existingItem.Stock += 1;

                        // Update the display stock to reflect the change
                        UpdateProductDisplayStock(product.Id);

                        // FORCE notification for calculated properties after incrementing existing item
                        NotifyCalculatedPropertiesChanged();
                    }
                }
            }

        }

        public void RemoveFromCurrentOrder(Product product)
        {
            if (product != null)
            {
                var existingItem = CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                if (existingItem != null)
                {
                    CurrentOrderItems.Remove(existingItem);

                    // Update the display stock to reflect the removal
                    UpdateProductDisplayStock(product.Id);

                    // Remove from persistent selection
                    RemoveFromPersistentSelection(product.Id);

                    // FORCE notification for calculated properties after removing item
                    NotifyCalculatedPropertiesChanged();


                    // Also remove from ListView selection (prevent event recursion)
                    if (PageReference is ContentPage page)
                    {
                        var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                        if (listView != null && listView.SelectedItems != null)
                        {
                            // Temporarily disconnect the selection changed event
                            var homePage = page as NeuroPOS.MVVM.View.HomePage;
                            if (homePage != null)
                            {
                                listView.SelectionChanged -= homePage.ListView_SelectionChanged;
                            }

                            // Find the original product in the DataSource and remove it from selection
                            var originalProduct = DataSource.DisplayItems?.Cast<Product>().FirstOrDefault(p => p.Id == product.Id);
                            if (originalProduct != null && listView.SelectedItems.Contains(originalProduct))
                            {
                                listView.SelectedItems.Remove(originalProduct);

                            }



                            // Reconnect the selection changed event
                            if (homePage != null)
                            {
                                listView.SelectionChanged += homePage.ListView_SelectionChanged;
                            }
                        }
                    }

                    // Also remove from SelectedItems collection
                    var selectedProduct = SelectedItems.FirstOrDefault(x => x is Product p && p.Id == product.Id);
                    if (selectedProduct != null)
                    {
                        SelectedItems.Remove(selectedProduct);

                    }

                    // Notify property changes
                    OnPropertyChanged(nameof(HasSelectedItems));
                }
            }
        }

        public void IncrementQuantity(Product product)
        {
            if (product != null)
            {
                var existingItem = CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                if (existingItem != null)
                {
                    // Get the original stock and validate against it
                    var originalStock = GetOriginalStock(product.Id);
                    var currentCartQuantity = existingItem.Stock;

                    // Only increment if current cart quantity is less than original stock
                    if (currentCartQuantity < originalStock)
                    {
                        existingItem.Stock += 1;

                        // Update the display stock to reflect the change
                        UpdateProductDisplayStock(product.Id);

                        // FORCE manual notification to test if bindings work
                        NotifyCalculatedPropertiesChanged();
                    }
                    // If no available stock, do nothing (don't increment)
                }
            }
        }

        public void DecrementQuantity(Product product)
        {
            if (product != null)
            {
                var existingItem = CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                if (existingItem != null && existingItem.Stock > 1)
                {
                    existingItem.Stock -= 1;

                    // Update the display stock to reflect the change
                    UpdateProductDisplayStock(product.Id);

                    // FORCE manual notification to test if bindings work
                    NotifyCalculatedPropertiesChanged();
                }
                else if (existingItem != null && existingItem.Stock == 1)
                {
                    // If quantity would go to 0, remove the item entirely
                    RemoveFromCurrentOrder(product);
                }
            }
        }

        /// <summary>
        /// Updates the display stock for a product to show available stock
        /// </summary>
        private void UpdateProductDisplayStock(int productId)
        {
            var originalProduct = Products.FirstOrDefault(p => p.Id == productId);
            if (originalProduct != null)
            {
                // Store the original stock if not already stored
                if (!_originalStocks.ContainsKey(productId))
                {
                    _originalStocks[productId] = originalProduct.Stock;
                }

                // Calculate available stock (original - cart quantity)
                var cartItem = CurrentOrderItems.FirstOrDefault(x => x.Id == productId);
                var cartQuantity = cartItem?.Stock ?? 0;
                var originalStock = _originalStocks[productId];
                var availableStock = Math.Max(0, originalStock - cartQuantity);

                // Update the display stock
                originalProduct.Stock = availableStock;
            }
        }

        /// <summary>
        /// Gets the original stock for a product (before any cart modifications)
        /// </summary>
        public int GetOriginalStock(int productId)
        {
            if (_originalStocks.ContainsKey(productId))
            {
                return _originalStocks[productId];
            }

            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                _originalStocks[productId] = product.Stock;
                return product.Stock;
            }

            return 0;
        }

        /// <summary>
        /// Gets the available stock for a product (original stock - cart quantity)
        /// </summary>
        public int GetAvailableStock(int productId)
        {
            var originalStock = GetOriginalStock(productId);
            var cartItem = CurrentOrderItems.FirstOrDefault(x => x.Id == productId);
            var cartQuantity = cartItem?.Stock ?? 0;

            return Math.Max(0, originalStock - cartQuantity);
        }

        // Dictionary to store original stock values before any cart modifications
        private Dictionary<int, int> _originalStocks = new Dictionary<int, int>();

        /// <summary>
        /// Resets all product stocks to their original values
        /// </summary>
        private void ResetAllProductStocks()
        {
            // Reset all products to their original stock values
            foreach (var kvp in _originalStocks)
            {
                var product = Products.FirstOrDefault(p => p.Id == kvp.Key);
                if (product != null)
                {
                    product.Stock = kvp.Value;
                }
            }

            // Clear the original stocks dictionary
            _originalStocks.Clear();
        }

        public void ClearAllSelections()
        {
            // Clear the ListView selections
            SelectedItems.Clear();

            // Clear the persistent selection backup
            _persistentSelectedIds.Clear();

            // Clear the cart
            CurrentOrderItems.Clear();

            // Reset all product stocks to their original values
            ResetAllProductStocks();

            // Force notification of calculated properties
            NotifyCalculatedPropertiesChanged();

            // Notify that selection state has changed
            OnPropertyChanged(nameof(HasSelectedItems));

            // Update the count display
            UpdateSelectedItemsCountDisplay();

            // Clear ListView selection through page reference (prevent event recursion)
            if (PageReference is ContentPage page)
            {
                var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                if (listView != null)
                {
                    // Temporarily disconnect the selection changed event
                    var homePage = page as NeuroPOS.MVVM.View.HomePage;
                    if (homePage != null)
                    {
                        listView.SelectionChanged -= homePage.ListView_SelectionChanged;
                    }

                    listView.SelectedItems?.Clear();

                    // Reconnect the selection changed event
                    if (homePage != null)
                    {
                        listView.SelectionChanged += homePage.ListView_SelectionChanged;
                    }
                }



                // Note: We don't clear searchFilterValue here as it's for autocomplete filtering, not ListView selection
            }

            // Notify property changes
            OnPropertyChanged(nameof(HasSelectedItems));


        }

        // Property to check if any items are selected (for the clear button visibility)
        public bool HasSelectedItems => _persistentSelectedIds?.Count > 0;

        // Reference to the page for callbacks
        public object PageReference { get; set; }

        public void AddSelectedItemsToOrder()
        {
            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                {
                    if (item is Product product)
                    {
                        AddToCurrentOrder(product, fromListViewSelection: false);
                    }
                }
                // Clear selection after adding
                SelectedItems.Clear();
            }
        }

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

            // Restore selections after sorting
            RestoreListViewSelections();

            // Update selected count after sorting
            UpdateSelectedItemsCountDisplay();
        }

        /// <summary>
        /// Adds a product ID to persistent selection backup
        /// </summary>
        public void AddToPersistentSelection(int productId)
        {
            if (!_persistentSelectedIds.Contains(productId))
            {
                _persistentSelectedIds.Add(productId);
                OnPropertyChanged(nameof(HasSelectedItems));
                UpdateSelectedItemsCountDisplay();
            }
        }

        /// <summary>
        /// Removes a product ID from persistent selection backup
        /// </summary>
        public void RemoveFromPersistentSelection(int productId)
        {
            if (_persistentSelectedIds.Contains(productId))
            {
                _persistentSelectedIds.Remove(productId);
                OnPropertyChanged(nameof(HasSelectedItems));
                UpdateSelectedItemsCountDisplay();
            }
        }

        /// <summary>
        /// Checks if a product ID is in persistent selection
        /// </summary>
        public bool IsInPersistentSelection(int productId)
        {
            return _persistentSelectedIds.Contains(productId);
        }

        /// <summary>
        /// Updates the selected items count display using persistent selection count
        /// </summary>
        public void UpdateSelectedItemsCountDisplay()
        {
            if (PageReference is ContentPage page)
            {
                var selectedValueLabel = page.FindByName("selectedValue") as Label;
                if (selectedValueLabel != null)
                {
                    selectedValueLabel.Text = _persistentSelectedIds.Count.ToString();
                }
            }
        }

        /// <summary>
        /// Refreshes all UI elements and ensures counts are synchronized
        /// </summary>
        public void RefreshUI()
        {
            UpdateSelectedItemsCountDisplay();
            OnPropertyChanged(nameof(HasSelectedItems));
            RestoreListViewSelections();
        }

        /// <summary>
        /// Restores ListView selections after sorting or filtering using persistent backup
        /// </summary>
        public void RestoreListViewSelections()
        {
            if (PageReference is ContentPage page && _persistentSelectedIds?.Count > 0)
            {
                var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                if (listView != null)
                {
                    // Temporarily disconnect selection event to avoid recursion
                    var homePage = page as NeuroPOS.MVVM.View.HomePage;
                    if (homePage != null)
                    {
                        listView.SelectionChanged -= homePage.ListView_SelectionChanged;
                    }

                    // Clear current selection
                    listView.SelectedItems?.Clear();
                    SelectedItems.Clear();

                    // Restore selections based on persistent IDs
                    foreach (var productId in _persistentSelectedIds)
                    {
                        // Find the product in the current DataSource display items
                        var displayProduct = DataSource.DisplayItems?.Cast<Product>()
                            .FirstOrDefault(p => p.Id == productId);

                        if (displayProduct != null)
                        {
                            listView.SelectedItems.Add(displayProduct);
                            SelectedItems.Add(displayProduct);
                        }
                    }

                    // Update the selected count display using persistent count
                    UpdateSelectedItemsCountDisplay();

                    // Reconnect selection event
                    if (homePage != null)
                    {
                        listView.SelectionChanged += homePage.ListView_SelectionChanged;
                    }
                }
            }
        }

        public int? GetCatId(string categoryName)
        {
            var match = Categories.FirstOrDefault(c =>
                c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            return match?.Id;
        }



       

        private bool FilterByActiveCategory(object obj)
        {
            if (obj is not Product product)
                return false;

            var activeCategory = Categories.FirstOrDefault(c => c.State == "Active Categorie");
            if (activeCategory == null)
                return true;

            // Compare category names case-insensitively
            return string.Equals(product.CategoryName, activeCategory.Name, StringComparison.OrdinalIgnoreCase);
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

        public ICommand RemoveFromCartCommand => new Command<Product>((product) =>
        {
            RemoveFromCurrentOrder(product);
        });

        public ICommand IncrementQuantityCommand => new Command<Product>((product) =>
        {
            IncrementQuantity(product);
        });

        public ICommand DecrementQuantityCommand => new Command<Product>((product) =>
        {
            DecrementQuantity(product);
        });

        public ICommand ClearAllSelectionsCommand => new Command(() =>
        {
            ClearAllSelections();
        });

        public ICommand SwitchCategoryState => new Command<string>((name) =>
        {
            if (name == "All")
            {
                foreach (var category in Categories)
                    category.State = "Inactive Categorie";
                Categories = new ObservableCollection<Category>(Categories);
            }
            else
            {
                var selected = Categories.FirstOrDefault(c => c.Name == name);
                foreach (var category in Categories)
                    category.State = "Inactive Categorie";

                selected.State = "Active Categorie";
                Categories = new ObservableCollection<Category>(Categories);
                DataSource.Filter = FilterByActiveCategory;
            }

            // Force refresh
            DataSource.RefreshFilter();
            OnPropertyChanged(nameof(DataSource));

            // Restore selections after filtering
            RestoreListViewSelections();

            // Update selected count after category filtering
            UpdateSelectedItemsCountDisplay();
        });







        #endregion

        #region Tasks

        public void SortProduct()
        {
            DataSource = new DataSource() { Source = Products };
            DataSource.Refresh();

        }


        #endregion

        public void LoadDB()
        {
            // Load products from the database or any other source
            SortProduct();
        }

    }
}