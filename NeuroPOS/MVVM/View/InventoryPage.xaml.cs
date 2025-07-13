using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.MVVM.Popups;
using Syncfusion.Maui.ListView;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;

namespace NeuroPOS.MVVM.View;

public partial class InventoryPage : ContentPage
{
    private InventoryVM _viewModel;

    public InventoryPage(InventoryVM vm)
    {
        InitializeComponent();
        _viewModel = vm;
        BindingContext = vm;

        // Set page reference for callbacks
        vm.PageReference = this;

        // Subscribe to ListView events
        listView.SelectionChanged += ListView_SelectionChanged;
        autocomplete.SelectionChanged += Autocomplete_SelectionChanged;

        // Subscribe to SelectAll checkbox
        SelectAllCheckBox.CheckedChanged += SelectAllCheckBox_CheckedChanged;
    }

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
                // Get the ViewModel
                var vm = BindingContext as InventoryVM;

                // Create a new list to avoid reference issues
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

                // Update the selected products in ViewModel for filtering
                vm.SelectedProducts = selectedProducts;

                // Apply filtering
                if (listView.DataSource != null)
                {
                    listView.DataSource.Filter = FilterProducts;
                    listView.DataSource.RefreshFilter();

                    // Restore selections after search filtering
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

            // If no search items are selected, show all products (default behavior)
            if (vm?.SelectedProducts == null || vm.SelectedProducts.Count == 0)
            {
                return true;
            }

            // Check if the product matches any of the search selected items
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
            return true; // Default to showing the product if there's an error
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        // Update sort icon based on sort state
        SortIcon.Source = _viewModel.SortState switch
        {
            InventoryVM.SortDirectionState.Ascending => "ascending.png",
            InventoryVM.SortDirectionState.Descending => "descending.png",
            InventoryVM.SortDirectionState.None => "ascending.png",
            _ => "ascending.png"
        };
    }

    private void SelectAllCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            if (e.Value)
            {
                // Select all visible products
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
                // Unselect all products
                _viewModel.ClearAllSelections();
            }

            // Refresh the ListView selections
            _viewModel.RestoreListViewSelections();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in SelectAll checkbox: {ex.Message}");
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.RefreshUI();
    }

    // Method to clear search filter and show all products
    public void ClearSearchFilter()
    {
        try
        {
            if (autocomplete != null)
            {
                autocomplete.SelectedItems?.Clear();
                _viewModel.SelectedProducts?.Clear();

                // Remove filter to show all products
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



    // Popup methods for confirmations
    public async void ShowDeleteProductConfirmation(Product product)
    {
        if (product != null)
        {
            var popup = new DeleteConfirmationPopup($"Are you sure you want to delete '{product.Name}'?");
            await Shell.Current.CurrentPage.ShowPopupAsync(popup);

            if (popup.Result)
            {
                _viewModel.DeleteProductById(product.Id);
                ClearSearchFilter(); // Clear search filter to show all remaining products
            }
        }
    }

    public async void ShowDeleteSelectedConfirmation()
    {
        var popup = new DeleteConfirmationPopup($"Are you sure you want to delete the selected products?");
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        if (popup.Result)
        {
            _viewModel.DeleteSelectedProductsByIds();
            ClearSearchFilter(); // Clear search filter to show all remaining products
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
                    _viewModel.SaveProductChanges();
                    ClearSearchFilter(); // Clear search filter after saving changes
                    break;
                case EditConfirmationResult.Discard:
                    _viewModel.CancelEdit();
                    ClearSearchFilter(); // Clear search filter after discarding changes
                    break;
                case EditConfirmationResult.Cancel:
                    // Do nothing - stay in edit mode
                    break;
            }
        }
        else
        {
            // No changes, just cancel
            _viewModel.CancelEdit();
        }
    }

    public async void ShowSaveEditConfirmation()
    {
        var popup = new EditConfirmationPopup("Are you sure you want to save these changes?");
        await Shell.Current.CurrentPage.ShowPopupAsync(popup);

        switch (popup.Result)
        {
            case EditConfirmationResult.Save:
                _viewModel.SaveProductChanges();
                ClearSearchFilter(); // Clear search filter after saving changes
                break;
            case EditConfirmationResult.Discard:
                // User chose to discard changes
                _viewModel.CancelEdit();
                ClearSearchFilter(); // Clear search filter after discarding changes
                break;
            case EditConfirmationResult.Cancel:
                // Do nothing - stay in edit mode
                break;
        }
    }

    // Image picker functionality
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
                // Generate unique filename
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var extension = Path.GetExtension(result.FileName);
                var newFileName = $"product_{timestamp}{extension}";

                // For development, let's use the local app data directory
                var localPath = Path.Combine(FileSystem.Current.AppDataDirectory, "ProductImages");

                // Create directory if it doesn't exist
                if (!Directory.Exists(localPath))
                {
                    Directory.CreateDirectory(localPath);
                }

                var destinationPath = Path.Combine(localPath, newFileName);

                // Copy the file
                using (var sourceStream = await result.OpenReadAsync())
                using (var destinationStream = File.Create(destinationPath))
                {
                    await sourceStream.CopyToAsync(destinationStream);
                }

                // Update the ViewModel with the new image path
                _viewModel.UpdateImageUrl(destinationPath);

                await DisplayAlert("Success", "Image updated successfully!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to select image: {ex.Message}", "OK");
        }
    }
}