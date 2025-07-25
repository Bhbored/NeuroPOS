using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.Popups;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Transaction = NeuroPOS.MVVM.Model.Transaction;
namespace NeuroPOS.MVVM.ViewModel;
[AddINotifyPropertyChangedInterface]
public partial class TransactionVM : ObservableObject
{
    public TransactionVM()
    {
        AllTransactions = new ObservableCollection<Transaction>();
        Transactions = new ObservableCollection<Transaction>();
        StatusFilterOptions = new ObservableCollection<string> { "All Status", "Completed", "Pending" };
        TypeFilterOptions = new ObservableCollection<string> { "All Types", "buy", "sell" };
        SortFilterOptions = new ObservableCollection<string> { "Newest First", "Oldest First" };
        SelectedStatusFilter = "All Status";
        SelectedTypeFilter = "All Types";
        SelectedSortFilter = "Newest First";
        TransactionCount = "";
        IsRefreshing = false;
        IsLoading = false;
        IsInitialized = false;
        AnyExpanded = false;
    }
    #region Properties
    [ObservableProperty]
    private ObservableCollection<Transaction> _allTransactions = new();
    private ObservableCollection<Transaction> _transactions = new();
    [ObservableProperty]
    private bool _anyExpanded = false;
    [ObservableProperty]
    private string _transactionCount = "";
    [ObservableProperty]
    private bool _isRefreshing = false;
    [ObservableProperty]
    private bool _isLoading = false;
    [ObservableProperty]
    private bool _isInitialized = false;
    [ObservableProperty]
    private DateTime? _filterStartDate;
    [ObservableProperty]
    private DateTime? _filterEndDate;
    [ObservableProperty]
    private bool _isDateFilterActive = false;
    [ObservableProperty]
    private string _dateFilterSummary = string.Empty;
    [ObservableProperty]
    private ObservableCollection<string> _statusFilterOptions = new() { "All Status", "Completed", "Pending" };
    private string _selectedStatusFilter = "All Status";
    [ObservableProperty]
    private ObservableCollection<string> _typeFilterOptions = new() { "All Types", "buy", "sell" };
    private string _selectedTypeFilter = "All Types";
    [ObservableProperty]
    private bool _isStatusFilterActive = false;
    [ObservableProperty]
    private bool _isTypeFilterActive = false;
    [ObservableProperty]
    private ObservableCollection<string> _sortFilterOptions = new() { "Newest First", "Oldest First" };
    private string _selectedSortFilter = "Newest First";
    [ObservableProperty]
    private bool _isSortFilterActive = false;
    public ObservableCollection<Transaction> Transactions
    {
        get => _transactions;
        set
        {
            if (_transactions != value)
            {
                _transactions = value;
                OnPropertyChanged();
            }
        }
    }
    public string SelectedStatusFilter
    {
        get => _selectedStatusFilter;
        set => _selectedStatusFilter = value;
    }
    public string SelectedTypeFilter
    {
        get => _selectedTypeFilter;
        set => _selectedTypeFilter = value;
    }
    public string SelectedSortFilter
    {
        get => _selectedSortFilter;
        set => _selectedSortFilter = value;
    }
    public bool HasAnyActiveFilter => IsDateFilterActive || IsStatusFilterActive || IsTypeFilterActive || IsSortFilterActive;
    #endregion

