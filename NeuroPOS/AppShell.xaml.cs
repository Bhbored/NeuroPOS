using CommunityToolkit.Maui.Extensions;
using NeuroPOS.MVVM.Popups;
using NeuroPOS.MVVM.View;
using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.Services;

namespace NeuroPOS
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;
        public AppShell(AuthService authService)
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(TransactionPage), typeof(TransactionPage));
            //Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            _authService = authService;
            UpdateFlyoutItems();
        }


        private async void OnLogoutButtonClicked(object sender, EventArgs e)
        {
            await this.ShowPopupAsync(new LogoutConfirmationPopup());
        }
        public void UpdateFlyoutItems()
        {
            InventoryShellContent.IsVisible = _authService.IsAdmin;
        }
        public async void PerformLogout()
        {
            _authService.Logout();
            InventoryShellContent.IsVisible = false;
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
