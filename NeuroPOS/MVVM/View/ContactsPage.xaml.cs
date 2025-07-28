using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.MVVM.Model;
using Syncfusion.Maui.Inputs;
using Syncfusion.Maui.ListView;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Contact = NeuroPOS.MVVM.Model.Contact;

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
            vm.RefreshContactsCommand.Execute(null);
        }
    }

    #region Filter and Search
    private void Autocomplete_SelectionChanged(object sender, EventArgs e)
    {
        try
        {
            var autocomplete = sender as Syncfusion.Maui.Inputs.SfAutocomplete;
            if (autocomplete != null)
            {
                var vm = BindingContext as ContactVM;
                if (vm != null)
                {
                    // Create a new list to avoid reference issues
                    var selectedContacts = new List<Contact>();
                    if (autocomplete.SelectedItems != null)
                    {
                        foreach (var item in autocomplete.SelectedItems)
                        {
                            if (item is Contact contact)
                            {
                                selectedContacts.Add(contact);
                            }
                        }
                    }

                    // Update the selected contacts in ViewModel for filtering
                    vm.AutocompleteSelectedContacts = new ObservableCollection<Contact>(selectedContacts);

                    // Apply filtering with proper null checks
                    if (vm.DataSource != null)
                    {
                        vm.DataSource.Filter = FilterContacts;

                        try
                        {
                            vm.DataSource.RefreshFilter();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error refreshing filter: {ex.Message}");
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Autocomplete error: {ex.Message}");
        }
    }

    private bool FilterContacts(object obj)
    {
        try
        {
            if (obj is not Contact contact)
                return false;

            var vm = BindingContext as ContactVM;

            // If no items are selected, show all contacts
            if (vm?.AutocompleteSelectedContacts == null || vm.AutocompleteSelectedContacts.Count == 0)
            {
                return true;
            }

            // Check if the contact matches any of the selected items
            foreach (var selectedContact in vm.AutocompleteSelectedContacts)
            {
                if (contact.Id == selectedContact.Id ||
                    contact.Name.Equals(selectedContact.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in filter: {ex.Message}");
            return true; // Default to showing the contact if there's an error
        }
    }
    #endregion




}