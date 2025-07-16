using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using System.Windows.Input;

namespace NeuroPOS.MVVM.Popups
{
    public partial class EditTaxPopup : Popup
    {
        public double Result { get; private set; }
        private readonly double _originalTaxRate;

        public EditTaxPopup(double currentTaxRate)
        {
            InitializeComponent();
            _originalTaxRate = currentTaxRate;
            Result = currentTaxRate; // Default to original rate
            TaxEntry.Text = currentTaxRate.ToString("F1");
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (double.TryParse(TaxEntry.Text, out double newTaxRate) && newTaxRate >= 0 && newTaxRate <= 100)
            {
                Result = newTaxRate;
                await AppShell.Current.ClosePopupAsync(this);
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            Result = _originalTaxRate;
            await AppShell.Current.ClosePopupAsync(this);
        }
    }
}