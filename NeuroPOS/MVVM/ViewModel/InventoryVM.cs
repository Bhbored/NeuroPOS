﻿using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.Popups;
using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.MVVM.View;
using PropertyChanged;
using Syncfusion.Maui.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;
using ListSortDirection = Syncfusion.Maui.DataSource.ListSortDirection;
namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class InventoryVM : INotifyPropertyChanged
    {
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
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
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
        public string EditName { get; set; }
        public double EditPrice { get; set; }
        public double EditCost { get; set; }
        public int EditStock { get; set; }
        public string EditCategory { get; set; }
        public string EditImageUrl { get; set; }
        public int NewProductId => Products.Count > 0 ? Products.Max(p => p.Id) + 1 : 1;
        public string NewProductName { get; set; } = string.Empty;
        public string NewProductPrice { get; set; } = string.Empty;
        public string NewProductCost { get; set; } = string.Empty;
        public Category NewProductCategory { get; set; }
        public string NewProductImageUrl { get; set; } = "emptyproduct.png";
        public int NewCategoryId => Categories.Count > 0 ? Categories.Max(c => c.Id) + 1 : 1;
        public string NewCategoryName { get; set; } = string.Empty;
        public string NewCategoryDescription { get; set; } = string.Empty;
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
        public object PageReference { get; set; }
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
        private ObservableCollection<int> _persistentSelectedIds = new ObservableCollection<int>();
        public bool HasSelectedItems => _persistentSelectedIds?.Count > 0;
        public ObservableCollection<int> PersistentSelectedIds => _persistentSelectedIds;
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
        private bool _isCategoriesDetailsVisible = false;
        public bool IsCategoriesDetailsVisible
        {
            get => _isCategoriesDetailsVisible;
            set
            {
                if (_isCategoriesDetailsVisible != value)
                {
                    _isCategoriesDetailsVisible = value;
                    OnPropertyChanged();
                }
            }
        }
        private List<Category> _selectedCategoriesForEdit = new List<Category>();
        public List<Category> SelectedCategoriesForEdit
        {
            get => _selectedCategoriesForEdit;
            set
            {
                if (_selectedCategoriesForEdit != value)
                {
                    _selectedCategoriesForEdit = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsEditingCategory));
                }
            }
        }
        public bool IsEditingCategory => SelectedCategoriesForEdit?.Count > 0;
        public string EditCategoryName { get; set; }
        public string EditCategoryDescription { get; set; }
        public bool IsEditingThisCategory(Category category)
        {
            return SelectedCategoriesForEdit?.Any(c => c.Id == category.Id) == true;
        }
        private Category _selectedCategoryForDelete;
        public Category SelectedCategoryForDelete
        {
            get => _selectedCategoryForDelete;
            set
            {
                if (_selectedCategoryForDelete != value)
                {
                    _selectedCategoryForDelete = value;
                    OnPropertyChanged();
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
        public void PopulateCategoryFilterOptions()
        {
            var categoriesFromProducts = Products
                .Select(p => p.CategoryName)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct();
            var availableCategories = Categories
                .Select(c => c.Name)
                .Where(c => !string.IsNullOrEmpty(c))
                .Distinct();
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
            OnPropertyChanged(nameof(CategoryFilterOptions));
        }
        public void ApplyCategoryFilter()
        {
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
            DataSource = new DataSource() { Source = Products };
            DataSource.Refresh();
        }
        public void ApplySorting()
        {
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
                App.ProductRepo.UpdateItem(SelectedProduct);
                _ = RefreshDBAsync();
                SelectedProduct = null;
                SelectedEditCategory = null;
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
            return !Products.Any(p => p.Name.Equals(NewProductName, StringComparison.OrdinalIgnoreCase));
        }
        public void AddNewProduct()
        {
            if (!ValidateNewProduct())
                return;
            if (!double.TryParse(NewProductPrice, out double price))
                price = 0;
            if (!double.TryParse(NewProductCost, out double cost))
                cost = 0;
            var newProduct = new Product
            {
                Id = NewProductId,
                Name = NewProductName,
                Price = price,
                Cost = cost,
                Stock = 0,
                CategoryName = NewProductCategory?.Name ?? "Uncategorized",
                ImageUrl = NewProductImageUrl,
                DateAdded = DateTime.Now
            };
            App.ProductRepo.InsertItem(newProduct);
            _ = RefreshDBAsync();
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
            return !Categories.Any(c => c.Name.Equals(NewCategoryName, StringComparison.OrdinalIgnoreCase));
        }
        public void AddNewCategory()
        {
            if (!ValidateNewCategory())
                return;
            var newCategory = new Category
            {
                Id = NewCategoryId,
                Name = NewCategoryName.Trim(),
                Description = string.IsNullOrWhiteSpace(NewCategoryDescription) ? string.Empty : NewCategoryDescription.Trim(),
                State = "Inactive Categorie"
            };
            App.CategoryRepo.InsertItem(newCategory);
            _ = RefreshDBAsync();
            OnPropertyChanged(nameof(Categories));
            OnPropertyChanged(nameof(NewCategoryId));
            PopulateCategoryFilterOptions();
            ClearNewCategoryForm();
        }
        public void UpdateSelectAllCheckboxState()
        {
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
        public void DeleteProductById(int productId)
        {
            var productToDelete = Products.FirstOrDefault(p => p.Id == productId);
            if (productToDelete == null) return;
            Products.Remove(productToDelete);
            ClearAllSelections();
            if (SelectedProduct?.Id == productId)
                SelectedProduct = null;
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
                        App.ProductRepo.DeleteItem(productToDelete);
                        if (SelectedProduct?.Id == productToDelete.Id)
                        {
                            SelectedProduct = null;
                        }
                    }
                }
                _ = RefreshDBAsync();
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
            if (SelectedCategoryFilter != "All Categories" &&
                !Products.Any(p => p.CategoryName
                       .Equals(SelectedCategoryFilter, StringComparison.OrdinalIgnoreCase)))
            {
                SelectedCategoryFilter = "All Categories";
            }
            if (SelectedProducts?.Count > 0)
            {
                var stillExist = SelectedProducts
                    .OfType<Product>()
                    .Where(p => Products.Any(x => x.Id == p.Id))
                    .ToList();
                if (stillExist.Count != SelectedProducts.Count)
                    SelectedProducts.Clear();
            }
            ApplyCategoryFilter();
            DataSource.RefreshFilter();
        }
        public async Task RefreshDBAsync()
        {
            IsRefreshing = true;
            try
            {
                await LoadData();
                await Task.Delay(500);
                ClearAllSelections();
                CancelEdit();
                ClearSearchFilter();
                RefreshUI();
                RevalidateActiveFilters();
            }
            finally
            {
                IsRefreshing = false;
            }
        }
        public void StartEditCategory(Category category)
        {
            foreach (var cat in SelectedCategoriesForEdit)
            {
                cat.IsBeingEdited = false;
            }
            SelectedCategoriesForEdit.Clear();
            if (category != null)
            {
                category.IsBeingEdited = true;
                SelectedCategoriesForEdit.Add(category);
                EditCategoryName = category.Name;
                EditCategoryDescription = category.Description ?? string.Empty;
                OnPropertyChanged(nameof(EditCategoryName));
                OnPropertyChanged(nameof(EditCategoryDescription));
            }
        }
        public void CancelEditCategory()
        {
            foreach (var category in SelectedCategoriesForEdit)
            {
                category.IsBeingEdited = false;
            }
            SelectedCategoriesForEdit.Clear();
            EditCategoryName = string.Empty;
            EditCategoryDescription = string.Empty;
            OnPropertyChanged(nameof(EditCategoryName));
            OnPropertyChanged(nameof(EditCategoryDescription));
        }
        public void SaveCategoryChanges()
        {
            if (SelectedCategoriesForEdit?.Count > 0)
            {
                var oldCategoryNames = SelectedCategoriesForEdit.Select(c => c.Name).Distinct().ToList();
                var DBProducts = App.ProductRepo.GetItems();
                var relatedProducts = DBProducts
                    .Where(p => oldCategoryNames.Contains(p.CategoryName))
                    .ToList();
                foreach (var product in relatedProducts)
                {
                    product.CategoryName = EditCategoryName;
                    App.ProductRepo.UpdateItem(product);
                }
                foreach (var category in SelectedCategoriesForEdit)
                {
                    category.Name = EditCategoryName;
                    category.Description = EditCategoryDescription;
                    App.CategoryRepo.UpdateItem(category);
                }
                _ = RefreshDBAsync();
                CancelEditCategory();
                if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
                {
                    page.ClosePopupAsync();
                }
            }
        }
        public void DeleteCategory(Category category)
        {
            SelectedCategoryForDelete = category;
            var DBProducts = App.ProductRepo.GetItems();
            var relatedProducts = DBProducts
                .Where(p => p.CategoryName == category.Name)
                .ToList();
            foreach (var product in relatedProducts)
            {
                product.CategoryName = "Uncategorized";
                App.ProductRepo.UpdateItem(product);
            }
            App.CategoryRepo.DeleteItem(category);
            SelectedCategoryForDelete = null;
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
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowSaveEditConfirmation();
            }
        });
        public ICommand CancelEditCommand => new Command(() =>
        {
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowCancelEditConfirmation();
            }
        });
        public ICommand DeleteProductCommand => new Command<Product>((product) =>
        {
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowDeleteProductConfirmation(product);
            }
        });
        public ICommand DeleteSelectedProductsCommand => new Command(() =>
        {
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowDeleteSelectedConfirmation();
            }
        });
        public ICommand AddProductCommand => new Command(() =>
        {
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowAddProductPopup();
            }
        });
        public ICommand AddCategoryCommand => new Command(() =>
        {
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
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                page.ShowBuyingTransactionPopup();
            }
        });
        public ICommand AdjustCategoriesCommand => new Command(async () =>
        {
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                var popup = new CategoriesDetailsPopup(this);
                await page.ShowPopupAsync(popup);
            }
        });
        public ICommand CloseCategoriesDetailsCommand => new Command(async () =>
        {
            CancelEditCategory();
            if (PageReference is NeuroPOS.MVVM.View.InventoryPage page)
            {
                await page.ClosePopupAsync();
            }
        });
        public ICommand EditCategoryCommand => new Command<Category>((category) =>
        {
            StartEditCategory(category);
        });
        public ICommand SaveCategoryChangesCommand => new Command(() =>
        {
            SaveCategoryChanges();
        });
        public ICommand CancelEditCategoryCommand => new Command(() =>
        {
            CancelEditCategory();
        });
        public ICommand DeleteCategoryCommand => new Command<Category>((category) =>
        {
            DeleteCategory(category);
        });
        #endregion
        #region Tasks
        private bool _isLoading = false;
        public async Task LoadData()
        {
            if (_isLoading) return;
            _isLoading = true;
            try
            {
                if (App.ProductRepo == null || App.CategoryRepo == null || App.TransactionRepo == null || App.TransactionLineRepo == null)
                {
                    return;
                }
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Products.Clear();
                    Categories.Clear();
                    BuyingTransaction.Clear();
                });
                var DBProducts = await Task.Run(() => App.ProductRepo.GetItems());
                var DBCategories = await Task.Run(() => App.CategoryRepo.GetItems());
                var DBTransactions = await Task.Run(() => App.TransactionRepo.GetItemsWithChildren());
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    Products.Clear();
                    Categories.Clear();
                    BuyingTransaction.Clear();
                    foreach (var item in DBProducts)
                    {
                        Products.Add(item);
                    }
                    foreach (var item in DBCategories)
                    {
                        Categories.Add(item);
                    }
                    foreach (var item in DBTransactions)
                    {
                        BuyingTransaction.Add(item);
                    }
                    SortProduct();
                    PopulateCategoryFilterOptions();
                });
            }
            finally
            {
                _isLoading = false;
            }
        }
        #endregion
    }
}