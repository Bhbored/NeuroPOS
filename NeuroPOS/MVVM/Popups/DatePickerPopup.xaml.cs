using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.ViewModel;
using System.Diagnostics;
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
        ResetDates();
        BindingContext = _viewModel;
    }

    private void ResetDates()
    {
        try
        {
            _viewModel.StartDate = null;
            _viewModel.EndDate = null;
            Debug.WriteLine("Dates reset to null");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error resetting dates: {ex.Message}");
        }
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }

    private async void OnApplyFilterClicked(object sender, EventArgs e)
    {
        try
        {
            Debug.WriteLine($"OnApplyFilterClicked - StartDate: {_viewModel.StartDate}, EndDate: {_viewModel.EndDate}");
            Debug.WriteLine($"IsDateRangeValid: {_viewModel.IsDateRangeValid}");

            if (!_viewModel.IsDateRangeValid)
            {
                await Snackbar.Make("Please select a valid date range.", null, "ok", duration: TimeSpan.FromSeconds(3)).Show();
                return;
            }

            // Ensure both dates are set before applying filter
            var startDate = _viewModel.StartDate ?? _viewModel.EndDate ?? DateTime.Now;
            var endDate = _viewModel.EndDate ?? _viewModel.StartDate ?? DateTime.Now;

            // Additional safety check - if we still don't have valid dates, use today's date
            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
            {
                Debug.WriteLine("Fallback: Using today's date for both start and end");
                startDate = DateTime.Today;
                endDate = DateTime.Today;
            }

            Debug.WriteLine($"Applying filter with dates: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}");

            // Apply the filter to the TransactionVM
            var filterApplied = await _transactionVM.ApplyDateFilter(startDate, endDate);

            if (!filterApplied)
            {
                // No transactions found in the date range
                await CloseAsync();

                // Show snackbar with appropriate message
                string message;
                if (startDate.Date == endDate.Date)
                {
                    message = $"No transactions found for {startDate:MMM dd, yyyy}";
                }
                else
                {
                    message = $"No transactions found from {startDate:MMM dd} to {endDate:MMM dd, yyyy}";
                }

                var snackbarOptions = new SnackbarOptions
                {
                    BackgroundColor = Colors.Orange,
                    TextColor = Colors.White
                };

                await Snackbar.Make(message, null, "ok", TimeSpan.FromSeconds(3), snackbarOptions).Show();
                return;
            }

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
        try
        {
            // Set calendar limitations to prevent future dates and limit to last year
            if (this.calendar != null)
            {
                this.calendar.MinimumDate = DateTime.Now.AddYears(-1);
                this.calendar.MaximumDate = DateTime.Now;
                Debug.WriteLine("Calendar limitations set successfully");
            }
            else
            {
                Debug.WriteLine("Calendar is null, cannot set limitations");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error setting calendar limitations: {ex.Message}");
        }
    }
}