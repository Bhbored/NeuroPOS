using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.Popups;
using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.View;

public partial class TransactionPage : ContentPage
{
    public TransactionPage(TransactionVM vm)
    {
        InitializeComponent();

        if (vm == null)
        {
            vm = new TransactionVM();
        }

        BindingContext = vm;

        // Wire up Picker event handlers
        StatusFilterPicker.SelectedIndexChanged += OnStatusFilterChanged;
        TypeFilterPicker.SelectedIndexChanged += OnTypeFilterChanged;
        SortFilterPicker.SelectedIndexChanged += OnSortFilterChanged;

        // Use proper async initialization
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await LoadDataAsync(vm);
        });
    }

    private void OnStatusFilterChanged(object sender, EventArgs e)
    {
        if (BindingContext is TransactionVM vm && sender is Picker picker)
        {
            vm.SelectedStatusFilter = picker.SelectedItem?.ToString() ?? "All Status";
            vm.OnStatusFilterChanged();
        }
    }

    private void OnTypeFilterChanged(object sender, EventArgs e)
    {
        if (BindingContext is TransactionVM vm && sender is Picker picker)
        {
            vm.SelectedTypeFilter = picker.SelectedItem?.ToString() ?? "All Types";
            vm.OnTypeFilterChanged();
        }
    }

    private void OnSortFilterChanged(object sender, EventArgs e)
    {
        if (BindingContext is TransactionVM vm && sender is Picker picker)
        {
            vm.SelectedSortFilter = picker.SelectedItem?.ToString() ?? "Newest First";
            vm.OnSortFilterChanged();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Refresh data when page appears to show new transactions
        if (BindingContext is TransactionVM vm && !vm.IsInitialized)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await vm.LoadData();
            });
        }
    }

    //public async Task RefreshTransactionData()
    //{
    //    if (BindingContext is TransactionVM vm)
    //    {
    //        await vm.ForceRefreshData();
    //    }
    //}

    private async Task LoadDataAsync(TransactionVM vm)
    {
        try
        {
            if (vm != null)
            {
                await vm.LoadData();
            }
        }
        catch (Exception)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await DisplayAlert("Error", "Failed to load transaction data. Please try again.", "OK");
            });
        }
    }

    private async void ShowDatePicker(object sender, EventArgs e)
    {
        try
        {
            if (BindingContext is TransactionVM transactionVM)
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new DatePickerPopup(transactionVM));
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Failed to show date picker.", "OK");
        }
    }
}