using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.MVVM.Controls;
using NeuroPOS.MVVM.Model;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace NeuroPOS.MVVM.Popups;

public partial class BuyingTransactionPopup : Popup
{
    private InventoryVM _inventoryVM;
    private List<BuyingProductEntry> _productEntries = new List<BuyingProductEntry>();
    private double _totalTransactionAmount = 0;

    public BuyingTransactionPopup(InventoryVM inventoryVM)
    {
        InitializeComponent();
        _inventoryVM = inventoryVM;
        BindingContext = _inventoryVM;

        // Add the first product entry
        AddProductEntry();
    }

    private void AddProductEntry()
    {
        var productEntry = new BuyingProductEntry
        {
            Products = _inventoryVM.Products,
            Index = _productEntries.Count,
            IsRemovable = _productEntries.Count > 0 // First entry is not removable
        };

        // Subscribe to events
        productEntry.ProductEntryChanged += OnProductEntryChanged;
        productEntry.RemoveRequested += OnProductEntryRemoveRequested;

        _productEntries.Add(productEntry);
        ProductEntriesContainer.Children.Add(productEntry);

        UpdateTotalAmount();
    }

    private void OnProductEntryChanged(object sender, EventArgs e)
    {
        UpdateTotalAmount();
    }

    private void OnProductEntryRemoveRequested(object sender, EventArgs e)
    {
        if (sender is BuyingProductEntry productEntry && _productEntries.Count > 1)
        {
            _productEntries.Remove(productEntry);
            ProductEntriesContainer.Children.Remove(productEntry);

            // Unsubscribe from events
            productEntry.ProductEntryChanged -= OnProductEntryChanged;
            productEntry.RemoveRequested -= OnProductEntryRemoveRequested;

            UpdateTotalAmount();
            UpdateRemovability();
        }
    }

    private void UpdateRemovability()
    {
        // Update the IsRemovable property for all entries
        for (int i = 0; i < _productEntries.Count; i++)
        {
            _productEntries[i].IsRemovable = i > 0; // First entry is never removable
        }
    }

    private void UpdateTotalAmount()
    {
        _totalTransactionAmount = _productEntries.Sum(entry => entry.TotalCost);
        TotalAmountLabel.Text = $"${_totalTransactionAmount:F2}";
    }

    private void OnAddMoreProductClicked(object sender, EventArgs e)
    {
        AddProductEntry();
    }

    private async void OnAddTransactionClicked(object sender, EventArgs e)
    {
        // Validate the transaction
        if (!ValidateTransaction())
            return;

        try
        {
            // Create the transaction
            var transaction = CreateTransaction();

            // Add to the buying transactions collection
            App.TransactionRepo.InsertItemWithChildren(transaction);
            UpdateProductStocks();
            // Show success snackbar
            var snackbar = Snackbar.Make($"Buying transaction added successfully (${_totalTransactionAmount:F2})",
                duration: TimeSpan.FromSeconds(4));
            await snackbar.Show();

            await CloseAsync();
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", $"Failed to add transaction: {ex.Message}", "OK");
        }
    }

    private bool ValidateTransaction()
    {
        // Check if there's at least one valid product entry
        var validEntries = _productEntries.Where(entry => entry.IsValid()).ToList();

        if (validEntries.Count == 0)
        {
            App.Current.MainPage.DisplayAlert("Validation Error", "Please select at least one product with a valid quantity.", "OK");
            return false;
        }

        // Check for duplicate products
        var productIds = validEntries.Select(entry => entry.SelectedProduct.Id).ToList();
        if (productIds.Count != productIds.Distinct().Count())
        {
            App.Current.MainPage.DisplayAlert("Validation Error", "Cannot select the same product multiple times. Please remove duplicates.", "OK");
            return false;
        }

        // Check if total amount is greater than 0
        if (_totalTransactionAmount <= 0)
        {
            App.Current.MainPage.DisplayAlert("Validation Error", "Transaction total must be greater than $0.00.", "OK");
            return false;
        }

        return true;
    }

    private Transaction CreateTransaction()
    {
        // Keep only rows that passed validation in the UI
        var validEntries = _productEntries.Where(e => e.IsValid()).ToList();

        // Parent Transaction -----------------------------------------------
        var transaction = new Transaction
        {
            Id = _inventoryVM.BuyingTransaction.Count > 0
                ? _inventoryVM.BuyingTransaction.Max(t => t.Id) + 1
                : 1,

            Date = DateTime.Now,
            TransactionType = "buy",
            IsPaid = true,

            Lines = new List<TransactionLine>(),
            ItemCount = validEntries.Sum(e => e.Quantity) // total units
        };

        // Build lines and calculate total cost -----------------------------
        double total = 0.0; // Cost * Qty for a BUY

        foreach (var entry in validEntries)
        {
            var p = entry.SelectedProduct; // DB-tracked Product instance

            var line = new TransactionLine
            {
                // Snapshot of current product data
                Name = p.Name,
                Cost = p.Cost,
                Price = p.Price,             // keep if you show MSRP
                Stock = entry.Quantity,      // quantity bought
                CategoryName = p.CategoryName,
                ImageUrl = p.ImageUrl,

                // Foreign-key link
                Product = p,
                ProductId = p.Id
            };

            transaction.Lines.Add(line);
            total += p.Cost * entry.Quantity;
        }

        transaction.TotalAmount = total;

        return transaction;
    }


    private void UpdateProductStocks()
    {
        var validEntries = _productEntries.Where(entry => entry.IsValid()).ToList();

        foreach (var entry in validEntries)
        {
            // Find the actual product in the inventory and update its stock
            var product = _inventoryVM.Products.FirstOrDefault(p => p.Id == entry.SelectedProduct.Id);
            if (product != null)
            {
                product.Stock += entry.Quantity; 
            }
            App.ProductRepo.UpdateItem(product);
        }

        _=_inventoryVM.RefreshDBAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        // Clear all entries
        foreach (var entry in _productEntries)
        {
            entry.Reset();
        }

        await CloseAsync();
    }
}