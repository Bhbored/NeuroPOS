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
    #region Properties
    public ObservableCollection<Transaction> Transactions { get; } = new();
    [ObservableProperty]
    private ObservableCollection<Transaction> _allTransactions = new();
    [ObservableProperty]
    private ObservableCollection<Transaction> _filteredTransactions = new();
    private const int PageSize = 10;
    [ObservableProperty]
    private bool _anyExpanded = false;
    [ObservableProperty]
    private int _currentPage = 1;
    [ObservableProperty]
    private int _totalPages;
    [ObservableProperty]
    private bool _canGoNext;
    [ObservableProperty]
    private bool _canGoPrevious;
    [ObservableProperty]
    private string _paginationInfo = "";
    [ObservableProperty]
    private bool _isRefreshing = false;
    [ObservableProperty]
    private bool _isLoading = false;
    [ObservableProperty]
    private bool _isInitialized = false;
    private DateTime _lastButtonClick = DateTime.MinValue;
    private const int ButtonCooldownMs = 100;
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
    [ObservableProperty]
    private string _selectedStatusFilter = "All Status";
    [ObservableProperty]
    private ObservableCollection<string> _typeFilterOptions = new() { "All Types", "buy", "sell" };
    [ObservableProperty]
    private string _selectedTypeFilter = "All Types";
    [ObservableProperty]
    private bool _isStatusFilterActive = false;
    [ObservableProperty]
    private bool _isTypeFilterActive = false;
    public bool HasAnyActiveFilter => IsDateFilterActive || IsStatusFilterActive || IsTypeFilterActive;
    #endregion

    #region Methods
    public void UpdateButtonStates()
    {
        var newCanGoPrevious = CurrentPage > 1;
        var newCanGoNext = CurrentPage < TotalPages;
        CanGoPrevious = newCanGoPrevious;
        CanGoNext = newCanGoNext;
        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
    }

    public void LoadData()
    {
        if (IsLoading)
        {
            return;
        }
        try
        {
            IsLoading = true;
            AllTransactions.Clear();
            FilteredTransactions.Clear();
            Transactions.Clear();
            var DBTransactions = App.TransactionRepo.GetItemsWithChildren();
            foreach (var item in DBTransactions)
            {
                AllTransactions.Add(item);
            }
            IsDateFilterActive = false;
            FilterStartDate = null;
            FilterEndDate = null;
            DateFilterSummary = string.Empty;
            IsStatusFilterActive = false;
            SelectedStatusFilter = "All Status";
            IsTypeFilterActive = false;
            SelectedTypeFilter = "All Types";
            TotalPages = (int)Math.Ceiling((double)AllTransactions.Count / PageSize);
            CurrentPage = 1;
            LoadPage(CurrentPage);
            IsInitialized = true;
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

    private void LoadPage(int pageNumber)
    {
        var activeSource = GetActiveTransactionSource();
        if (activeSource == null || pageNumber < 1 || pageNumber > TotalPages)
        {
            return;
        }
        try
        {
            var skipCount = (pageNumber - 1) * PageSize;
            var takeCount = Math.Min(PageSize, activeSource.Count - skipCount);
            var items = activeSource
                .Skip(skipCount)
                .Take(takeCount)
                .ToList();
            MainThread.BeginInvokeOnMainThread(() => UpdatePageUI(items, pageNumber, skipCount, takeCount));
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"LoadPage error: {ex}");
        }
    }

    private void UpdatePageUI(List<Transaction> items, int pageNumber, int skipCount, int takeCount)
    {
        try
        {
            try
            {
                Transactions.Clear();
            }
            catch (Exception clearEx)
            {
                Debug.WriteLine($"UpdatePageUI: Error clearing collection: {clearEx.Message}");
            }
            System.Threading.Thread.Sleep(50);
            int addedCount = 0;
            foreach (var item in items)
            {
                try
                {
                    Transactions.Add(item);
                    addedCount++;
                    if (addedCount < items.Count)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
                catch (Exception addEx)
                {
                    Debug.WriteLine($"UpdatePageUI: Error adding transaction #{item.Id}: {addEx.Message}");
                }
            }
            var newCanGoPrevious = pageNumber > 1;
            var newCanGoNext = pageNumber < TotalPages;
            try
            {
                CanGoPrevious = newCanGoPrevious;
                CanGoNext = newCanGoNext;
                var startItem = skipCount + 1;
                var endItem = skipCount + takeCount;
                var totalCount = GetActiveTransactionSource().Count;
                PaginationInfo = $"Showing {startItem} to {endItem} of {totalCount} results";
                OnPropertyChanged(nameof(Transactions));
                System.Threading.Thread.Sleep(10);
                OnPropertyChanged(nameof(PaginationInfo));
                System.Threading.Thread.Sleep(10);
                OnPropertyChanged(nameof(CanGoNext));
                System.Threading.Thread.Sleep(10);
                OnPropertyChanged(nameof(CanGoPrevious));
                System.Threading.Thread.Sleep(10);
                OnPropertyChanged(nameof(CurrentPage));
            }
            catch (Exception propEx)
            {
                Debug.WriteLine($"UpdatePageUI: Error updating properties: {propEx.Message}");
            }
            System.Threading.Thread.Sleep(100);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"UpdatePageUI error: {ex.Message}");
            Debug.WriteLine($"UpdatePageUI full error: {ex}");
        }
    }

    public void NextPage()
    {
        var now = DateTime.Now;
        if ((now - _lastButtonClick).TotalMilliseconds < ButtonCooldownMs)
        {
            return;
        }
        _lastButtonClick = now;
        if (IsLoading)
        {
            return;
        }
        if (CurrentPage < TotalPages && CanGoNext)
        {
            CurrentPage++;
            LoadPage(CurrentPage);
        }
    }

    public void PreviousPage()
    {
        var now = DateTime.Now;
        if ((now - _lastButtonClick).TotalMilliseconds < ButtonCooldownMs)
        {
            return;
        }
        _lastButtonClick = now;
        if (IsLoading)
        {
            return;
        }
        if (CurrentPage > 1 && CanGoPrevious)
        {
            CurrentPage--;
            LoadPage(CurrentPage);
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
        foreach (var transaction in Transactions)
        {
            transaction.IsExpanded = false;
        }
        UpdateAnyExpandedState();
    }

    private void UpdateAnyExpandedState()
    {
        AnyExpanded = Transactions.Any(t => t.IsExpanded);
    }

    public async Task ApplyDateFilter(DateTime startDate, DateTime endDate)
    {
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
        OnPropertyChanged(nameof(HasAnyActiveFilter));
        ApplyAllFilters();
        await Task.Delay(500);
    }

    public void ClearDateFilter()
    {
        FilterStartDate = null;
        FilterEndDate = null;
        IsDateFilterActive = false;
        DateFilterSummary = string.Empty;
        ApplyAllFilters();
    }

    partial void OnSelectedStatusFilterChanged(string value)
    {
        ApplyStatusFilter(value);
    }

    partial void OnSelectedTypeFilterChanged(string value)
    {
        ApplyTypeFilter(value);
    }

    public void ApplyStatusFilter(string selectedStatus)
    {
        SelectedStatusFilter = selectedStatus;
        IsStatusFilterActive = selectedStatus != "All Status";
        OnPropertyChanged(nameof(HasAnyActiveFilter));
        ApplyAllFilters();
    }

    public void ApplyTypeFilter(string selectedType)
    {
        SelectedTypeFilter = selectedType;
        IsTypeFilterActive = selectedType != "All Types";
        OnPropertyChanged(nameof(HasAnyActiveFilter));
        ApplyAllFilters();
    }

    private void ApplyAllFilters()
    {
        try
        {
            IsLoading = true;
            FilteredTransactions.Clear();
            var sourceTransactions = AllTransactions.AsEnumerable();
            if (IsDateFilterActive && FilterStartDate.HasValue && FilterEndDate.HasValue)
            {
                var startDate = FilterStartDate.Value.Date;
                var endDate = FilterEndDate.Value.Date.AddDays(1).AddTicks(-1);
                sourceTransactions = sourceTransactions.Where(t => t.Date >= startDate && t.Date <= endDate);
            }
            if (IsStatusFilterActive)
            {
                sourceTransactions = sourceTransactions.Where(t =>
                    SelectedStatusFilter == "Completed" ? t.IsPaid : !t.IsPaid);
            }
            if (IsTypeFilterActive)
            {
                sourceTransactions = sourceTransactions.Where(t =>
                    string.Equals(t.TransactionType, SelectedTypeFilter, StringComparison.OrdinalIgnoreCase));
            }
            var filteredList = sourceTransactions
                .OrderByDescending(t => t.Date)
                .ToList();
            foreach (var transaction in filteredList)
            {
                FilteredTransactions.Add(transaction);
            }
            CurrentPage = 1;
            TotalPages = (int)Math.Ceiling((double)GetActiveTransactionSource().Count / PageSize);
            LoadPage(CurrentPage);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void ClearAllFilters()
    {
        try
        {
            IsLoading = true;
            FilterStartDate = null;
            FilterEndDate = null;
            IsDateFilterActive = false;
            DateFilterSummary = string.Empty;
            SelectedStatusFilter = "All Status";
            IsStatusFilterActive = false;
            SelectedTypeFilter = "All Types";
            IsTypeFilterActive = false;
            OnPropertyChanged(nameof(HasAnyActiveFilter));
            FilteredTransactions.Clear();
            CurrentPage = 1;
            TotalPages = (int)Math.Ceiling((double)AllTransactions.Count / PageSize);
            LoadPage(CurrentPage);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private ObservableCollection<Transaction> GetActiveTransactionSource()
    {
        return (IsDateFilterActive || IsStatusFilterActive || IsTypeFilterActive) ? FilteredTransactions : AllTransactions;
    }
    #endregion

    #region Tasks
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
            LoadData();
            await Task.Delay(1000);
        }
        finally
        {
            IsRefreshing = false;
        }
    }
    #endregion

    #region Commands
    public ICommand ToggleExpandCommand => new Command<object>(OnToggleExpand);
    public ICommand CollapseAllCommand => new Command(OnCollapseAll);
    public ICommand NextPageCommand => new Command(NextPage);
    public ICommand PreviousPageCommand => new Command(PreviousPage);
    public ICommand RefreshCommand => new Command(async () => await RefreshAsync());
    public ICommand UpdateButtonsCommand => new Command(UpdateButtonStates);
    public ICommand ClearAllFiltersCommand => new Command(ClearAllFilters);
    #endregion
}