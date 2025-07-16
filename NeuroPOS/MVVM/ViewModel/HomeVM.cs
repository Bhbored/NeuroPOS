using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.Popups;
using PropertyChanged;
using Syncfusion.Maui.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Contact = NeuroPOS.MVVM.Model.Contact;
using ListSortDirection = Syncfusion.Maui.DataSource.ListSortDirection;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class HomeVM : INotifyPropertyChanged
    {

        public HomeVM()
        {
            Contacts = [
                new Contact { Name = "John Doe" },
                new Contact { Name = "Jane Smith" },
                new Contact { Name = "Alice Johnson" },
                new Contact { Name = "Bob Brown" },
                new Contact { Name = "Charlie Davis" },
                ];

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

        public ObservableCollection<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();
        public ObservableCollection<Transaction> Transactions { get; set; } = new ObservableCollection<Transaction>();

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

        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();



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

        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();




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

        private double _taxRate = 2.0; // Default 2%
        public double TaxRate
        {
            get => _taxRate;
            set
            {
                if (Math.Abs(_taxRate - value) > 0.01) // Only change if significantly different
                {
                    _taxRate = value;
                    OnPropertyChanged();
                    // Only notify calculated properties if we have order items
                    if (CurrentOrderItems?.Count > 0)
                    {
                        NotifyCalculatedPropertiesChanged();
                    }
                }
            }
        }

        // Update the Tax property to use the TaxRate
        public double Tax
        {
            get
            {
                var result = Subtotal * (TaxRate / 100.0);
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

        public void AddToPersistentSelection(int productId)
        {
            if (!_persistentSelectedIds.Contains(productId))
            {
                _persistentSelectedIds.Add(productId);
                OnPropertyChanged(nameof(HasSelectedItems));
                UpdateSelectedItemsCountDisplay();
            }
        }

        public void RemoveFromPersistentSelection(int productId)
        {
            if (_persistentSelectedIds.Contains(productId))
            {
                _persistentSelectedIds.Remove(productId);
                OnPropertyChanged(nameof(HasSelectedItems));
                UpdateSelectedItemsCountDisplay();
            }
        }

        public bool IsInPersistentSelection(int productId)
        {
            return _persistentSelectedIds.Contains(productId);
        }

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
        public void RefreshUI()
        {
            UpdateSelectedItemsCountDisplay();
            OnPropertyChanged(nameof(HasSelectedItems));
            RestoreListViewSelections();
        }

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

        public ICommand EditTaxCommand => new Command(async () =>
        {
            await EditTaxRate();
        });


        #endregion

        #region Tasks

        public void SortProduct()
        {
            DataSource = new DataSource() { Source = Products };
            DataSource.Refresh();

        }


        #endregion

        public async Task LoadDB()
        {

            if (App.ProductRepo == null || App.CategoryRepo == null)
            {
                return;
            }

            Products.Clear();
            Categories.Clear();
            var DBProducts = App.ProductRepo.GetItems();
            var DBCategories = App.CategoryRepo.GetItems();
            foreach (var item in DBProducts)
            {
                Products.Add(item);
            }
            foreach (var item in DBCategories)
            {
                Categories.Add(item);
            }
            SortProduct();
        }

        // Method to update tax rate without triggering property setter logic
        private void UpdateTaxRateSilently(double newTaxRate)
        {
            _taxRate = newTaxRate;
        }

        public async Task EditTaxRate()
        {
            try
            {
                var originalTaxRate = TaxRate;
                var popup = new EditTaxPopup(TaxRate);

                await Application.Current.MainPage.ShowPopupAsync(popup);

                // Get the result directly from the popup
                var newTaxRate = popup.Result;

                // Only update if the value actually changed
                if (Math.Abs(newTaxRate - originalTaxRate) > 0.01)
                {
                    // Update tax rate silently without triggering property setter
                    UpdateTaxRateSilently(newTaxRate);

                    // Manually update the UI for tax-related properties only
                    OnPropertyChanged(nameof(TaxRate));
                    OnPropertyChanged(nameof(Tax));
                    OnPropertyChanged(nameof(Total));


                    var snackbar = Snackbar.Make($"Tax rate updated to {TaxRate:F1}%",
          duration: TimeSpan.FromSeconds(3));
                    await snackbar.Show();
                }
            }
            catch (Exception ex)
            {

                var snackbar = Snackbar.Make("Failed to update tax rate",
           duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }

       

    }
}