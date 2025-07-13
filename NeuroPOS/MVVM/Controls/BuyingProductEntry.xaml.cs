using NeuroPOS.MVVM.Model;
using System.Collections.ObjectModel;
using Syncfusion.Maui.Inputs;

namespace NeuroPOS.MVVM.Controls;

public partial class BuyingProductEntry : ContentView
{
    public static readonly BindableProperty ProductsProperty = BindableProperty.Create(
        nameof(Products), typeof(ObservableCollection<Product>), typeof(BuyingProductEntry),
        default(ObservableCollection<Product>), propertyChanged: OnProductsChanged);

    public static readonly BindableProperty IndexProperty = BindableProperty.Create(
        nameof(Index), typeof(int), typeof(BuyingProductEntry), 0);

    public static readonly BindableProperty IsRemovableProperty = BindableProperty.Create(
        nameof(IsRemovable), typeof(bool), typeof(BuyingProductEntry), false,
        propertyChanged: OnIsRemovableChanged);

    public ObservableCollection<Product> Products
    {
        get => (ObservableCollection<Product>)GetValue(ProductsProperty);
        set => SetValue(ProductsProperty, value);
    }

    public int Index
    {
        get => (int)GetValue(IndexProperty);
        set => SetValue(IndexProperty, value);
    }

    public bool IsRemovable
    {
        get => (bool)GetValue(IsRemovableProperty);
        set => SetValue(IsRemovableProperty, value);
    }

    public Product SelectedProduct { get; private set; }
    public int Quantity { get; private set; } = 1;
    public double UnitCost { get; private set; } = 0;
    public double TotalCost { get; private set; } = 0;

    public event EventHandler<EventArgs> ProductEntryChanged;
    public event EventHandler<EventArgs> RemoveRequested;

    public BuyingProductEntry()
    {
        InitializeComponent();
        UpdateTotalCost();
    }

    private static void OnProductsChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BuyingProductEntry control && newValue is ObservableCollection<Product> products)
        {
            control.ProductAutocomplete.ItemsSource = products;
        }
    }

    private static void OnIsRemovableChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is BuyingProductEntry control && newValue is bool isRemovable)
        {
            control.RemoveButton.IsVisible = isRemovable;
        }
    }

    private void OnProductSelectionChanged(object sender, EventArgs e)
    {
        if (ProductAutocomplete.SelectedItem is Product selectedProduct)
        {
            SelectedProduct = selectedProduct;
            UnitCost = selectedProduct.Cost;
            UnitCostLabel.Text = $"${UnitCost:F2}";
            UpdateTotalCost();
            ProductEntryChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnQuantityChanged(object sender, TextChangedEventArgs e)
    {
        if (int.TryParse(e.NewTextValue, out int quantity) && quantity >= 0)
        {
            Quantity = Math.Max(1, quantity); // Minimum 1
            QuantityEntry.Text = Quantity.ToString();
        }
        else
        {
            Quantity = 1;
            QuantityEntry.Text = "1";
        }

        UpdateTotalCost();
        ProductEntryChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateTotalCost()
    {
        TotalCost = UnitCost * Quantity;
        TotalCostLabel.Text = $"${TotalCost:F2}";
    }

    private void OnRemoveClicked(object sender, EventArgs e)
    {
        RemoveRequested?.Invoke(this, EventArgs.Empty);
    }

    public bool IsValid()
    {
        return SelectedProduct != null && Quantity > 0;
    }

    public void Reset()
    {
        ProductAutocomplete.SelectedItem = null;
        ProductAutocomplete.Text = string.Empty;
        QuantityEntry.Text = "1";
        UnitCostLabel.Text = "$0.00";
        TotalCostLabel.Text = "$0.00";
        SelectedProduct = null;
        UnitCost = 0;
        Quantity = 1;
        TotalCost = 0;
    }
}