using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.ViewModel;
using System.Windows.Input;

namespace NeuroPOS.MVVM.Popups;

public partial class DatePickerPopup : Popup
{
    private readonly DatePickerVM _viewModel;
    private readonly TransactionVM _transactionVM;

    public DatePickerPopup(TransactionVM transactionVM)
    {
        InitializeComponent();
        _transactionVM = transactionVM;
        _viewModel = new DatePickerVM();
        DatePickerLimitations();
        BindingContext = _viewModel;
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private async void OnApplyFilterClicked(object sender, EventArgs e)
    {
        try
        {
            if (!_viewModel.IsDateRangeValid)
            {
                await App.Current.MainPage.DisplayAlert("Invalid Date Range",
                    "Please select a valid date range before applying the filter.", "OK");
                return;
            }

            // Apply the filter to the TransactionVM
            await _transactionVM.ApplyDateFilter(_viewModel.StartDate.Value, _viewModel.EndDate.Value);

            await CloseAsync();
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error",
                $"Failed to apply date filter: {ex.Message}", "OK");
        }
    }

    private void DatePickerLimitations()
    {
        // Set calendar limitations to prevent future dates and limit to last year
        this.calendar.MinimumDate = DateTime.Now.AddYears(-1);
        this.calendar.MaximumDate = DateTime.Now;
    }
}