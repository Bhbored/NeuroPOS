using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.Popups;

public partial class ViewContactDetailsPopup : Popup
{
    private readonly ContactVM _viewModel;

    public ViewContactDetailsPopup(ContactVM viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}