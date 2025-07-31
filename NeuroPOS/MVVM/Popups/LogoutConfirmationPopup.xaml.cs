using CommunityToolkit.Maui.Views;

using NeuroPOS.MVVM.Popups;

namespace NeuroPOS.MVVM.Popups;

public partial class LogoutConfirmationPopup : Popup
{
    public LogoutConfirmationPopup()
    {
        InitializeComponent();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        CloseAsync();
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        if (Application.Current?.MainPage is AppShell appShell)
        {
            appShell.PerformLogout();
        }
        CloseAsync();
    }
}