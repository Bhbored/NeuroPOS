using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using NeuroPOS.MVVM.Model;
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
using Microsoft.Maui.Devices;

namespace NeuroPOS.MVVM.ViewModel;

[AddINotifyPropertyChangedInterface]
public partial class TransactionVM : ObservableObject
{
  
    public TransactionVM()
    {


    }

    #region Properties
    public ObservableCollection<Transaction> Transactions { get; } = new();

    [ObservableProperty]
    private ObservableCollection<Transaction> _allTransactions = new();

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
        // Prevent multiple simultaneous loads
        if (IsLoading)
        {
            return;
        }

        try
        {
            IsLoading = true;

            // Always clear and reload for debugging
            AllTransactions.Clear();
            Transactions.Clear();

            var testData = GenerateTestData(57);

            foreach (var item in testData)
            {
                AllTransactions.Add(item);
            }


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

        if (AllTransactions == null || pageNumber < 1 || pageNumber > TotalPages)
        {
            return;
        }

        try
        {
            var skipCount = (pageNumber - 1) * PageSize;
            var takeCount = Math.Min(PageSize, AllTransactions.Count - skipCount);


            var items = AllTransactions
                .Skip(skipCount)
                .Take(takeCount)
                .ToList();
            // Always update on main thread
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

            // Clear collection safely
            try
            {
                Transactions.Clear();
            }
            catch (Exception clearEx)
            {
                Debug.WriteLine($"UpdatePageUI: Error clearing collection: {clearEx.Message}");
            }

            // Add a small delay to let the UI settle
            System.Threading.Thread.Sleep(50);


            // Add items with exception handling for each item
            int addedCount = 0;
            foreach (var item in items)
            {
                try
                {
                    Transactions.Add(item);
                    addedCount++;

                    // Small delay between items to avoid COM issues
                    if (addedCount < items.Count)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                }
                catch (Exception addEx)
                {
                    Debug.WriteLine($"UpdatePageUI: Error adding transaction #{item.Id}: {addEx.Message}");
                    // Continue with next item
                }
            }


            // Update pagination properties with explicit logging
            var newCanGoPrevious = pageNumber > 1;
            var newCanGoNext = pageNumber < TotalPages;


            try
            {
                CanGoPrevious = newCanGoPrevious;
                CanGoNext = newCanGoNext;

                // Update pagination info
                var startItem = skipCount + 1;
                var endItem = skipCount + takeCount;
                PaginationInfo = $"Showing {startItem} to {endItem} of {AllTransactions.Count} results";


                // Force all property notifications with delays
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

            // Call debug state after everything settles
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

        // Rate limiting
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

        // Rate limiting
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

    #endregion

    #region Test Data

    private ObservableCollection<Transaction> GenerateTestData(int count)
    {

        var transactions = new ObservableCollection<Transaction>();

        for (int i = 1; i <= count; i++)
        {
            var transaction = new Transaction
            {
                Id = 10000 + i,
                Date = DateTime.Now.AddDays(-i),
                TransactionType = i % 2 == 0 ? "sell" : "buy",
                TotalAmount = i * 5.5,
                ItemCount = i % 5 + 1,
                IsPaid = i % 3 != 0,
                IsExpanded = false,
                TransactionItems = new List<Product>
                {
                    new Product
                    {
                        Name = $"Product {i}",
                        Price = i * 1.1,
                        Stock = i % 10,
                        CategoryName = i % 2 == 0 ? "Fruits" : "Dairy",
                        DateAdded = DateTime.Now.AddDays(-i * 2)
                    }
                }
            };

            transactions.Add(transaction);
 
        }

        return transactions;
    }

    #endregion
}