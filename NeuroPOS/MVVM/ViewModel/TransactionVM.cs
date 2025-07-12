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
    private const int ButtonCooldownMs = 100; // Very short cooldown to prevent rapid-fire only

    public TransactionVM()
    {

        
    }

    #region Debug Methods

   

  

    public void UpdateButtonStates()
    {
       
        var newCanGoPrevious = CurrentPage > 1;
        var newCanGoNext = CurrentPage < TotalPages;

        CanGoPrevious = newCanGoPrevious;
        CanGoNext = newCanGoNext;

        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
    }

    #endregion

    #region Pull-to-Refresh

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

    #region Pagination Logic

    public void LoadData()
    {
        // Prevent multiple simultaneous loads
        if (IsLoading)
        {
            Debug.WriteLine("LoadData: Already loading, skipping...");
            return;
        }

        try
        {
            IsLoading = true;

            // Always clear and reload for debugging
            AllTransactions.Clear();
            Transactions.Clear();

            var testData = GenerateTestData(57);
            Debug.WriteLine($"LoadData: Generated {testData.Count} test items");

            foreach (var item in testData)
            {
                AllTransactions.Add(item);
            }

            Debug.WriteLine($"LoadData: Added {AllTransactions.Count} items to AllTransactions");

            TotalPages = (int)Math.Ceiling((double)AllTransactions.Count / PageSize);
            CurrentPage = 1;

            Debug.WriteLine($"LoadData: TotalPages calculated as {TotalPages}");

            LoadPage(CurrentPage);

            IsInitialized = true;
            Debug.WriteLine($"LoadData: Completed. AllTransactions: {AllTransactions.Count}, TotalPages: {TotalPages}");
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
        Debug.WriteLine($"LoadPage: Starting with pageNumber={pageNumber}");

        if (AllTransactions == null || pageNumber < 1 || pageNumber > TotalPages)
        {
            Debug.WriteLine($"LoadPage: Invalid conditions. AllTransactions null: {AllTransactions == null}, pageNumber: {pageNumber}, TotalPages: {TotalPages}");
            return;
        }

        try
        {
            var skipCount = (pageNumber - 1) * PageSize;
            var takeCount = Math.Min(PageSize, AllTransactions.Count - skipCount);

            Debug.WriteLine($"LoadPage: skipCount={skipCount}, takeCount={takeCount}");

            var items = AllTransactions
                .Skip(skipCount)
                .Take(takeCount)
                .ToList();

            Debug.WriteLine($"LoadPage: Retrieved {items.Count} items from AllTransactions");

            // Log the actual IDs being loaded
            foreach (var item in items)
            {
                Debug.WriteLine($"LoadPage: Item #{item.Id} prepared for display");
            }

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
        Debug.WriteLine($"UpdatePageUI: Starting with {items.Count} items, pageNumber={pageNumber}");
        Debug.WriteLine($"UpdatePageUI: Thread ID: {System.Threading.Thread.CurrentThread.ManagedThreadId}");

        try
        {
            Debug.WriteLine($"UpdatePageUI: Clearing {Transactions.Count} existing transactions");

            // Clear collection safely
            try
            {
                Transactions.Clear();
                Debug.WriteLine($"UpdatePageUI: Transactions cleared, count now: {Transactions.Count}");
            }
            catch (Exception clearEx)
            {
                Debug.WriteLine($"UpdatePageUI: Error clearing collection: {clearEx.Message}");
            }

            // Add a small delay to let the UI settle
            System.Threading.Thread.Sleep(50);

            Debug.WriteLine($"UpdatePageUI: Adding {items.Count} new transactions");

            // Add items with exception handling for each item
            int addedCount = 0;
            foreach (var item in items)
            {
                try
                {
                    Transactions.Add(item);
                    addedCount++;
                    Debug.WriteLine($"UpdatePageUI: Added transaction #{item.Id} ({addedCount}/{items.Count}), Collection count: {Transactions.Count}");

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

            Debug.WriteLine($"UpdatePageUI: Final Transactions.Count: {Transactions.Count}");

            // Update pagination properties with explicit logging
            var newCanGoPrevious = pageNumber > 1;
            var newCanGoNext = pageNumber < TotalPages;

            Debug.WriteLine($"UpdatePageUI: Setting CanGoPrevious from {CanGoPrevious} to {newCanGoPrevious}");
            Debug.WriteLine($"UpdatePageUI: Setting CanGoNext from {CanGoNext} to {newCanGoNext}");

            try
            {
                CanGoPrevious = newCanGoPrevious;
                CanGoNext = newCanGoNext;

                // Update pagination info
                var startItem = skipCount + 1;
                var endItem = skipCount + takeCount;
                PaginationInfo = $"Showing {startItem} to {endItem} of {AllTransactions.Count} results";

                Debug.WriteLine($"UpdatePageUI: Final state - Transactions.Count: {Transactions.Count}");
                Debug.WriteLine($"UpdatePageUI: CanGoPrevious: {CanGoPrevious}, CanGoNext: {CanGoNext}");
                Debug.WriteLine($"UpdatePageUI: PaginationInfo: '{PaginationInfo}'");

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

                Debug.WriteLine("UpdatePageUI: Property change notifications sent");
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
        Debug.WriteLine($"NextPage: Called. CurrentPage: {CurrentPage}, TotalPages: {TotalPages}, CanGoNext: {CanGoNext}");

        // Rate limiting
        var now = DateTime.Now;
        if ((now - _lastButtonClick).TotalMilliseconds < ButtonCooldownMs)
        {
            Debug.WriteLine($"NextPage: Rate limited. Last click was {(now - _lastButtonClick).TotalMilliseconds}ms ago");
            return;
        }
        _lastButtonClick = now;

        if (IsLoading)
        {
            Debug.WriteLine("NextPage: Currently loading, ignoring click");
            return;
        }

        if (CurrentPage < TotalPages && CanGoNext)
        {
            CurrentPage++;
            Debug.WriteLine($"NextPage: Moving to page {CurrentPage}");
            LoadPage(CurrentPage);
        }
        else
        {
            Debug.WriteLine($"NextPage: Cannot go next. CurrentPage: {CurrentPage}, TotalPages: {TotalPages}, CanGoNext: {CanGoNext}");
        }
    }

    public void PreviousPage()
    {
        Debug.WriteLine($"PreviousPage: Called. CurrentPage: {CurrentPage}, CanGoPrevious: {CanGoPrevious}");

        // Rate limiting
        var now = DateTime.Now;
        if ((now - _lastButtonClick).TotalMilliseconds < ButtonCooldownMs)
        {
            Debug.WriteLine($"PreviousPage: Rate limited. Last click was {(now - _lastButtonClick).TotalMilliseconds}ms ago");
            return;
        }
        _lastButtonClick = now;

        if (IsLoading)
        {
            Debug.WriteLine("PreviousPage: Currently loading, ignoring click");
            return;
        }

        if (CurrentPage > 1 && CanGoPrevious)
        {
            CurrentPage--;
            Debug.WriteLine($"PreviousPage: Moving to page {CurrentPage}");
            LoadPage(CurrentPage);
        }
        else
        {
            Debug.WriteLine($"PreviousPage: Cannot go previous. CurrentPage: {CurrentPage}, CanGoPrevious: {CanGoPrevious}");
        }
    }

    #endregion

    #region Expand/Collapse Logic

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
        Debug.WriteLine($"GenerateTestData: Creating {count} transactions");

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

            // Debug first and last few transactions
            if (i <= 5 || i > count - 5)
            {
                Debug.WriteLine($"GenerateTestData: Created transaction {i}: #{transaction.Id}");
            }
        }

        Debug.WriteLine($"GenerateTestData: Created {transactions.Count} transactions");
        Debug.WriteLine($"GenerateTestData: First transaction ID: #{transactions[0].Id}");
        Debug.WriteLine($"GenerateTestData: Last transaction ID: #{transactions[transactions.Count - 1].Id}");
        return transactions;
    }

    #endregion
}