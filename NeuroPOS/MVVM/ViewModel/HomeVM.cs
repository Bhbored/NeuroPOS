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
    new Product() { Id = 1, Name = "Laptop", CategoryName = "Fruits", Price = 999.99, Stock = 10, DateAdded = DateTime.Now.AddDays(-5), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 2, Name = "Mouse", CategoryName = "Fruits", Price = 25.50, Stock = 50, DateAdded = DateTime.Now.AddDays(-2), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 3, Name = "Keyboard", CategoryName = "Vegetables", Price = 45.99, Stock = 5, DateAdded = DateTime.Now, ImageUrl = "emptyproduct.png" },
    new Product() { Id = 4, Name = "Monitor", CategoryName = "Vegetables", Price = 199.99, Stock = 15, DateAdded = DateTime.Now.AddDays(-1), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 5, Name = "Headphones", CategoryName = "Drinks", Price = 79.99, Stock = 3, DateAdded = DateTime.Now.AddDays(-3), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 6, Name = "Wireless Earbuds", CategoryName = "Drinks", Price = 59.99, Stock = 50, DateAdded = DateTime.Now.AddDays(-5), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 7, Name = "Smart Watch", CategoryName = "Snacks", Price = 199.99, Stock = 15, DateAdded = DateTime.Now.AddDays(-10), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 8, Name = "Bluetooth Speaker", CategoryName = "Snacks", Price = 89.99, Stock = 30, DateAdded = DateTime.Now.AddDays(-2), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 9, Name = "Tablet", CategoryName = "Fruits", Price = 499.99, Stock = 20, DateAdded = DateTime.Now.AddDays(-7), ImageUrl = "emptyproduct.png" },
    new Product() { Id = 10, Name = "Smartphone", CategoryName = "Fruits", Price = 799.99, Stock = 25, DateAdded = DateTime.Now.AddDays(-4), ImageUrl = "emptyproduct.png" },
      new Product() { Id = 1, Name = "Apple", CategoryName = "Fruits", Price = 0.99, Stock = 50, DateAdded = DateTime.Now.AddDays(-2) },
    new Product() { Id = 2, Name = "Banana", CategoryName = "Fruits", Price = 0.49, Stock = 100, DateAdded = DateTime.Now.AddDays(-1) },
    new Product() { Id = 3, Name = "Orange", CategoryName = "Fruits", Price = 0.79, Stock = 75, DateAdded = DateTime.Now.AddDays(-3) },
    
    // Vegetables (CategoryId = 2)
    new Product() { Id = 4, Name = "Carrot", CategoryName = "Vegetables", Price = 0.89, Stock = 60, DateAdded = DateTime.Now.AddDays(-4) },
    new Product() { Id = 5, Name = "Broccoli", CategoryName = "Vegetables", Price = 1.29, Stock = 40, DateAdded = DateTime.Now.AddDays(-5) },
    new Product() { Id = 6, Name = "Spinach", CategoryName = "Vegetables", Price = 1.99, Stock = 30, DateAdded = DateTime.Now.AddDays(-1) },
    
    // Drinks (CategoryId = 3)
    new Product() { Id = 7, Name = "Water", CategoryName = "Drinks", Price = 1.49, Stock = 200, DateAdded = DateTime.Now.AddDays(-7) },
    new Product() { Id = 8, Name = "Orange Juice", CategoryName = "Drinks", Price = 2.99, Stock = 50, DateAdded = DateTime.Now.AddDays(-2) },
    new Product() { Id = 9, Name = "Soda", CategoryName = "Drinks", Price = 1.79, Stock = 80, DateAdded = DateTime.Now.AddDays(-3) },
    
    // Snacks (CategoryId = 4)
    new Product() { Id = 10, Name = "Chips", CategoryName = "Snacks", Price = 2.49, Stock = 45, DateAdded = DateTime.Now.AddDays(-4) },
    new Product() { Id = 11, Name = "Cookies", CategoryName = "Snacks", Price = 3.99, Stock = 35, DateAdded = DateTime.Now.AddDays(-6) },
    new Product() { Id = 12, Name = "Nuts", CategoryName = "Snacks", Price = 4.49, Stock = 25, DateAdded = DateTime.Now.AddDays(-1) },

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


                                // Update the selectedValue display (ListView selection counter only)
                                var selectedValueLabel = page.FindByName("selectedValue") as Label;
                                if (selectedValueLabel != null)
                                {
                                    selectedValueLabel.Text = listView.SelectedItems.Count.ToString();

                                }

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
                    // Increase quantity if already exists
                    existingItem.Stock += 1;

                    // FORCE notification for calculated properties after incrementing existing item
                    NotifyCalculatedPropertiesChanged();
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

                            // Update the selectedValue display (ListView selection counter only)
                            var selectedValueLabel = page.FindByName("selectedValue") as Label;
                            if (selectedValueLabel != null)
                            {
                                selectedValueLabel.Text = listView.SelectedItems.Count.ToString();

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
                    existingItem.Stock += 1;
                    // FORCE manual notification to test if bindings work
                    NotifyCalculatedPropertiesChanged();
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

        public void ClearAllSelections()
        {
            // Clear the ListView selections
            SelectedItems.Clear();

            // Clear the cart
            CurrentOrderItems.Clear();

            // Force notification of calculated properties
            NotifyCalculatedPropertiesChanged();

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

                // Update the selected count display (only the ListView selection counter)
                var selectedValueLabel = page.FindByName("selectedValue") as Label;
                if (selectedValueLabel != null)
                {
                    selectedValueLabel.Text = "0";

                }

                // Note: We don't clear searchFilterValue here as it's for autocomplete filtering, not ListView selection
            }

            // Notify property changes
            OnPropertyChanged(nameof(HasSelectedItems));


        }

        // Property to check if any items are selected (for the clear button visibility)
        public bool HasSelectedItems => SelectedItems?.Count > 0;

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
        }

        public int? GetCatId(string categoryName)
        {
            var match = Categories.FirstOrDefault(c =>
                c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            return match?.Id;
        }



        public void LoadDB()
        {
            // Implement DB loading here if needed
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