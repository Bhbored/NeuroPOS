using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using Syncfusion.Maui.DataSource.Extensions;
using Syncfusion.Maui.Inputs;
using System.Collections.ObjectModel;
namespace NeuroPOS.MVVM.View;

public partial class HomePage : ContentPage
{

    SfAutocomplete editor = null;
    public HomePage()
	{
		InitializeComponent();
        BindingContext = new HomeVM();
        this.listView.ItemGenerator = new Animation.ItemGeneratorExt(this.listView);
    }

    private void autocomplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        if (autocomplete != null && autocomplete.SelectedValue is IList<object> value)
        {
            selectedValue.Text = value.Count.ToString();
        }
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if(sender is not Border border)
            return;
        var vm = BindingContext as HomeVM;
        if(vm.SortState == HomeVM.SortDirectionState.Ascending)
        {
            Icon.Source = "ascending.png";
        }
        else if(vm.SortState == HomeVM.SortDirectionState.Descending)
        {
            Icon.Source = "descending.png";
        }
        else { Icon.Source = ""; }
    }

    private void autocomplete_SelectionChanged(object sender, EventArgs e)
    {

        editor = sender as SfAutocomplete;
        if(listView.DataSource != null)
        {
            listView.DataSource.Filter = FilterProducts;
            listView.DataSource.RefreshFilter();
        }



    }
    private bool FilterProducts(object obj)
    {
            if (editor == null || string.IsNullOrEmpty(editor.Text))
        {
            return true; // No filter applied
        }
        var product = obj as Product;
        List<Product> products = new List<Product>();
        foreach (var item in autocomplete.SelectedItems)
        {
            products.Add(item as Product);
        }
        foreach (var item in products)
        {
            if (product.Name.ToLower().Contains(item.Name.ToLower()) || product.Id == item.Id) ;
        return true; // Product matches the filter criteria

        }
        return false;
        


    }

   
}
