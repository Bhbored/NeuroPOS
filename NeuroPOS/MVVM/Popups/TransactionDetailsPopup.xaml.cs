using CommunityToolkit.Maui.Views;

namespace NeuroPOS.MVVM.Popups;

public partial class TransactionDetailsPopup : Popup
{
    public TransactionDetailsPopup()
    {
        InitializeComponent();
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        CloseAsync();
    }
}