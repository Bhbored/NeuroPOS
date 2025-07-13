using System;
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
        public int EditStock { get; set; }
        public string EditCategory { get; set; }
        public string EditImageUrl { get; set; }

        // Reference to the page for callbacks
        public object PageReference { get; set; }

        // Selection tracking
        private ObservableCollection<int> _persistentSelectedIds = new ObservableCollection<int>();
        public bool HasSelectedItems => _persistentSelectedIds?.Count > 0;

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
                new Product { Id = 1, Name = "Organic Apples", Price = 1.99, Stock = 150, DateAdded = DateTime.Now.AddDays(-10), CategoryName = "Produce", ImageUrl = "emptyproduct.png" },
                new Product { Id = 2, Name = "Whole Wheat Bread", Price = 3.49, Stock = 75, DateAdded = DateTime.Now.AddDays(-8), CategoryName = "Bakery", ImageUrl = "emptyproduct.png" },
                new Product { Id = 3, Name = "Cheddar Cheese", Price = 4.99, Stock = 100, DateAdded = DateTime.Now.AddDays(-6), CategoryName = "Dairy", ImageUrl = "emptyproduct.png" },
                new Product { Id = 4, Name = "Ground Beef", Price = 5.99, Stock = 50, DateAdded = DateTime.Now.AddDays(-4), CategoryName = "Meat", ImageUrl = "emptyproduct.png" },
                new Product { Id = 5, Name = "Salmon Fillet", Price = 9.99, Stock = 30, DateAdded = DateTime.Now.AddDays(-2), CategoryName = "Seafood", ImageUrl = "emptyproduct.png" },
                new Product { Id = 6, Name = "Orange Juice", Price = 2.99, Stock = 80, DateAdded = DateTime.Now.AddDays(-5), CategoryName = "Beverages", ImageUrl = "emptyproduct.png" },
                new Product { Id = 7, Name = "Potato Chips", Price = 2.49, Stock = 120, DateAdded = DateTime.Now.AddDays(-3), CategoryName = "Snacks", ImageUrl = "emptyproduct.png" },
                new Product { Id = 8, Name = "Wireless Headphones", Price = 79.99, Stock = 25, DateAdded = DateTime.Now.AddDays(-1), CategoryName = "Electronics", ImageUrl = "emptyproduct.png" },
                new Product { Id = 9, Name = "Fresh Bananas", Price = 0.99, Stock = 200, DateAdded = DateTime.Now.AddDays(-7), CategoryName = "Produce", ImageUrl = "emptyproduct.png" },
                new Product { Id = 10, Name = "Croissants", Price = 1.99, Stock = 40, DateAdded = DateTime.Now.AddDays(-2), CategoryName = "Bakery", ImageUrl = "emptyproduct.png" },
                new Product { Id = 11, Name = "Greek Yogurt", Price = 1.49, Stock = 85, DateAdded = DateTime.Now.AddDays(-4), CategoryName = "Dairy", ImageUrl = "emptyproduct.png" },
                new Product { Id = 12, Name = "Chicken Breast", Price = 6.99, Stock = 60, DateAdded = DateTime.Now.AddDays(-3), CategoryName = "Meat", ImageUrl = "emptyproduct.png" },
                new Product { Id = 13, Name = "Shrimp", Price = 12.99, Stock = 35, DateAdded = DateTime.Now.AddDays(-1), CategoryName = "Seafood", ImageUrl = "emptyproduct.png" },
                new Product { Id = 14, Name = "Coffee Beans", Price = 8.99, Stock = 45, DateAdded = DateTime.Now.AddDays(-6), CategoryName = "Beverages", ImageUrl = "emptyproduct.png" },
                new Product { Id = 15, Name = "Energy Bars", Price = 3.99, Stock = 95, DateAdded = DateTime.Now.AddDays(-2), CategoryName = "Snacks", ImageUrl = "emptyproduct.png" },
                new Product { Id = 16, Name = "Smartphone", Price = 599.99, Stock = 15, DateAdded = DateTime.Now.AddDays(-5), CategoryName = "Electronics", ImageUrl = "emptyproduct.png" },
                new Product { Id = 17, Name = "Avocados", Price = 1.29, Stock = 90, DateAdded = DateTime.Now.AddDays(-3), CategoryName = "Produce", ImageUrl = "emptyproduct.png" },
                new Product { Id = 18, Name = "Bagels", Price = 2.79, Stock = 55, DateAdded = DateTime.Now.AddDays(-1), CategoryName = "Bakery", ImageUrl = "emptyproduct.png" },
                new Product { Id = 19, Name = "Milk", Price = 3.29, Stock = 120, DateAdded = DateTime.Now.AddDays(-4), CategoryName = "Dairy", ImageUrl = "emptyproduct.png" },
                new Product { Id = 20, Name = "Pork Chops", Price = 7.49, Stock = 40, DateAdded = DateTime.Now.AddDays(-2), CategoryName = "Meat", ImageUrl = "emptyproduct.png" }
            };

            PopulateCategoryFilterOptions();
            SortProduct();
        }

        public void PopulateCategoryFilterOptions()
        {
            // Get unique categories from products
            var uniqueCategories = Products
                .Select(p => p.CategoryName)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            CategoryFilterOptions.Clear();
            CategoryFilterOptions.Add("All Categories");

            foreach (var category in uniqueCategories)
            {
                CategoryFilterOptions.Add(category);
            }
        }

        public void ApplyCategoryFilter()
        {
            if (SelectedCategoryFilter == "All Categories")
            {
                DataSource.Filter = null;
            }
            else
            {
                DataSource.Filter = FilterBySelectedCategory;
            }

            DataSource.RefreshFilter();
            OnPropertyChanged(nameof(DataSource));
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
            DataSource = new DataSource() { Source = Products };
            DataSource.Refresh();
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
            SelectedProduct = product;
            if (product != null)
            {
                EditName = product.Name;
                EditPrice = product.Price;
                EditStock = product.Stock;
                EditCategory = product.CategoryName;
                EditImageUrl = product.ImageUrl;
            }
        }

        public void SaveProductChanges()
        {
            if (SelectedProduct != null)
            {
                SelectedProduct.Name = EditName;
                SelectedProduct.Price = EditPrice;
                SelectedProduct.Stock = EditStock;
                SelectedProduct.CategoryName = EditCategory;
                SelectedProduct.ImageUrl = EditImageUrl;

                // Clear selection after saving
                SelectedProduct = null;

                // Refresh the data source and category options
                DataSource.Refresh();
                PopulateCategoryFilterOptions();
            }
        }

        public void CancelEdit()
        {
            SelectedProduct = null;
            EditName = string.Empty;
            EditPrice = 0;
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
            SaveProductChanges();
        });

        public ICommand CancelEditCommand => new Command(() =>
        {
            CancelEdit();
        });

        public ICommand DeleteProductCommand => new Command<Product>((product) =>
        {
            if (product != null)
            {
                Products.Remove(product);
                DataSource.Refresh();
                PopulateCategoryFilterOptions(); // Refresh filter options after deletion
                if (SelectedProduct?.Id == product.Id)
                {
                    SelectedProduct = null;
                }
            }
        });
        #endregion
    }
}
