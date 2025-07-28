using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using Syncfusion.Maui.Inputs;
using Contact = NeuroPOS.MVVM.Model.Contact;

namespace NeuroPOS.MVVM.Popups
{
    public partial class OnTabPaymentPopup : Popup
    {
        public bool IsConfirmed { get; private set; } = false;
        public Contact SelectedContact { get; private set; }
        private readonly HomeVM _viewModel;

        public OnTabPaymentPopup(HomeVM viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = viewModel;

            // Wire up the autocomplete selection changed event
            ContactAutocomplete.SelectionChanged += OnContactSelectionChanged;
        }

        private void OnContactSelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                SelectedContact = e.AddedItems[0] as Contact;
            }
        }

        private async void OnConfirmClicked(object sender, EventArgs e)
        {
            if (SelectedContact == null)
            {
                await Snackbar.Make("Please select a contact before proceeding.",
                    duration: TimeSpan.FromSeconds(3)).Show();
                return;
            }

            IsConfirmed = true;
            await CloseAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            IsConfirmed = false;
            await CloseAsync();
        }
    }
}