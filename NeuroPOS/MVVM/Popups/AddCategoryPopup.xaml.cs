using NeuroPOS.MVVM.ViewModel;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace NeuroPOS.MVVM.Popups;

public partial class AddCategoryPopup : Popup
{
    private InventoryVM _inventoryVM;

    public AddCategoryPopup(InventoryVM inventoryVM)
    {
        InitializeComponent();
        _inventoryVM = inventoryVM;
        BindingContext = _inventoryVM;
    }

    private async void OnAddCategoryClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_inventoryVM.NewCategoryName))
        {
            await App.Current.MainPage.DisplayAlert("Error", "Please enter a category name.", "OK");
            return;
        }

        if (!_inventoryVM.ValidateNewCategory())
        {
            await App.Current.MainPage.DisplayAlert("Error", "A category with this name already exists.", "OK");
            return;
        }

        // Store category name before adding (since form gets cleared)
        var categoryName = _inventoryVM.NewCategoryName;

        _inventoryVM.AddNewCategory();

        // Show success snackbar
        var snackbar = Snackbar.Make($"Category '{categoryName}' created successfully",
            duration: TimeSpan.FromSeconds(3));
        await snackbar.Show();

        await CloseAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        _inventoryVM.ClearNewCategoryForm();
        await CloseAsync();
    }
}