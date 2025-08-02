using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.MVVM.Popups;
using Syncfusion.Maui.ListView;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
namespace NeuroPOS.MVVM.View;
public partial class InventoryPage : ContentPage
{
    private InventoryVM _viewModel;

    private UndoData _lastUndoData;
    public InventoryPage(InventoryVM vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;
        vm.PageReference = this;
        listView.SelectionChanged += ListView_SelectionChanged;
        autocomplete.SelectionChanged += Autocomplete_SelectionChanged;
        SelectAllCheckBox.CheckedChanged += SelectAllCheckBox_CheckedChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is InventoryVM vm)
        {
            if
                (vm.Products.Count == 0 && vm.Categories.Count == 0)
            {
                await vm.RefreshDBAsync();
            }
            vm.RefreshUI();
        }
    }
    #region helper class
    private class UndoData
    {
        public string ActionType { get; set; }
        public Product DeletedProduct { get; set; }
        public List<Product> DeletedProducts { get; set; }
        public Product EditedProduct { get; set; }
        public Product PreviousProductState { get; set; }
        public int ProductIndex { get; set; }
        public List<int> ProductIndices { get; set; }
    }
    #endregion

    #region filter and selection handling
    private void ListView_SelectionChanged(object sender, ItemSelectionChangedEventArgs e)
    {
        if (e.AddedItems?.Count > 0)
        {
            foreach (var item in e.AddedItems)
            {
                if (item is Product product)
                {
                    _viewModel.AddToPersistentSelection(product.Id);
                }
            }
        }
        if (e.RemovedItems?.Count > 0)
        {
            foreach (var item in e.RemovedItems)
            {
                if (item is Product product)
                {
                    _viewModel.RemoveFromPersistentSelection(product.Id);
                }
            }
        }
        _viewModel.UpdateSelectedItemsCountDisplay();
    }
    private void Autocomplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        try
        {
            if (autocomplete != null)
            {
                var vm = BindingContext as InventoryVM;
                var selectedProducts = new List<object>();
                if (autocomplete.SelectedItems != null)
                {
                    foreach (var item in autocomplete.SelectedItems)
                    {
                        if (item is Product product)
                        {
                            selectedProducts.Add(product);
                        }
                    }
                }
                vm.SelectedProducts = selectedProducts;
                if (listView.DataSource != null)
                {
                    listView.DataSource.Filter = FilterProducts;
                    listView.DataSource.RefreshFilter();
                    vm?.RestoreListViewSelections();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in autocomplete selection changed: {ex.Message}");
        }
    }
    private bool FilterProducts(object obj)
    {
        try
        {
            if (obj is not Product product)
                return false;
            var vm = BindingContext as InventoryVM;
            if (vm?.SelectedProducts == null || vm.SelectedProducts.Count == 0)
            {
                return true;
            }
            foreach (var selectedItem in vm.SelectedProducts)
            {
                if (selectedItem is Product selectedProduct)
                {
                    if (product.Id == selectedProduct.Id ||
                        product.Name.Equals(selectedProduct.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in filter: {ex.Message}");
            return true;
        }
    }
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        SortIcon.Source = _viewModel.SortState switch
        {
            InventoryVM.SortDirectionState.Ascending => "ascending.png",
            InventoryVM.SortDirectionState.Descending => "descending.png",
            InventoryVM.SortDirectionState.None => "",
            _ => ""
        };
    }
    private void SelectAllCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            if (e.Value)
            {
                foreach (var item in listView.DataSource.DisplayItems)
                {
                    if (item is Product product)
                    {
                        _viewModel.AddToPersistentSelection(product.Id);
                    }
                }
            }
            else
            {
                _viewModel.ClearAllSelections();
            }
            _viewModel.RestoreListViewSelections();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SelectAll checkbox: {ex.Message}");
        }
    }
    public void ClearSearchFilter()
    {
        try
        {
            if (autocomplete != null)
            {
                autocomplete.SelectedItems?.Clear();
                _viewModel.SelectedProducts?.Clear();
                if (listView.DataSource != null)
                {
                    listView.DataSource.Filter = null;
                    listView.DataSource.RefreshFilter();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error clearing search filter: {ex.Message}");
        }
    }
    #endregion

    #region Snackbar and Undo System
    private async void ShowSnackbarWithUndo(string message, string undoText, Action undoAction)
    {
        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#2D3748"),
            TextColor = Colors.White,
            ActionButtonTextColor = Color.FromArgb("#4299E1"),
            CornerRadius = new CornerRadius(8),
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
            ActionButtonFont = Microsoft.Maui.Font.SystemFontOfSize(14, FontWeight.Bold),
            CharacterSpacing = 0.5
        };
        var snackbar = Snackbar.Make(message, undoAction, undoText, TimeSpan.FromSeconds(5), snackbarOptions);
        await snackbar.Show();
    }
    private async void ShowSuccessSnackbar(string message)
    {
        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Color.FromArgb("#38A169"),
            TextColor = Colors.White,
            CornerRadius = new CornerRadius(8),
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
            CharacterSpacing = 0.5
        };
        var snackbar = Snackbar.Make(message, null, "", TimeSpan.FromSeconds(3), snackbarOptions);
        await snackbar.Show();
    }
    private void UndoDeleteProduct()
    {
        if (_lastUndoData?.ActionType == "DELETE_PRODUCT" && _lastUndoData.DeletedProduct != null)
        {
            var productName = _lastUndoData.DeletedProduct.Name;
            if (_lastUndoData.ProductIndex >= 0 && _lastUndoData.ProductIndex <= _viewModel.Products.Count)
            {
                _viewModel.Products.Insert(_lastUndoData.ProductIndex, _lastUndoData.DeletedProduct);
            }
            else
            {
                _viewModel.Products.Add(_lastUndoData.DeletedProduct);
            }
            _viewModel.DataSource.Source = _viewModel.Products;
            _viewModel.DataSource.Refresh();
            _viewModel.PopulateCategoryFilterOptions();
            _viewModel.RevalidateActiveFilters();
            _lastUndoData = null;
            ShowSuccessSnackbar($"'{productName}' restored successfully");
        }
    }
    private void UndoDeleteSelectedProducts()
    {
        if (_lastUndoData?.ActionType == "DELETE_SELECTED" && _lastUndoData.DeletedProducts != null)
        {
            var count = _lastUndoData.DeletedProducts.Count;
            foreach (var product in _lastUndoData.DeletedProducts)
            {
                App.ProductRepo.InsertItem(product);
            }
            _ = _viewModel.RefreshDBAsync();
            _lastUndoData = null;
            ShowSuccessSnackbar($"{count} product{(count > 1 ? "s" : "")} restored successfully");
        }
    }
    private void UndoEditProduct()
    {
        if (_lastUndoData?.ActionType == "EDIT_PRODUCT" && _lastUndoData.EditedProduct != null && _lastUndoData.PreviousProductState != null)
        {
            var product = _lastUndoData.EditedProduct;
            var previous = _lastUndoData.PreviousProductState;
            var productName = previous.Name;
            product.Name = previous.Name;
            product.Price = previous.Price;
            product.Cost = previous.Cost;
            product.Stock = previous.Stock;
            product.CategoryName = previous.CategoryName;
            product.ImageUrl = previous.ImageUrl;
            App.ProductRepo.UpdateItem(product);
            _ = _viewModel.RefreshDBAsync();
            _viewModel.RevalidateActiveFilters();
            _lastUndoData = null;
            ShowSuccessSnackbar($"'{productName}' changes reverted successfully");
        }
    }
    #endregion

    #region POPUPS logic

    public async void ShowDeleteProductConfirmation(Product product)
    {
        if (product != null)
        {
            var popup = new DeleteConfirmationPopup($"Are you sure you want to delete '{product.Name}'?");
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            if (popup.Result)
            {
                var productIndex = _viewModel.Products.IndexOf(product);
                _lastUndoData = new UndoData
                {
                    ActionType = "DELETE_PRODUCT",
                    DeletedProduct = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Cost = product.Cost,
                        Stock = product.Stock,
                        CategoryName = product.CategoryName,
                        ImageUrl = product.ImageUrl,
                        DateAdded = product.DateAdded
                    },
                    ProductIndex = productIndex
                };
                _viewModel.DeleteProductById(product.Id);
                ClearSearchFilter();
                _viewModel.RevalidateActiveFilters();
                ShowSnackbarWithUndo($"'{product.Name}' deleted", "UNDO", UndoDeleteProduct);
            }
        }
    }
    public async void ShowDeleteSelectedConfirmation()
    {
        var selectedIds = _viewModel.PersistentSelectedIds?.ToList();
        if (selectedIds != null && selectedIds.Count > 0)
        {
            var count = selectedIds.Count;
            var popup = new DeleteConfirmationPopup($"Are you sure you want to delete {count} selected product{(count > 1 ? "s" : "")}?");
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            if (popup.Result)
            {
                var deletedProducts = new List<Product>();
                foreach (var id in selectedIds)
                {
                    var product = _viewModel.Products.FirstOrDefault(p => p.Id == id);
                    if (product != null)
                    {
                        deletedProducts.Add(new Product
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Cost = product.Cost,
                            Stock = product.Stock,
                            CategoryName = product.CategoryName,
                            ImageUrl = product.ImageUrl,
                            DateAdded = product.DateAdded
                        });
                    }
                }
                _lastUndoData = new UndoData
                {
                    ActionType = "DELETE_SELECTED",
                    DeletedProducts = deletedProducts
                };
                _viewModel.DeleteSelectedProductsByIds();
                ClearSearchFilter();
                _viewModel.RevalidateActiveFilters();
                ShowSnackbarWithUndo($"{count} product{(count > 1 ? "s" : "")} deleted", "UNDO", UndoDeleteSelectedProducts);
            }
        }
    }
    public async void ShowCancelEditConfirmation()
    {
        if (_viewModel.HasUnsavedChanges())
        {
            var popup = new EditConfirmationPopup("Do you want to save your changes before canceling?");
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            switch (popup.Result)
            {
                case EditConfirmationResult.Save:
                    await SaveProductWithSnackbar();
                    break;
                case EditConfirmationResult.Discard:
                    _viewModel.CancelEdit();
                    ClearSearchFilter();
                    ShowSuccessSnackbar("Changes discarded");
                    break;
                case EditConfirmationResult.Cancel:
                    break;
            }
        }
        else
        {
            _viewModel.CancelEdit();

        }
    }
    public async void ShowSaveEditConfirmation()
    {
        if (_viewModel.HasUnsavedChanges())
        {
            var popup = new EditConfirmationPopup("Are you sure you want to save these changes?");
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            switch (popup.Result)
            {
                case EditConfirmationResult.Save:
                    await SaveProductWithSnackbar();
                    break;
                case EditConfirmationResult.Discard:
                    _viewModel.CancelEdit();
                    ClearSearchFilter();
                    ShowSuccessSnackbar("Changes discarded");
                    break;
                case EditConfirmationResult.Cancel:
                    break;
            }
        }
        else
        {
            await SaveProductWithSnackbar();
        }
    }
    private async Task SaveProductWithSnackbar()
    {
        if (_viewModel.SelectedProduct != null)
        {
            _lastUndoData = new UndoData
            {
                ActionType = "EDIT_PRODUCT",
                EditedProduct = _viewModel.SelectedProduct,
                PreviousProductState = new Product
                {
                    Id = _viewModel.SelectedProduct.Id,
                    Name = _viewModel.SelectedProduct.Name,
                    Price = _viewModel.SelectedProduct.Price,
                    Cost = _viewModel.SelectedProduct.Cost,
                    Stock = _viewModel.SelectedProduct.Stock,
                    CategoryName = _viewModel.SelectedProduct.CategoryName,
                    ImageUrl = _viewModel.SelectedProduct.ImageUrl,
                    DateAdded = _viewModel.SelectedProduct.DateAdded
                }
            };
            var productName = _viewModel.SelectedProduct.Name;
            _viewModel.SaveProductChanges();
            ClearSearchFilter();
            _viewModel.RevalidateActiveFilters();
            ShowSnackbarWithUndo($"'{productName}' updated successfully", "UNDO", UndoEditProduct);
        }
    }
    public async void ShowAddProductPopup()
    {
        _viewModel.ClearNewProductForm();
        var popup = new AddProductPopup();
        popup.BindingContext = _viewModel;
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        if (popup.Result == AddProductResult.Add)
        {
            if (!_viewModel.ValidateNewProduct())
            {
                await DisplayAlert("Product Already Exists",
                    $"A product with the name '{_viewModel.NewProductName}' already exists. Please edit the existing product's stock instead.",
                    "OK");
                return;
            }
            var productName = _viewModel.NewProductName;
            _viewModel.AddNewProduct();
            ClearSearchFilter();
            _viewModel.RevalidateActiveFilters();
            ShowSuccessSnackbar($"'{productName}' added successfully");
        }
    }
    public async void ShowAddCategoryPopup()
    {
        _viewModel.ClearNewCategoryForm();
        var popup = new AddCategoryPopup(_viewModel);
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        _viewModel.RevalidateActiveFilters();
    }
    public async void ShowBuyingTransactionPopup()
    {
        var popup = new BuyingTransactionPopup(_viewModel);
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        _viewModel.RevalidateActiveFilters();
    }
    private async void OnChangeImageClicked(object sender, EventArgs e)
    {
        try
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.image" } },
                { DevicePlatform.Android, new[] { "image/*" } },
                { DevicePlatform.WinUI, new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" } },
                { DevicePlatform.Tizen, new[] { "*/*" } },
                { DevicePlatform.macOS, new[] { "public.image" } }
            });
            var options = new PickOptions
            {
                PickerTitle = "Select Product Image",
                FileTypes = customFileType
            };
            var result = await FilePicker.Default.PickAsync(options);
            if (result != null)
            {
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var extension = Path.GetExtension(result.FileName);
                var newFileName = $"product_{timestamp}{extension}";
                var localPath = Path.Combine(FileSystem.Current.AppDataDirectory, "ProductImages");
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }
                var destinationPath = Path.Combine(localPath, newFileName);
                using (var sourceStream = await result.OpenReadAsync())
                using (var destinationStream = File.Create(destinationPath))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }
                _viewModel.UpdateImageUrl(destinationPath);
                await DisplayAlert("Success", "Image updated successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to select image: {ex.Message}", "OK");
        }
    }

    #endregion


}