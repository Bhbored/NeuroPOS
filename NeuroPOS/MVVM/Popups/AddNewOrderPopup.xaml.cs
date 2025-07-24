using CommunityToolkit.Maui.Views;

namespace NeuroPOS.MVVM.Popups
{
    public enum AddNewOrderResult
    {
        Cancel,
        Add
    }

    public partial class AddNewOrderPopup : Popup
    {
        public AddNewOrderResult Result { get; private set; } = AddNewOrderResult.Cancel;

        public AddNewOrderPopup()
        {
            InitializeComponent();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            Result = AddNewOrderResult.Cancel;
            await CloseAsync();
        }

        private async void OnAddClicked(object sender, EventArgs e)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(CustomerNameEntry.Text))
            {
                await Application.Current.MainPage.DisplayAlert("Validation Error", "Please enter a customer name.", "OK");
                return;
            }

            Result = AddNewOrderResult.Add;
            await CloseAsync();
        }
    }
}