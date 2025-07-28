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
    private readonly Func<DateTime, DateTime, Task<bool>> _applyDateFilterCallback;
    private readonly string _entityType;

    public DatePickerPopup(Func<DateTime, DateTime, Task<bool>> applyDateFilterCallback, string entityType = "items")
    {
        InitializeComponent();
        _applyDateFilterCallback = applyDateFilterCallback;
        _entityType = entityType;
        _viewModel = new DatePickerVM(entityType);
        DatePickerLimitations();
        ResetDates();
        BindingContext = _viewModel;
    }

    // Constructor for backward compatibility with TransactionVM
    public DatePickerPopup(TransactionVM transactionVM) : this(
        async (startDate, endDate) => await transactionVM.ApplyDateFilter(startDate, endDate), 
        "transactions")
    {
    }

    private void ResetDates()
    {
        _viewModel.StartDate = null;
        _viewModel.EndDate = null;
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
                await Snackbar.Make("Please select a valid date range.", null, "ok", duration: TimeSpan.FromSeconds(3)).Show();
                return;
            }

            var startDate = _viewModel.StartDate ?? _viewModel.EndDate ?? DateTime.Now;
            var endDate = _viewModel.EndDate ?? _viewModel.StartDate ?? DateTime.Now;

            if (startDate == DateTime.MinValue || endDate == DateTime.MinValue)
            {
                startDate = DateTime.Today;
                endDate = DateTime.Today;
            }

            var filterApplied = await _applyDateFilterCallback(startDate, endDate);

            if (!filterApplied)
            {
                await CloseAsync();

                string message;
                if (startDate.Date == endDate.Date)
                {
                    message = $"No {_entityType} found for {startDate:MMM dd, yyyy}";
                }
                else
                {
                    message = $"No {_entityType} found from {startDate:MMM dd} to {endDate:MMM dd, yyyy}";
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
            await Application.Current.MainPage.DisplayAlert("Error",
                $"Failed to apply date filter: {ex.Message}", "OK");
        }
    }

    private void DatePickerLimitations()
    {
        if (this.calendar != null)
        {
            this.calendar.MinimumDate = DateTime.Now.AddYears(-1);
            this.calendar.MaximumDate = DateTime.Now;
        }
    }
}