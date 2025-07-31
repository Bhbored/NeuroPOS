using NeuroPOS.Services;

namespace NeuroPOS.MVVM.View;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService;

    public LoginPage(AuthService authService)
    {
        InitializeComponent();
        _authService = authService;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        bool rememberMe = RememberMeCheckBox.IsChecked;

        if (_authService.Login(UsernameEntry.Text, PasswordEntry.Text, rememberMe))
        {
            await Shell.Current.GoToAsync("//HomePage");
        }
        else
        {
            await DisplayAlert("Error", "Invalid credentials", "OK");
        }
    }
}