    #region Methods
    public async Task LoadData()
    {
        if (IsLoading) return;
        try
        {
            IsLoading = true;
            Debug.WriteLine("LoadData: Starting to load transactions...");
            if (App.TransactionRepo == null)
            {
                Debug.WriteLine("LoadData: TransactionRepo is null!");
                return;
            }
            var DBTransactions = App.TransactionRepo.GetItemsWithChildren();
            Debug.WriteLine($"LoadData: Retrieved {DBTransactions?.Count ?? 0} transactions from database");
            if (DBTransactions == null || !DBTransactions.Any())
            {
                Debug.WriteLine("LoadData: No transactions found in database");
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    AllTransactions.Clear();
                    Transactions.Clear();
                    TransactionCount = "No transactions found";
                    IsInitialized = true;
                });
                return;
            }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                try
                {
                    Debug.WriteLine($"LoadData: Loading {DBTransactions.Count} transactions into UI...");
                    AllTransactions.Clear();
                    Transactions.Clear();
                    foreach (var item in DBTransactions)
                    {
                        if (item == null) continue;
                        if (item.Lines == null) item.Lines = new List<TransactionLine>();
                        AllTransactions.Add(item);
                        Transactions.Add(item);
                    }
                    Debug.WriteLine($"LoadData: Successfully loaded {AllTransactions.Count} transactions");
                    IsDateFilterActive = false;
                    FilterStartDate = null;
                    FilterEndDate = null;
                    DateFilterSummary = string.Empty;
                    IsStatusFilterActive = false;
                    SelectedStatusFilter = "All Status";
                    IsTypeFilterActive = false;
                    SelectedTypeFilter = "All Types";
                    IsSortFilterActive = false;
                    SelectedSortFilter = "Newest First";
                    TransactionCount = $"Showing {AllTransactions.Count} transactions";
                    IsInitialized = true;
                    Debug.WriteLine($"LoadData: Final transaction count: {AllTransactions.Count}");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"LoadData UI error: {ex.Message}");
                }
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadData error: {ex}");
        }
        finally
        {
            IsLoading = false;
        }
    }
    private void OnToggleExpand(object parameter)
    {
        if (parameter is Transaction transaction)
        {
            transaction.IsExpanded = !transaction.IsExpanded;
            UpdateAnyExpandedState();
        }
    }
    private void OnCollapseAll()
    {
        foreach (var transaction in AllTransactions)
        {
            transaction.IsExpanded = false;
        }
        UpdateAnyExpandedState();
    }
    private void UpdateAnyExpandedState()
    {
        AnyExpanded = AllTransactions.Any(t => t.IsExpanded);
    }
    public async Task<bool> ApplyDateFilter(DateTime startDate, DateTime endDate)
    {
        try
        {
            var startDateOnly = startDate.Date;
            var endDateOnly = endDate.Date.AddDays(1).AddTicks(-1);
            var transactionsInRange = AllTransactions.Where(t => t.Date >= startDateOnly && t.Date <= endDateOnly).ToList();
            if (!transactionsInRange.Any())
            {
                return false;
            }
            FilterStartDate = startDate;
            FilterEndDate = endDate;
            IsDateFilterActive = true;
            if (startDate.Date == endDate.Date)
            {
                DateFilterSummary = $"Showing transactions for {startDate:MMM dd, yyyy}";
            }
            else
            {
                DateFilterSummary = $"Showing transactions from {startDate:MMM dd} to {endDate:MMM dd, yyyy}";
            }
            var filteredTransactions = AllTransactions.Where(t => t.Date >= startDateOnly && t.Date <= endDateOnly).ToList();
            Transactions = new ObservableCollection<Transaction>(filteredTransactions);
            TransactionCount = $"Showing {Transactions.Count} transactions";
            string successMessage;
            if (startDate.Date == endDate.Date)
            {
                successMessage = $"Found {Transactions.Count} transactions for {startDate:MMM dd, yyyy}";
            }
            else
            {
                successMessage = $"Found {Transactions.Count} transactions from {startDate:MMM dd} to {endDate:MMM dd, yyyy}";
            }
            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.Green,
                TextColor = Colors.White
            };
            await Snackbar.Make(successMessage, null, "ok", TimeSpan.FromSeconds(3), snackbarOptions).Show();
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    public void ClearDateFilter()
    {
        FilterStartDate = null;
        FilterEndDate = null;
        IsDateFilterActive = false;
        DateFilterSummary = string.Empty;
        Transactions = new ObservableCollection<Transaction>(AllTransactions);
        TransactionCount = $"Showing {Transactions.Count} transactions";
    }
    public void OnStatusFilterChanged()
    {
        Debug.WriteLine($"OnStatusFilterChanged: {SelectedStatusFilter}");
        var allTransactions = AllTransactions.ToList();
        Debug.WriteLine($"Total transactions: {allTransactions.Count}");
        List<Transaction> filteredList = new List<Transaction>();
        if (SelectedStatusFilter == "Completed")
        {
            filteredList = allTransactions.Where(t => t.IsPaid).ToList();
            Debug.WriteLine($"Completed transactions: {filteredList.Count}");
            IsStatusFilterActive = true;
        }
        else if (SelectedStatusFilter == "Pending")
        {
            filteredList = allTransactions.Where(t => !t.IsPaid).ToList();
            Debug.WriteLine($"Pending transactions: {filteredList.Count}");
            IsStatusFilterActive = true;
        }
        else
        {
            filteredList = allTransactions;
            Debug.WriteLine($"All transactions: {filteredList.Count}");
            IsStatusFilterActive = false;
        }
        try
        {
            var newCollection = new ObservableCollection<Transaction>();
            foreach (var transaction in filteredList)
            {
                newCollection.Add(transaction);
            }
            Transactions = newCollection;
            TransactionCount = $"Showing {Transactions.Count} transactions";
            Debug.WriteLine($"UI updated successfully with {Transactions.Count} transactions");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating UI: {ex.Message}");
        }
    }
    public void OnTypeFilterChanged()
    {
        Debug.WriteLine($"OnTypeFilterChanged: {SelectedTypeFilter}");
        var allTransactions = AllTransactions.ToList();
        Debug.WriteLine($"Total transactions: {allTransactions.Count}");
        List<Transaction> filteredList = new List<Transaction>();
        if (SelectedTypeFilter == "buy")
        {
            filteredList = allTransactions.Where(t => t.TransactionType == "buy").ToList();
            Debug.WriteLine($"Buy transactions: {filteredList.Count}");
            IsTypeFilterActive = true;
        }
        else if (SelectedTypeFilter == "sell")
        {
            filteredList = allTransactions.Where(t => t.TransactionType == "sell").ToList();
            Debug.WriteLine($"Sell transactions: {filteredList.Count}");
            IsTypeFilterActive = true;
        }
        else
        {
            filteredList = allTransactions;
            Debug.WriteLine($"All transactions: {filteredList.Count}");
            IsTypeFilterActive = false;
        }
        try
        {
            var newCollection = new ObservableCollection<Transaction>();
            foreach (var transaction in filteredList)
            {
                newCollection.Add(transaction);
            }
            Transactions = newCollection;
            TransactionCount = $"Showing {Transactions.Count} transactions";
            Debug.WriteLine($"UI updated successfully with {Transactions.Count} transactions");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating UI: {ex.Message}");
        }
    }
    public void OnSortFilterChanged()
    {
        Debug.WriteLine($"OnSortFilterChanged: {SelectedSortFilter}");
        var allTransactions = AllTransactions.ToList();
        Debug.WriteLine($"Total transactions: {allTransactions.Count}");
        List<Transaction> sortedList = new List<Transaction>();
        if (SelectedSortFilter == "Newest First")
        {
            sortedList = allTransactions.OrderByDescending(t => t.Date).ToList();
            Debug.WriteLine($"Newest first sorted: {sortedList.Count}");
            IsSortFilterActive = true;
        }
        else if (SelectedSortFilter == "Oldest First")
        {
            sortedList = allTransactions.OrderBy(t => t.Date).ToList();
            Debug.WriteLine($"Oldest first sorted: {sortedList.Count}");
            IsSortFilterActive = true;
        }
        else
        {
            sortedList = allTransactions;
            Debug.WriteLine($"No sorting applied: {sortedList.Count}");
            IsSortFilterActive = false;
        }
        try
        {
            var newCollection = new ObservableCollection<Transaction>();
            foreach (var transaction in sortedList)
            {
                newCollection.Add(transaction);
            }
            Transactions = newCollection;
            TransactionCount = $"Showing {Transactions.Count} transactions";
            Debug.WriteLine($"UI updated successfully with {Transactions.Count} transactions");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error updating UI: {ex.Message}");
        }
    }
    public void ClearAllFilters()
    {
        Debug.WriteLine("ClearAllFilters: Starting");
        IsDateFilterActive = false;
        FilterStartDate = null;
        FilterEndDate = null;
        DateFilterSummary = string.Empty;
        IsStatusFilterActive = false;
        SelectedStatusFilter = "All Status";
        IsTypeFilterActive = false;
        SelectedTypeFilter = "All Types";
        IsSortFilterActive = false;
        SelectedSortFilter = "Newest First";
        Transactions = new ObservableCollection<Transaction>(AllTransactions);
        TransactionCount = $"Showing {Transactions.Count} transactions";
        Debug.WriteLine($"ClearAllFilters: Done, showing {Transactions.Count} transactions");
    }
    public async Task RefreshAsync()
    {
        if (IsRefreshing)
        {
            return;
        }
        try
        {
            IsRefreshing = true;
            await Task.Delay(1000);
            _ = LoadData();
            await Task.Delay(1000);
        }
        finally
        {
            IsRefreshing = false;
        }
    }
    public void TestFilter()
    {
        Debug.WriteLine("TestFilter: Starting test");
        try
        {
            Debug.WriteLine($"TestFilter: AllTransactions count: {AllTransactions?.Count ?? 0}");
            if (AllTransactions == null)
            {
                Debug.WriteLine("TestFilter: AllTransactions is null!");
                return;
            }
            Debug.WriteLine("TestFilter: Starting iteration test");
            int count = 0;
            foreach (var transaction in AllTransactions)
            {
                count++;
                if (count > 5) break;
            }
            Debug.WriteLine($"TestFilter: Iterated through {count} transactions successfully");
            Debug.WriteLine("TestFilter: Creating empty collection");
            var emptyCollection = new ObservableCollection<Transaction>();
            Debug.WriteLine("TestFilter: Empty collection created successfully");
            Debug.WriteLine("TestFilter: Assigning empty collection to UI");
            Transactions = emptyCollection;
            Debug.WriteLine("TestFilter: Empty collection assigned successfully");
            TransactionCount = "Test completed";
            Debug.WriteLine("TestFilter: Count updated successfully");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"TestFilter error: {ex.Message}");
            Debug.WriteLine($"TestFilter stack trace: {ex.StackTrace}");
        }
    }
    #endregion

    #region Commands
    public ICommand ToggleExpandCommand => new Command<object>(OnToggleExpand);
    public ICommand CollapseAllCommand => new Command(OnCollapseAll);
    public ICommand RefreshCommand => new Command(async () => await RefreshAsync());
    public ICommand ClearAllFiltersCommand => new Command(ClearAllFilters);
    public ICommand ShowTransactionDetailsCommand => new Command<Transaction>(OnShowTransactionDetails);
    #endregion
    private async void OnShowTransactionDetails(Transaction transaction)
    {
        try
        {
            if (transaction != null)
            {
                var popup = new TransactionDetailsPopup();
                popup.BindingContext = transaction;
                await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ShowTransactionDetails error: {ex.Message}");
        }
    }
}