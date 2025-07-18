﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NeuroPOS.MVVM.Model;
using PropertyChanged;
using Syncfusion.Maui.DataSource;
using ListSortDirection = Syncfusion.Maui.DataSource.ListSortDirection;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class InventoryVM : INotifyPropertyChanged
    {
        public InventoryVM()
        {
            // Initialize DataSource first
            DataSource = new DataSource()
            {
                Source = new ObservableCollection<Product>()
            };
            LoadData();
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
        public ObservableCollection<Transaction> BuyingTransaction { get; set; } = new ObservableCollection<Transaction>();
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<object> SelectedItems { get; set; } = new ObservableCollection<object>();
        public IList<object> SelectedProducts { get; set; } = new List<object>();

        private ObservableCollection<Category> _categories = new ObservableCollection<Category>
        {
            new Category { Id = 1, Name = "Produce", State = "Inactive Categorie" },
            new Category { Id = 2, Name = "Bakery", State = "Inactive Categorie" },
            new Category { Id = 3, Name = "Dairy", State = "Inactive Categorie" },
            new Category { Id = 4, Name = "Meat", State = "Inactive Categorie" },
            new Category { Id = 5, Name = "Seafood", State = "Inactive Categorie" },
            new Category { Id = 6, Name = "Beverages", State = "Inactive Categorie" },
            new Category { Id = 7, Name = "Snacks", State = "Inactive Categorie" },
            new Category { Id = 8, Name = "Electronics", State = "Inactive Categorie" }
        };

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged();
                }
            }
        }

        private DataSource _dataSource;
        public DataSource DataSource
        {
            get => _dataSource;
            set
            {
                if (_dataSource != value)
                {
                    _dataSource = value;
                    OnPropertyChanged();
                }
            }
        }

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
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SortLabel));
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

        // Selected product for editing
        private Product _selectedProduct;
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (_selectedProduct != value)
                {
                    _selectedProduct = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasSelectedProduct));
                    OnPropertyChanged(nameof(IsEditMode));
                }
            }
        }

        public bool HasSelectedProduct => SelectedProduct != null;
        public bool IsEditMode => HasSelectedProduct;

        // Editing properties
        public string EditName { get; set; }
        public double EditPrice { get; set; }
        public double EditCost { get; set; }
        public int EditStock { get; set; }
        public string EditCategory { get; set; }
        public string EditImageUrl { get; set; }

        // New product properties
        public int NewProductId => Products.Count > 0 ? Products.Max(p => p.Id) + 1 : 1;
        public string NewProductName { get; set; } = string.Empty;
        public string NewProductPrice { get; set; } = string.Empty;
        public string NewProductCost { get; set; } = string.Empty;
        public Category NewProductCategory { get; set; }
        public string NewProductImageUrl { get; set; } = "emptyproduct.png";

        // New category properties
        public int NewCategoryId => Categories.Count > 0 ? Categories.Max(c => c.Id) + 1 : 1;
        public string NewCategoryName { get; set; } = string.Empty;
        public string NewCategoryDescription { get; set; } = string.Empty;

        // Category selection for dropdown
        private Category _selectedEditCategory;
        public Category SelectedEditCategory
        {
            get => _selectedEditCategory;
            set
            {
                if (_selectedEditCategory != value)
                {
                    _selectedEditCategory = value;
                    if (value != null)
                    {
                        EditCategory = value.Name;
                    }
                    OnPropertyChanged();
                }
            }
        }

        // Reference to the page for callbacks
        public object PageReference { get; set; }

        // Refresh state tracking
        private bool _isRefreshing = false;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    OnPropertyChanged();
                }
            }
        }

        // Selection tracking
        private ObservableCollection<int> _persistentSelectedIds = new ObservableCollection<int>();
        public bool HasSelectedItems => _persistentSelectedIds?.Count > 0;
        public ObservableCollection<int> PersistentSelectedIds => _persistentSelectedIds;

        // Track select all state
        private bool _isSelectAllChecked = false;
        public bool IsSelectAllChecked
        {
            get => _isSelectAllChecked;
            set
            {
                if (_isSelectAllChecked != value)
                {
                    _isSelectAllChecked = value;
                    OnPropertyChanged();
                }
            }
        }

        // Category filtering for dropdown
        public ObservableCollection<string> CategoryFilterOptions { get; set; } = new ObservableCollection<string>();

        private string _selectedCategoryFilter = "All Categories";
        public string SelectedCategoryFilter
        {
            get => _selectedCategoryFilter;
            set
            {
                if (_selectedCategoryFilter != value)
                {
                    _selectedCategoryFilter = value;
                    OnPropertyChanged();
                    ApplyCategoryFilter();
                }
            }
        }

        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Methods
        public void LoadData()
        {
            Products = new ObservableCollection<Product>
            {
                new Product { Id = 1, Name = "Organic Apples", Price = 1.99, Cost = 1.25, Stock = 150, DateAdded = DateTime.Now.AddDays(-10), CategoryName = "Produce", ImageUrl = "emptyproduct.png" },
                new Product { Id = 2, Name = "Whole Wheat Bread", Price = 3.49, Cost = 2.10, Stock = 75, DateAdded = DateTime.Now.AddDays(-8), CategoryName = "Bakery", ImageUrl = "emptyproduct.png" },
                new Product { Id = 3, Name = "Cheddar Cheese", Price = 4.99, Cost = 3.20, Stock = 100, DateAdded = DateTime.Now.AddDays(-6), CategoryName = "Dairy", ImageUrl = "emptyproduct.png" },
                new Product { Id = 4, Name = "Ground Beef", Price = 5.99, Cost = 4.50, Stock = 50, DateAdded = DateTime.Now.AddDays(-4), CategoryName = "Meat", ImageUrl = "emptyproduct.png" },
                new Product { Id = 5, Name = "Salmon Fillet", Price = 9.99, Cost = 7.50, Stock = 30, DateAdded = DateTime.Now.AddDays(-2), CategoryName = "Seafood", ImageUrl = "emptyproduct.png" },
                new Product { Id = 6, Name = "Orange Juice", Price = 2.99, Cost = 1.80, Stock = 80, DateAdded = DateTime.Now.AddDays(-5), CategoryName = "Beverages", ImageUrl = "emptyproduct.png" },
                new Product { Id = 7, Name = "Potato Chips", Price = 2.49, Cost = 1.50, Stock = 120, DateAdded = DateTime.Now.AddDays(-3), CategoryName = "Snacks", ImageUrl = "emptyproduct.png" },
                new Product { Id = 8, Name = "Wireless Headphones", Price = 79.99, Cost = 45.00, Stock = 25, DateAdded = DateTime.Now.AddDays(-1), CategoryName = "Electronics", ImageUrl = "emptyproduct.png" },
                new Product { Id = 9, Name = "Fresh Bananas", Price = 0.99, Cost = 0.65, Stock = 200, DateAdded = DateTime.Now.AddDays(-7), CategoryName = "Produce", ImageUrl = "emptyproduct.png" },
                new Product { Id = 10, Name = "Croissants", Price = 1.99, Cost = 1.20, Stock = 40, DateAdded = DateTime.Now.AddDays(-2), CategoryName = "Bakery", ImageUrl = "emptyproduct.png" },
                new Product { Id = 11, Name = "Greek Yogurt", Price = 1.49, Cost = 0.90, Stock = 85, DateAdded = DateTime.Now.AddDays(-4), CategoryName = "Dairy", ImageUrl = "emptyproduct.png" },
                new Product { Id = 12, Name = "Chicken Breast", Price = 6.99, Cost = 5.25, Stock = 60, DateAdded = DateTime.Now.AddDays(-3), CategoryName = "Meat", ImageUrl = "emptyproduct.png" },
                new Product { Id = 13, Name = "Shrimp", Price = 12.99, Cost = 9.50, Stock = 35, DateAdded = DateTime.Now.AddDays(-1), CategoryName = "Seafood", ImageUrl = "emptyproduct.png" },
                new Product { Id = 14, Name = "Coffee Beans", Price = 8.99, Cost = 5.50, Stock = 45, DateAdded = DateTime.Now.AddDays(-6), CategoryName = "Beverages", ImageUrl = "emptyproduct.png" },
                new Product { Id = 15, Name = "Energy Bars", Price = 3.99, Cost = 2.40, Stock = 95, DateAdded = DateTime.Now.AddDays(-2), CategoryName = "Snacks", ImageUrl = "emptyproduct.png" },
                new Product { Id = 16, Name = "Smartphone", Price = 599.99, Cost = 350.00, Stock = 15, DateAdded = DateTime.Now.AddDays(-5), CategoryName = "Electronics", ImageUrl = "emptyproduct.png" },
                new Product { Id = 17, Name = "Avocados", Price = 1.29, Cost = 0.85, Stock = 90, DateAdded = DateTime.Now.AddDays(-3), CategoryName = "Produce", ImageUrl = "emptyproduct.png" },
                new Product { Id = 18, Name = "Bagels", Price = 2.79, Cost = 1.75, Stock = 55, DateAdded = DateTime.Now.AddDays(-1), CategoryName = "Bakery", ImageUrl = "emptyproduct.png" },
                new Product { Id = 19, Name = "Milk", Price = 3.29, Cost = 2.15, Stock = 120, DateAdded = DateTime.Now.AddDays(-4), CategoryName = "Dairy", ImageUrl = "emptyproduct.png" },
                new Product { Id = 20, Name = "Pork Chops", Price = 7.49, Cost = 5.75, Stock = 40, DateAdded = DateTime.Now.AddDays(-2), CategoryName = "Meat", ImageUrl = "emptyproduct.png" }
            };

            // CRITICAL FIX: Update DataSource to point to the new Products collection
            DataSource.Source = Products;

            PopulateCategoryFilterOptions();
            SortProduct();
        }

        public void PopulateCategoryFilterOptions()
        {
            // Get categories from products (that have products assigned)
            var categoriesFromProducts = Products
                .Select(p => p.CategoryName)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct();

            // Get all available categories from Categories collection
            var availableCategories = Categories
                .Select(c => c.Name)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct();

            // Combine and get unique categories
            var allCategories = categoriesFromProducts
                .Union(availableCategories)
                .OrderBy(c => c)
                .ToList();

            CategoryFilterOptions.Clear();
            CategoryFilterOptions.Add("All Categories");

            foreach (var category in allCategories)
            {
                CategoryFilterOptions.Add(category);
            }

            // Trigger property change notification for UI updates
            OnPropertyChanged(nameof(CategoryFilterOptions));
        }

        public void ApplyCategoryFilter()
        {
            // Ensure DataSource is properly connected
            DataSource.Source = Products;

            if (SelectedCategoryFilter == "All Categories")
            {
                DataSource.Filter = null;
            }
            else
            {
                DataSource.Filter = FilterBySelectedCategory;
            }

            DataSource.RefreshFilter();
            RestoreListViewSelections();
            UpdateSelectedItemsCountDisplay();
        }

        private bool FilterBySelectedCategory(object obj)
        {
            if (obj is not Product product)
                return false;

            if (SelectedCategoryFilter == "All Categories")
                return true;

            return string.Equals(product.CategoryName, SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase);
        }

        public void SortProduct()
        {
            // Ensure DataSource is properly connected to Products
            DataSource.Source = Products;
            DataSource.Refresh();
        }

        public void ApplySorting()
        {
            // Ensure DataSource is properly connected
            DataSource.Source = Products;
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

            RestoreListViewSelections();
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

        public void UpdateSelectedItemsCountDisplay()
        {
            // Selection count display is now handled through binding in the new design
            // This method is kept for compatibility but no longer updates UI elements
        }

        public void RestoreListViewSelections()
        {
            if (PageReference is ContentPage page && _persistentSelectedIds?.Count > 0)
            {
                var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                if (listView != null)
                {
                    listView.SelectedItems?.Clear();
                    SelectedItems.Clear();

                    foreach (var productId in _persistentSelectedIds)
                    {
                        var displayProduct = DataSource.DisplayItems?.Cast<Product>()
                            .FirstOrDefault(p => p.Id == productId);

                        if (displayProduct != null)
                        {
                            listView.SelectedItems.Add(displayProduct);
                            SelectedItems.Add(displayProduct);
                        }
                    }

                    UpdateSelectedItemsCountDisplay();
                }
            }
        }

        public void ClearAllSelections()
        {
            SelectedItems.Clear();
            _persistentSelectedIds.Clear();
            SelectedProduct = null;
            IsSelectAllChecked = false;

            OnPropertyChanged(nameof(HasSelectedItems));
            UpdateSelectedItemsCountDisplay();

            if (PageReference is ContentPage page)
            {
                var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                listView?.SelectedItems?.Clear();
            }
        }

        public void SelectProductForEdit(Product product)
        {
            // Always start a fresh edit with no outstanding selections
            ClearAllSelections();
            SelectedProduct = product;
            if (product != null)
            {
                EditName = product.Name;
                EditPrice = product.Price;
                EditCost = product.Cost;
                EditStock = product.Stock;
                EditCategory = product.CategoryName;
                EditImageUrl = product.ImageUrl;

                // Set the selected category for the dropdown
                SelectedEditCategory = Categories.FirstOrDefault(c => c.Name == product.CategoryName);
            }
        }

        public void SaveProductChanges()
        {
            if (SelectedProduct != null)
            {
                SelectedProduct.Name = EditName;
                SelectedProduct.Price = EditPrice;
                SelectedProduct.Cost = EditCost;
                SelectedProduct.Stock = EditStock;
                SelectedProduct.CategoryName = SelectedEditCategory?.Name ?? EditCategory;
                SelectedProduct.ImageUrl = EditImageUrl;

                // Clear selection after saving
                SelectedProduct = null;
                SelectedEditCategory = null;

                // Clear search filter to show all products
                ClearSearchFilter();

                // Refresh DataSource to show changes
                DataSource.Source = Products;
                DataSource.Refresh();
                PopulateCategoryFilterOptions();
            }
        }

        public void CancelEdit()
        {
            SelectedProduct = null;
            SelectedEditCategory = null;
            EditName = string.Empty;
            EditPrice = 0;
            EditCost = 0;
            EditStock = 0;
            EditCategory = string.Empty;
            EditImageUrl = string.Empty;
        }

        public void RefreshUI()
        {
            UpdateSelectedItemsCountDisplay();
            OnPropertyChanged(nameof(HasSelectedItems));
            RestoreListViewSelections();
        }

        public void ClearSearchFilter()
        {
            // Clear search filter through the page reference
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ClearSearchFilter();
            }
        }

        public void UpdateImageUrl(string newImageUrl)
        {
            EditImageUrl = newImageUrl;
            OnPropertyChanged(nameof(EditImageUrl));
        }

        public void ClearNewProductForm()
        {
            NewProductName = string.Empty;
            NewProductPrice = string.Empty;
            NewProductCost = string.Empty;
            NewProductCategory = null;
            NewProductImageUrl = "emptyproduct.png";
            OnPropertyChanged(nameof(NewProductName));
            OnPropertyChanged(nameof(NewProductPrice));
            OnPropertyChanged(nameof(NewProductCost));
            OnPropertyChanged(nameof(NewProductCategory));
            OnPropertyChanged(nameof(NewProductImageUrl));
            OnPropertyChanged(nameof(NewProductId));
        }

        public bool ValidateNewProduct()
        {
            // Check if a product with the same name already exists
            return !Products.Any(p => p.Name.Equals(NewProductName, StringComparison.OrdinalIgnoreCase));
        }

        public void AddNewProduct()
        {
            if (!ValidateNewProduct())
                return; // Validation failed - duplicate product

            // Parse numeric values
            if (!double.TryParse(NewProductPrice, out double price))
                price = 0;

            if (!double.TryParse(NewProductCost, out double cost))
                cost = 0;

            // Create new product
            var newProduct = new Product
            {
                Id = NewProductId,
                Name = NewProductName,
                Price = price,
                Cost = cost,
                Stock = 0, // Initial stock is always 0
                CategoryName = NewProductCategory?.Name ?? "Uncategorized",
                ImageUrl = NewProductImageUrl,
                DateAdded = DateTime.Now
            };

            // Add to collection
            Products.Add(newProduct);

            // Refresh UI
            DataSource.Source = Products;
            DataSource.Refresh();
            PopulateCategoryFilterOptions();

            // Clear form
            ClearNewProductForm();
        }

        public void ClearNewCategoryForm()
        {
            NewCategoryName = string.Empty;
            NewCategoryDescription = string.Empty;
            OnPropertyChanged(nameof(NewCategoryName));
            OnPropertyChanged(nameof(NewCategoryDescription));
            OnPropertyChanged(nameof(NewCategoryId));
        }

        public bool ValidateNewCategory()
        {
            // Check if a category with the same name already exists (case-insensitive)
            return !Categories.Any(c => c.Name.Equals(NewCategoryName, StringComparison.OrdinalIgnoreCase));
        }

        public void AddNewCategory()
        {
            if (!ValidateNewCategory())
                return; // Validation failed - duplicate category

            // Create new category
            var newCategory = new Category
            {
                Id = NewCategoryId,
                Name = NewCategoryName.Trim(),
                Description = string.IsNullOrWhiteSpace(NewCategoryDescription) ? string.Empty : NewCategoryDescription.Trim(),
                ProductCount = 0, // No products initially
                State = "Inactive Categorie" // Default state
            };

            // Add to collection
            Categories.Add(newCategory);

            // Trigger explicit property change notifications for UI updates
            OnPropertyChanged(nameof(Categories));
            OnPropertyChanged(nameof(NewCategoryId));

            // Refresh category filter options
            PopulateCategoryFilterOptions();

            // Clear form
            ClearNewCategoryForm();
        }



        public void UpdateSelectAllCheckboxState()
        {
            // Check if all visible products are selected
            var visibleProducts = DataSource.DisplayItems?.Cast<Product>().ToList();
            if (visibleProducts != null && visibleProducts.Any())
            {
                var allSelected = visibleProducts.All(p => _persistentSelectedIds.Contains(p.Id));
                IsSelectAllChecked = allSelected;
            }
            else
            {
                IsSelectAllChecked = false;
            }
        }

        // Methods for delete operations (to be called after confirmation)
        public void DeleteProductById(int productId)
        {
            // Locate the item once
            var productToDelete = Products.FirstOrDefault(p => p.Id == productId);
            if (productToDelete == null) return;

            //  Remove it from the collection that backs the ListView
            Products.Remove(productToDelete);

            //  Make absolutely sure no stale selections remain  
            //     (this was the source of the “everything disappears” symptom)
            ClearAllSelections();

            //  Reset edit state if the item being edited was deleted
            if (SelectedProduct?.Id == productId)
                SelectedProduct = null;

            //  Refresh data-bound helpers
            DataSource.Source = Products;
            DataSource.Refresh();
            PopulateCategoryFilterOptions();
        }


        public void DeleteSelectedProductsByIds()
        {
            if (_persistentSelectedIds?.Count > 0)
            {
                var idsToDelete = _persistentSelectedIds.ToList();

                foreach (var productId in idsToDelete)
                {
                    var productToDelete = Products.FirstOrDefault(p => p.Id == productId);
                    if (productToDelete != null)
                    {
                        Products.Remove(productToDelete);

                        if (SelectedProduct?.Id == productToDelete.Id)
                        {
                            SelectedProduct = null;
                        }
                    }
                }

                ClearAllSelections();

                // Clear search filter to show all products
                ClearSearchFilter();

                // Refresh DataSource to show changes
                DataSource.Source = Products;
                DataSource.Refresh();
                PopulateCategoryFilterOptions();
            }
        }

        public bool HasUnsavedChanges()
        {
            if (SelectedProduct == null) return false;

            return EditName != SelectedProduct.Name ||
                   Math.Abs(EditPrice - SelectedProduct.Price) > 0.001 ||
                   Math.Abs(EditCost - SelectedProduct.Cost) > 0.001 ||
                   EditStock != SelectedProduct.Stock ||
                   (SelectedEditCategory?.Name ?? EditCategory) != SelectedProduct.CategoryName ||
                   EditImageUrl != SelectedProduct.ImageUrl;
        }
        public void RevalidateActiveFilters()
        {
            // 1️⃣  If the current category yields no rows, reset it
            if (SelectedCategoryFilter != "All Categories" &&
                !Products.Any(p => p.CategoryName
                       .Equals(SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase)))
            {
                SelectedCategoryFilter = "All Categories";
            }

            // 2️⃣  If any search tokens point to deleted rows, clear search
            if (SelectedProducts?.Count > 0)
            {
                var stillExist = SelectedProducts
                    .OfType<Product>()
                    .Where(p => Products.Any(x => x.Id == p.Id))
                    .ToList();

                if (stillExist.Count != SelectedProducts.Count)
                    SelectedProducts.Clear();   // empty search list
            }

            // 3️⃣  Re-apply filters and refresh UI
            ApplyCategoryFilter();         // (also refreshes DataSource)
            DataSource.RefreshFilter();    // make sure search predicate reruns
        }

        public async Task RefreshDBAsync()
        {
            // Start refresh animation
            IsRefreshing = true;

            try
            {
                // Small delay to show the refresh animation
                await Task.Delay(500);

                // 1️⃣  Clear all selections and edit state
                ClearAllSelections();
                CancelEdit();
                ClearSearchFilter();

                // 2️⃣  Reload data from "database" (simulated)
                LoadData();

                // 3️⃣  Refresh UI components
                RefreshUI();
                RevalidateActiveFilters();

                // 4️⃣  Notify property changes for UI updates
                OnPropertyChanged(nameof(Products));
                OnPropertyChanged(nameof(Categories));
                OnPropertyChanged(nameof(CategoryFilterOptions));
                OnPropertyChanged(nameof(NewProductId));
                OnPropertyChanged(nameof(NewCategoryId));
            }
            finally
            {
                // Stop refresh animation
                IsRefreshing = false;
            }
        }

        public void RefreshDB()
        {
            // Synchronous version for button clicks
            _ = RefreshDBAsync();
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

        public ICommand ClearAllSelectionsCommand => new Command(() =>
        {
            ClearAllSelections();
        });


        public ICommand EditProductCommand => new Command<Product>((product) =>
        {
            SelectProductForEdit(product);
        });

        public ICommand SaveProductCommand => new Command(() =>
        {
            // Show save confirmation popup
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowSaveEditConfirmation();
            }
        });

        public ICommand CancelEditCommand => new Command(() =>
        {
            // The actual popup logic will be handled in the View
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowCancelEditConfirmation();
            }
        });

        public ICommand DeleteProductCommand => new Command<Product>((product) =>
        {
            // The actual popup logic will be handled in the View
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowDeleteProductConfirmation(product);
            }
        });

        public ICommand DeleteSelectedProductsCommand => new Command(() =>
        {
            // The actual popup logic will be handled in the View
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowDeleteSelectedConfirmation();
            }
        });

        public ICommand AddProductCommand => new Command(() =>
        {
            // Show add product popup
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowAddProductPopup();
            }
        });

        public ICommand AddCategoryCommand => new Command(() =>
        {
            // Show add category popup
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowAddCategoryPopup();
            }
        });

        public ICommand RefreshDBCommand => new Command(async () =>
        {
            await RefreshDBAsync();
        });

        public ICommand AddBuyingTransactionCommand => new Command(() =>
        {
            // Show buying transaction popup
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowBuyingTransactionPopup();
            }
        });


        #endregion
    }
}