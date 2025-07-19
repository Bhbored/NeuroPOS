using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.Popups;

public partial class AddContactPopup : Popup
{
    private readonly ContactVM _viewModel;

    public AddContactPopup(ContactVM viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private async void OnAddContactClicked(object sender, EventArgs e)
    {
        try
        {
            await _viewModel.ConfirmAddContact();
            await CloseAsync();
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error",
                $"Failed to add contact: {ex.Message}", "OK");
        }
    }
}