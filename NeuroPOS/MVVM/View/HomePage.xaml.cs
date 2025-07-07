using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using Syncfusion.Maui.DataSource.Extensions;
using Syncfusion.Maui.Inputs;
using System.Collections.ObjectModel;

namespace NeuroPOS.MVVM.View;

public partial class HomePage : ContentPage
{
    public HomePage(HomeVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
        this.listView.ItemGenerator = new Animation.ItemGeneratorExt(this.listView);
        vm.SelectedItems.Clear();//intially clear selected items
    }

    #region filering logic
    private void autocomplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        try
        {
            if (autocomplete != null)
            {
                // Update selected count
                selectedValue.Text = autocomplete.SelectedItems?.Count.ToString() ?? "0";

                // Get the ViewModel
                var vm = BindingContext as HomeVM;

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

                // Update the selected products in ViewModel
                vm.SelectedProducts = selectedProducts;

                // Apply filtering
                if (listView.DataSource != null)
                {
                    listView.DataSource.Filter = FilterProducts;
                    listView.DataSource.RefreshFilter();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in selection changed: {ex.Message}");
            // Optionally show an alert to the user
            // await DisplayAlert("Error", "An error occurred while filtering products", "OK");
        }
    }

    private bool FilterProducts(object obj)
    {
        try
        {
            if (obj is not Product product)
                return false;

            var vm = BindingContext as HomeVM;

            // If no items are selected, show all products
            if (vm?.SelectedProducts == null || vm.SelectedProducts.Count == 0)
            {
                return true;
            }

            // Check if the product matches any of the selected items
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
    #endregion

    //dynamic Icon
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e) 
    {
        if (sender is not Border border)
            return;
        var vm = BindingContext as HomeVM;
        if (vm.SortState == HomeVM.SortDirectionState.Ascending)
        {
            Icon.Source = "ascending.png";
        }
        else if (vm.SortState == HomeVM.SortDirectionState.Descending)
        {
            Icon.Source = "descending.png";
        }
        else { Icon.Source = ""; }
    }


}