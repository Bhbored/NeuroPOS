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
        vm.SelectedItems.Clear();
        RefreshListView();
        vm.PageReference = this;

    }

    #region filering logic
    public void ListView_SelectionChanged(object sender, Syncfusion.Maui.ListView.ItemSelectionChangedEventArgs e)
    {
        try
        {

            var vm = BindingContext as HomeVM;
            if (vm != null)
            {
                var listView = sender as Syncfusion.Maui.ListView.SfListView;

                // Update the selected count display based on actual ListView selection (for cart)
                // Update the selected items display using persistent count
                vm?.UpdateSelectedItemsCountDisplay();

                // Update the ViewModel SelectedItems and persistent selection
                if (e.AddedItems != null)
                {
                    foreach (var item in e.AddedItems)
                    {
                        // Small if - only add to SelectedItems if stock > 0
                        if (item is Product product && product.Stock > 0)
                        {
                            if (!vm.SelectedItems.Contains(item))
                            {
                                vm.SelectedItems.Add(item);
                                vm.AddToPersistentSelection(product.Id);
                            }
                        }
                        else if (item is Product zeroStockProduct)
                        {
                            // Remove from ListView selection immediately
                            listView.SelectedItems.Remove(item);
                        }
                    }
                }

                if (e.RemovedItems != null)
                {
                    foreach (var item in e.RemovedItems)
                    {
                        vm.SelectedItems.Remove(item);
                        if (item is Product product)
                        {
                            vm.RemoveFromPersistentSelection(product.Id);
                        }
                    }
                }

                // Add newly selected items to current order
                if (e.AddedItems != null)
                {
                    foreach (var item in e.AddedItems)
                    {
                        if (item is Product product)
                        {
                            // Small if - don't add items with 0 stock
                            if (product.Stock > 0)
                            {
                                // Check if this product is already in the cart before adding
                                var existingCartItem = vm.CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                                if (existingCartItem == null)
                                {
                                    vm.AddToCurrentOrder(product, fromListViewSelection: true);
                                }
                                else
                                {
                                    // If already in cart, just increment quantity
                                    vm.IncrementQuantity(existingCartItem);
                                }
                            }
                            else
                            {
                                // Remove selection for 0 stock items
                                listView.SelectedItems.Remove(item);
                            }
                        }
                        else
                        {
                        }
                    }
                }


                // Remove unselected items from current order 
                if (e.RemovedItems != null)
                {
                    foreach (var item in e.RemovedItems)
                    {
                        if (item is Product product)
                        {
                            var existingItem = vm.CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                            if (existingItem != null)
                            {
                                // Use RemoveFromCurrentOrder to properly update stock display
                                vm.RemoveFromCurrentOrder(existingItem);
                            }
                        }
                    }
                }


                // Notify that selection has changed for HasSelectedItems property
                vm.NotifySelectionChanged();

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Error in ListView selection changed: {ex.Message}");
            Console.WriteLine($"[ERROR] Stack trace: {ex.StackTrace}");
        }
    }
    private void autocomplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        try
        {
            if (autocomplete != null)
            {
                // Update the search filter count display - shows how many items selected in autocomplete
                searchFilterValue.Text = autocomplete.SelectedItems?.Count.ToString() ?? "0";

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

                // Update the selected products in ViewModel for filtering
                vm.SelectedProducts = selectedProducts;

                // Apply filtering with proper null checks
                if (listView?.DataSource != null)
                {
                    listView.DataSource.Filter = FilterProducts;

                    try
                    {
                        listView.DataSource.RefreshFilter();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error refreshing filter: {ex.Message}");
                    }

                    // Restore selections after search filtering
                    vm?.RestoreListViewSelections();

                    // Update selected count after filtering
                    vm?.UpdateSelectedItemsCountDisplay();
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

    #region other logic
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

    public void RefreshListView()
    {
        this.listView.ItemGenerator = new Animation.ItemGeneratorExt(this.listView);
        searchFilterValue.Text = "0"; // Search filter starts with 0 items selected
        selectedValue.Text = "0"; // No items selected initially
        this.listView.SelectionChanged += ListView_SelectionChanged;
    }

    #endregion


    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is HomeVM vm)
        {
            vm.LoadDB();
        }
    }

}