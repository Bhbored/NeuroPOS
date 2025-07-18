using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.MVVM.Model;
using Syncfusion.Maui.Inputs;
using Syncfusion.Maui.ListView;
using System.Threading.Tasks;

namespace NeuroPOS.MVVM.View;

public partial class ContactsPage : ContentPage
{
	public ContactsPage(ContactVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ContactVM vm)
        {
          vm.LoadDB();
        }
    }

    #region Filter and Search
    private void Autocomplete_SelectionChanged(object sender, EventArgs e)
    {
        // Handle autocomplete selection changes
        try
        {
            var autocomplete = sender as Syncfusion.Maui.Inputs.SfAutocomplete;
            if (autocomplete != null)
            {
                var vm = BindingContext as ContactVM;
                if (vm != null)
                {
                    // Update search text based on selected items
                    var selectedItems = autocomplete.SelectedValue as System.Collections.IList;
                    if (selectedItems != null && selectedItems.Count > 0)
                    {
                        var names = new List<string>();
                        foreach (var item in selectedItems)
                        {
                            if (item is NeuroPOS.MVVM.Model.Contact contact)
                            {
                                names.Add(contact.Name);
                            }
                        }
                        vm.SearchText = string.Join(", ", names);
                    }
                    else
                    {
                        vm.SearchText = "";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Autocomplete error: {ex.Message}");
        }
    }
    #endregion




}