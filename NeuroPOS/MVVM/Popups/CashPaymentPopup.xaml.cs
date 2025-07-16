using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.Popups
{
    public partial class CashPaymentPopup : Popup
    {
        public bool IsConfirmed { get; private set; } = false;

        public CashPaymentPopup(HomeVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }


        private async void OnConfirmClicked(object sender, EventArgs e)
        {
            IsConfirmed = true;
            await AppShell.Current.ClosePopupAsync(this);
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            IsConfirmed = false;
            await AppShell.Current.ClosePopupAsync(this);
        }
    }
}