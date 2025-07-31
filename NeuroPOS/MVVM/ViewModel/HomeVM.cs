using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Input;
using NeuroPOS.Data;
using NeuroPOS.DTO;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.Popups;
using NeuroPOS.MVVM.View;
using NeuroPOS.Services;
using PropertyChanged;
using Syncfusion.Maui.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Contact = NeuroPOS.MVVM.Model.Contact;
using ListSortDirection = Syncfusion.Maui.DataSource.ListSortDirection;
namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class HomeVM : INotifyPropertyChanged
    {
        public HomeVM(AssistantClient assistant, InventoryVM inventory, OrderVM order)
        {
            _assistant = assistant;
            _inventory = inventory;
            _order = order;
        }
        #region Enums
        public enum SortDirectionState
        {
            None,
            Ascending,
            Descending
        }
        #endregion

        #region Properties
        public ObservableCollection<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();
        private ObservableCollection<Product> _currentOrderItems = new ObservableCollection<Product>();
        public ObservableCollection<Product> CurrentOrderItems
        {
            get => _currentOrderItems;
            set
            {
                if (_currentOrderItems != null)
                {
                    _currentOrderItems.CollectionChanged -= OnCurrentOrderItemsChanged;
                    foreach (var item in _currentOrderItems)
                    {
                        if (item is INotifyPropertyChanged notifyItem)
                            notifyItem.PropertyChanged -= OnOrderItemPropertyChanged;
                    }
                }
                _currentOrderItems = value;
                if (_currentOrderItems != null)
                {
                    _currentOrderItems.CollectionChanged += OnCurrentOrderItemsChanged;
                    foreach (var item in _currentOrderItems)
                    {
                        if (item is INotifyPropertyChanged notifyItem)
                            notifyItem.PropertyChanged += OnOrderItemPropertyChanged;
                    }
                }
                OnPropertyChanged();
                NotifyCalculatedPropertiesChanged();
            }
        }
        public ObservableCollection<object> SelectedItems { get; set; } = [];
        public IList<object> SelectedProducts { get; set; } = [];
        private ObservableCollection<int> _persistentSelectedIds = new ObservableCollection<int>();
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set
            {
                if (_categories != value)
                {
                    _categories = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public DataSource DataSource { get; set; } = new DataSource()
        {
            Source = new ObservableCollection<Product>()
        };
        private SortDirectionState _sortState = SortDirectionState.None;
        public SortDirectionState SortState
        {
            get => _sortState;
            set
            {
                if (_sortState != value)
                {
                    _sortState = value;
                    ApplySorting();
                }
            }
        }
        public string SortLabel => SortState switch
        {
            SortDirectionState.Ascending => "Sort: A → Z",
            SortDirectionState.Descending => "Sort: Z → A",
            SortDirectionState.None => "Sort: None",
            _ => "Sort"
        };
        public double Subtotal
        {
            get
            {
                var result = CurrentOrderItems?.Sum(item => item.Price * item.Stock) ?? 0;
                return result;
            }
        }
        private double _taxRate = 2.0;
        public double TaxRate
        {
            get => _taxRate;
            set
            {
                if (Math.Abs(_taxRate - value) > 0.01)
                {
                    _taxRate = value;
                    OnPropertyChanged();
                    if (CurrentOrderItems?.Count > 0)
                    {
                        NotifyCalculatedPropertiesChanged();
                    }
                }
            }
        }
        public double Tax
        {
            get
            {
                var result = Subtotal * (TaxRate / 100.0);
                return result;
            }
        }
        private double _discount = 0;
        public double Discount
        {
            get => _discount;
            set
            {
                var maxDiscount = Math.Max(0, Subtotal);
                var validatedDiscount = Math.Min(Math.Max(0, value), maxDiscount);
                if (Math.Abs(_discount - validatedDiscount) > 0.01)
                {
                    _discount = validatedDiscount;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Total));
                }
            }
        }
        public double Total
        {
            get
            {
                var result = Subtotal + Tax - Discount;
                return Math.Max(0, result);
            }
        }
        public string OrderDateTime => DateTime.Now.ToString("MMM dd, yyyy - hh:mm tt");
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Methods
        private void OnCurrentOrderItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is INotifyPropertyChanged notifyItem)
                    {
                        notifyItem.PropertyChanged += OnOrderItemPropertyChanged;
                    }
                }
            }
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is INotifyPropertyChanged notifyItem)
                        notifyItem.PropertyChanged -= OnOrderItemPropertyChanged;
                }
            }
            NotifyCalculatedPropertiesChanged();
            OnPropertyChanged(nameof(HasSelectedItems));
        }
        private void OnOrderItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Product.Stock) || e.PropertyName == nameof(Product.Price))
            {
                if (e.PropertyName == nameof(Product.Stock) && sender is Product product)
                {
                    ValidateAndUpdateQuantity(product, product.Stock.ToString());
                }
                else
                {
                    NotifyCalculatedPropertiesChanged();
                }
            }
        }
        public void NotifyCalculatedPropertiesChanged()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(Tax));
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(Discount));
        }
        public void NotifySelectionChanged()
        {
            OnPropertyChanged(nameof(HasSelectedItems));
        }
        public void AddToCurrentOrder(Product product, bool fromListViewSelection = false)
        {
            if (product != null)
            {
                var existingItem = CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                if (existingItem == null)
                {
                    var newOrderItem = new Product
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Stock = 1,
                        ImageUrl = product.ImageUrl,
                        DateAdded = product.DateAdded,
                        CategoryName = product.CategoryName
                    };
                    CurrentOrderItems.Add(newOrderItem);
                    UpdateProductDisplayStock(product.Id);
                    AddToPersistentSelection(product.Id);
                    NotifyCalculatedPropertiesChanged();
                    if (!fromListViewSelection && PageReference is ContentPage page)
                    {
                        var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                        if (listView != null)
                        {
                            var originalProduct = DataSource.DisplayItems?.Cast<Product>().FirstOrDefault(p => p.Id == product.Id);
                            if (originalProduct != null && !listView.SelectedItems.Contains(originalProduct))
                            {
                                listView.SelectedItems.Add(originalProduct);
                                if (!SelectedItems.Contains(originalProduct))
                                {
                                    SelectedItems.Add(originalProduct);
                                    OnPropertyChanged(nameof(HasSelectedItems));
                                }
                            }
                        }
                    }
                    OnPropertyChanged(nameof(CurrentOrderItems));
                }
                else
                {
                    var originalStock = GetOriginalStock(product.Id);
                    var currentCartQuantity = existingItem.Stock;
                    if (currentCartQuantity < originalStock)
                    {
                        existingItem.Stock += 1;
                        UpdateProductDisplayStock(product.Id);
                        NotifyCalculatedPropertiesChanged();
                    }
                }
            }
        }
        public void RemoveFromCurrentOrder(Product product)
        {
            if (product != null)
            {
                var existingItem = CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                if (existingItem != null)
                {
                    CurrentOrderItems.Remove(existingItem);
                    UpdateProductDisplayStock(product.Id);
                    RemoveFromPersistentSelection(product.Id);
                    NotifyCalculatedPropertiesChanged();
                    if (PageReference is ContentPage page)
                    {
                        var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                        if (listView != null && listView.SelectedItems != null)
                        {
                            var homePage = page as NeuroPOS.MVVM.View.HomePage;
                            if (homePage != null)
                            {
                                listView.SelectionChanged -= homePage.ListView_SelectionChanged;
                            }
                            var originalProduct = DataSource.DisplayItems?.Cast<Product>().FirstOrDefault(p => p.Id == product.Id);
                            if (originalProduct != null && listView.SelectedItems.Contains(originalProduct))
                            {
                                listView.SelectedItems.Remove(originalProduct);
                            }
                            if (homePage != null)
                            {
                                listView.SelectionChanged += homePage.ListView_SelectionChanged;
                            }
                        }
                    }
                    var selectedProduct = SelectedItems.FirstOrDefault(x => x is Product p && p.Id == product.Id);
                    if (selectedProduct != null)
                    {
                        SelectedItems.Remove(selectedProduct);
                    }
                    OnPropertyChanged(nameof(HasSelectedItems));
                }
            }
        }
        public void IncrementQuantity(Product product)
        {
            if (product != null)
            {
                var existingItem = CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                if (existingItem != null)
                {
                    var originalStock = GetOriginalStock(product.Id);
                    var currentCartQuantity = existingItem.Stock;
                    if (currentCartQuantity < originalStock)
                    {
                        existingItem.Stock += 1;
                        UpdateProductDisplayStock(product.Id);
                        NotifyCalculatedPropertiesChanged();
                    }
                }
            }
        }
        public void DecrementQuantity(Product product)
        {
            if (product != null)
            {
                var existingItem = CurrentOrderItems.FirstOrDefault(x => x.Id == product.Id);
                if (existingItem != null && existingItem.Stock > 1)
                {
                    existingItem.Stock -= 1;
                    UpdateProductDisplayStock(product.Id);
                    NotifyCalculatedPropertiesChanged();
                }
                else if (existingItem != null && existingItem.Stock == 1)
                {
                    RemoveFromCurrentOrder(product);
                }
            }
        }
        private void UpdateProductDisplayStock(int productId)
        {
            var originalProduct = Products.FirstOrDefault(p => p.Id == productId);
            if (originalProduct == null) return;
            if (!_originalStocks.ContainsKey(productId))
                _originalStocks[productId] = originalProduct.Stock;
            var cartItem = CurrentOrderItems.FirstOrDefault(x => x.Id == productId);
            var cartQuantity = cartItem?.Stock ?? 0;
            var originalStock = _originalStocks[productId];
            originalProduct.Stock = Math.Max(0, originalStock - cartQuantity);
            (PageReference as HomePage)?.RefreshRow(originalProduct);
        }
        public int GetOriginalStock(int productId)
        {
            if (_originalStocks.ContainsKey(productId))
            {
                return _originalStocks[productId];
            }
            var product = Products.FirstOrDefault(p => p.Id == productId);
            if (product != null)
            {
                _originalStocks[productId] = product.Stock;
                return product.Stock;
            }
            return 0;
        }
        public int GetAvailableStock(int productId)
        {
            var originalStock = GetOriginalStock(productId);
            var cartItem = CurrentOrderItems.FirstOrDefault(x => x.Id == productId);
            var cartQuantity = cartItem?.Stock ?? 0;
            return Math.Max(0, originalStock - cartQuantity);
        }
        private Dictionary<int, int> _originalStocks = new Dictionary<int, int>();
        private void ResetAllProductStocks()
        {
            foreach (var kvp in _originalStocks)
            {
                var product = Products.FirstOrDefault(p => p.Id == kvp.Key);
                if (product != null)
                {
                    product.Stock = kvp.Value;
                }
            }
            _originalStocks.Clear();
        }
        public void ClearAllSelections()
        {
            SelectedItems.Clear();
            _persistentSelectedIds.Clear();
            CurrentOrderItems.Clear();
            Discount = 0;
            ResetAllProductStocks();
            NotifyCalculatedPropertiesChanged();
            OnPropertyChanged(nameof(HasSelectedItems));
            UpdateSelectedItemsCountDisplay();
            if (PageReference is ContentPage page)
            {
                var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                if (listView != null)
                {
                    var homePage = page as NeuroPOS.MVVM.View.HomePage;
                    if (homePage != null)
                    {
                        listView.SelectionChanged -= homePage.ListView_SelectionChanged;
                    }
                    listView.SelectedItems?.Clear();
                    if (homePage != null)
                    {
                        listView.SelectionChanged += homePage.ListView_SelectionChanged;
                    }
                }
            }
            OnPropertyChanged(nameof(HasSelectedItems));
        }
        public bool HasSelectedItems => _persistentSelectedIds?.Count > 0;
        public object PageReference { get; set; }
        public void AddSelectedItemsToOrder()
        {
            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                {
                    if (item is Product product)
                    {
                        AddToCurrentOrder(product, fromListViewSelection: false);
                    }
                }
                SelectedItems.Clear();
            }
        }
        public void ApplySorting()
        {
            DataSource.SortDescriptors.Clear();
            if (SortState == SortDirectionState.Ascending)
            {
                DataSource.SortDescriptors.Add(new SortDescriptor()
                {
                    PropertyName = "Name",
                    Direction = ListSortDirection.Ascending
                });
            }
            else if (SortState == SortDirectionState.Descending)
            {
                DataSource.SortDescriptors.Add(new SortDescriptor()
                {
                    PropertyName = "Name",
                    Direction = ListSortDirection.Descending
                });
            }
            RestoreListViewSelections();
            UpdateSelectedItemsCountDisplay();
        }
        public void AddToPersistentSelection(int productId)
        {
            if (!_persistentSelectedIds.Contains(productId))
            {
                _persistentSelectedIds.Add(productId);
                OnPropertyChanged(nameof(HasSelectedItems));
                UpdateSelectedItemsCountDisplay();
            }
        }
        public void RemoveFromPersistentSelection(int productId)
        {
            if (_persistentSelectedIds.Contains(productId))
            {
                _persistentSelectedIds.Remove(productId);
                OnPropertyChanged(nameof(HasSelectedItems));
                UpdateSelectedItemsCountDisplay();
            }
        }
        public bool IsInPersistentSelection(int productId)
        {
            return _persistentSelectedIds.Contains(productId);
        }
        public void UpdateSelectedItemsCountDisplay()
        {
            if (PageReference is ContentPage page)
            {
                var selectedValueLabel = page.FindByName("selectedValue") as Label;
                if (selectedValueLabel != null)
                {
                    selectedValueLabel.Text = _persistentSelectedIds.Count.ToString();
                }
            }
        }
        public void RefreshUI()
        {
            UpdateSelectedItemsCountDisplay();
            OnPropertyChanged(nameof(HasSelectedItems));
            RestoreListViewSelections();
        }
        public void RestoreListViewSelections()
        {
            if (PageReference is ContentPage page && _persistentSelectedIds?.Count > 0)
            {
                var listView = page.FindByName("listView") as Syncfusion.Maui.ListView.SfListView;
                if (listView != null)
                {
                    var homePage = page as NeuroPOS.MVVM.View.HomePage;
                    if (homePage != null)
                    {
                        listView.SelectionChanged -= homePage.ListView_SelectionChanged;
                    }
                    listView.SelectedItems?.Clear();
                    SelectedItems.Clear();
                    foreach (var productId in _persistentSelectedIds)
                    {
                        var displayProduct = DataSource.DisplayItems?.Cast<Product>()
                            .FirstOrDefault(p => p.Id == productId);
                        if (displayProduct != null)
                        {
                            listView.SelectedItems.Add(displayProduct);
                            SelectedItems.Add(displayProduct);
                        }
                    }
                    UpdateSelectedItemsCountDisplay();
                    if (homePage != null)
                    {
                        listView.SelectionChanged += homePage.ListView_SelectionChanged;
                    }
                }
            }
        }
        private bool FilterByActiveCategory(object obj)
        {
            if (obj is not Product product)
                return false;
            var activeCategory = Categories.FirstOrDefault(c => c.State == "Active Categorie");
            if (activeCategory == null)
                return true;
            return string.Equals(product.CategoryName, activeCategory.Name, StringComparison.OrdinalIgnoreCase);
        }
        public void UpdateDiscount(double newDiscount)
        {
            var maxDiscount = Math.Max(0, Subtotal);
            var validatedDiscount = Math.Min(Math.Max(0, newDiscount), maxDiscount);
            Discount = validatedDiscount;
        }
        private void UpdateTaxRateSilently(double newTaxRate)
        {
            _taxRate = newTaxRate;
        }
        public void UpdateCartTotals()
        {
            NotifyCalculatedPropertiesChanged();
        }
        public void ValidateAndUpdateQuantity(Product product, string newValue)
        {
            try
            {
                if (int.TryParse(newValue, out int newQuantity))
                {
                    if (newQuantity < 1)
                    {
                        newQuantity = 1;
                    }
                    product.Stock = newQuantity;
                    UpdateCartTotals();
                }
                else
                {
                    product.Stock = 1;
                    UpdateCartTotals();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in quantity validation: {ex.Message}");
            }
        }
        #endregion

        #region Commands
        public ICommand ToggleSortCommand => new Command(() =>
        {
            SortState = SortState switch
            {
                SortDirectionState.None => SortDirectionState.Ascending,
                SortDirectionState.Ascending => SortDirectionState.Descending,
                SortDirectionState.Descending => SortDirectionState.None,
                _ => SortDirectionState.None
            };
        });
        public ICommand RemoveFromCartCommand => new Command<Product>((product) =>
        {
            RemoveFromCurrentOrder(product);
        });
        public ICommand IncrementQuantityCommand => new Command<Product>((product) =>
        {
            IncrementQuantity(product);
        });
        public ICommand DecrementQuantityCommand => new Command<Product>((product) =>
        {
            DecrementQuantity(product);
        });
        public ICommand ClearAllSelectionsCommand => new Command(() =>
        {
            ClearAllSelections();
        });
        public ICommand SwitchCategoryState => new Command<string>((name) =>
        {
            if (name == "All")
            {
                foreach (var category in Categories)
                    category.State = "Inactive Categorie";
                Categories = new ObservableCollection<Category>(Categories);
            }
            else
            {
                var selected = Categories.FirstOrDefault(c => c.Name == name);
                foreach (var category in Categories)
                    category.State = "Inactive Categorie";
                selected.State = "Active Categorie";
                Categories = new ObservableCollection<Category>(Categories);
                DataSource.Filter = FilterByActiveCategory;
            }
            DataSource.RefreshFilter();
            OnPropertyChanged(nameof(DataSource));
            RestoreListViewSelections();
            UpdateSelectedItemsCountDisplay();
        });
        public ICommand EditTaxCommand => new Command(async () =>
        {
            await EditTaxRate();
        });
        public ICommand CashPaymentCommand => new Command<bool?>(
            async silent => await ProcessCashPayment(silent == true));
        public ICommand OnTabPaymentCommand => new AsyncRelayCommand(ProcessOnTabPaymentInteractive);
        public ICommand OnTabPaymentSilentCommand => new AsyncRelayCommand<Contact>(ProcessOnTabPaymentSilent);
        #endregion

        #region Tasks
        public void SortProduct()
        {
            DataSource = new DataSource() { Source = Products };
            DataSource.Refresh();
        }
        public async Task EditTaxRate()
        {
            try
            {
                var originalTaxRate = TaxRate;
                var popup = new EditTaxPopup(TaxRate);
                await AppShell.Current.ShowPopupAsync(popup);
                var newTaxRate = popup.Result;
                if (Math.Abs(newTaxRate - originalTaxRate) > 0.01)
                {
                    UpdateTaxRateSilently(newTaxRate);
                    OnPropertyChanged(nameof(TaxRate));
                    OnPropertyChanged(nameof(Tax));
                    OnPropertyChanged(nameof(Total));
                    var snackbar = Snackbar.Make($"Tax rate updated to {TaxRate:F1}%", duration: TimeSpan.FromSeconds(3));
                    await snackbar.Show();
                }
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make("Failed to update tax rate", duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        public async Task ProcessCashPayment(bool silent = false)
        {
            try
            {
                if (CurrentOrderItems.Count == 0)
                {
                    await Snackbar.Make("Cart is empty.", duration: TimeSpan.FromSeconds(3)).Show();
                    return;
                }
                if (!silent)
                {
                    var popup = new CashPaymentPopup(this);
                    await AppShell.Current.ShowPopupAsync(popup);
                    if (!popup.IsConfirmed) return;
                }
                await PersistCashTransaction();
                await Snackbar.Make("Cash payment processed successfully!",
                                    duration: TimeSpan.FromSeconds(3)).Show();
            }
            catch (Exception ex)
            {
                await Snackbar.Make($"Failed to process payment: {ex.Message}",
                                    duration: TimeSpan.FromSeconds(3)).Show();
            }
        }
        private Task ProcessOnTabPaymentInteractive() =>
    ProcessOnTabPayment(presetContact: null, silent: false);
        private Task ProcessOnTabPaymentSilent(Contact contact) =>
    ProcessOnTabPayment(presetContact: contact, silent: true);
        private async Task PersistCashTransaction()
        {
            var tx = new Transaction
            {
                TransactionType = "sell",
                IsPaid = true,
                Tax = Tax,
                Discount = Discount,
                TotalAmount = Total,
                ItemCount = CurrentOrderItems.Count,
                Lines = new()
            };
            var db = App.ProductRepo.GetItems().ToDictionary(p => p.Id);
            foreach (var c in CurrentOrderItems)
            {
                if (!db.TryGetValue(c.Id, out var prod)) continue;
                tx.Lines.Add(new TransactionLine
                {
                    Name = prod.Name,
                    Price = prod.Price,
                    Stock = c.Stock,
                    CategoryName = prod.CategoryName,
                    ImageUrl = prod.ImageUrl,
                    ProductId = prod.Id,
                    Product = prod
                });
                prod.Stock -= c.Stock;
                App.ProductRepo.UpdateItem(prod);
            }
            App.TransactionRepo.InsertItemWithChildren(tx);
            ClearAllSelections();
            _ = LoadDB();
        }
        public async Task ProcessOnTabPayment(Contact? presetContact = null, bool silent = false)
        {
            try
            {
                if (CurrentOrderItems.Count == 0)
                {
                    await Snackbar.Make("Cart is empty.", duration: TimeSpan.FromSeconds(3)).Show();
                    return;
                }
                Contact contact = presetContact;
                if (!silent)
                {
                    var popup = new OnTabPaymentPopup(this);
                    await AppShell.Current.ShowPopupAsync(popup);
                    if (!popup.IsConfirmed) return;
                    contact = popup.SelectedContact;
                    if (contact == null)
                    {
                        await Snackbar.Make("Select a contact before proceeding.",
                                            duration: TimeSpan.FromSeconds(3)).Show();
                        return;
                    }
                }
                await PersistOnTabTransaction(contact);
                await Snackbar.Make($"Credit transaction created for {contact.Name}.",
                                    duration: TimeSpan.FromSeconds(3)).Show();
            }
            catch (Exception ex)
            {
                await Snackbar.Make($"Failed to create credit transaction: {ex.Message}",
                                    duration: TimeSpan.FromSeconds(3)).Show();
            }
        }
        private async Task PersistOnTabTransaction(Contact contact)
        {
            var tx = new Transaction
            {
                TransactionType = "sell",
                IsPaid = false,
                Tax = Tax,
                Discount = Discount,
                TotalAmount = Total,
                ItemCount = CurrentOrderItems.Count,
                ContactId = contact.Id,
                Lines = new()
            };
            var db = App.ProductRepo.GetItems().ToDictionary(p => p.Id);
            foreach (var c in CurrentOrderItems)
            {
                if (!db.TryGetValue(c.Id, out var prod)) continue;
                tx.Lines.Add(new TransactionLine
                {
                    Name = prod.Name,
                    Price = prod.Price,
                    Stock = c.Stock,
                    CategoryName = prod.CategoryName,
                    ImageUrl = prod.ImageUrl,
                    ProductId = prod.Id,
                    Product = prod
                });
                prod.Stock -= c.Stock;
                App.ProductRepo.UpdateItem(prod);
            }
            App.ContactRepo.AddNewChildToParentRecursively(
                contact, tx, (ct, list) => ct.Transactions = list.ToList());
            ClearAllSelections();
            _ = LoadDB();
        }
        public async Task LoadDB()
        {
            if (App.ProductRepo == null || App.CategoryRepo == null)
            {
                return;
            }
            Products.Clear();
            Categories.Clear();
            Contacts.Clear();
            var DBProducts = App.ProductRepo.GetItems();
            var DBCategories = App.CategoryRepo.GetItems();
            var DBContacts = App.ContactRepo.GetItemsWithChildren();
            foreach (var item in DBProducts)
            {
                Products.Add(item);
            }
            foreach (var item in DBCategories)
            {
                Categories.Add(item);
            }
            foreach (var item in DBContacts)
            {
                Contacts.Add(item);
            }
            SortProduct();
        }
        #endregion

        #region ai logic
        private string _assistantInput;
        public string AssistantInput
        {
            get => _assistantInput;
            set => SetProperty(ref _assistantInput, value);
        }
        public AssistantClient _assistant;
        public InventoryVM _inventory;
        public OrderVM _order;
        private readonly Dictionary<string, string> _alias =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["coca"] = "Coca-Cola",
                ["pepsi"] = "Pepsi",
                ["coke"] = "Coca-Cola",
                ["chips"] = "Chips"
            };
        public ICommand SendAssistantCommand => new AsyncRelayCommand(SendAssistantAsync);
        #region stats automation
        private int CountTransactionsToday(string type = null)
        {
            var today = DateTime.Today;
            return App.TransactionRepo.GetItems()
                .Count(t => t.Date.Date == today &&
                       (type == null || t.TransactionType == type));
        }
        private string ItemsSoldToday()
        {
            var today = DateTime.Today;
            var sold = App.TransactionRepo.GetItemsWithChildren()
                .Where(t => t.TransactionType == "sell" && t.Date.Date == today)
                .SelectMany(t => t.Lines ?? Enumerable.Empty<TransactionLine>())
                .GroupBy(l => l.Name)
                .Select(g => $"{g.Key} × {g.Sum(l => l.Stock)}");
            return !sold.Any()
                ? "No items sold today."
                : string.Join(", ", sold);
        }
        private string SalesStats(string period)
        {
            DateTime today = DateTime.Today;
            DateTime start, end = today;
            switch (period)
            {
                case "today":
                    start = today;
                    break;
                case "yesterday":
                    start = today.AddDays(-1);
                    end = start;
                    break;
                case "last_week":
                    start = today.AddDays(-7);
                    break;
                case "last_month":
                    start = today.AddMonths(-1);
                    break;
                default:
                    start = today.AddDays(-30);
                    break;
            }
            double total = App.TransactionRepo.GetItems()
                          .Where(t => t.TransactionType == "sell" &&
                                      t.Date.Date >= start && t.Date.Date <= end)
                          .Sum(t => t.TotalAmount);
            string label = period switch
            {
                "today" => "today",
                "yesterday" => "yesterday",
                "last_week" => "in the last 7 days",
                "last_month" => "in the last 30 days",
                _ => "in the last 30 days"
            };
            return $"📊 Sales {label}: {total:C}";
        }
        private double CashFlowToday()
        {
            var today = DateTime.Today;
            return App.CashFlowSnapshotRepo.GetItems()
                   .Where(s => s.Date.Date == today)
                   .OrderByDescending(s => s.Date)
                   .FirstOrDefault()?.TotalValue ?? 0;
        }
        #endregion
        private async Task SendAssistantAsync()
        {
            try
            {
                var raw = await _assistant.GetRawAssistantReplyAsync(
            AssistantInput ?? string.Empty,
            Products.Select(p => p.Name).ToList(),
            Contacts.Select(c => c.Name).ToList());
                AssistantInput = string.Empty;
                var cleaned = CleanupRaw(raw);
                var intent = JsonSerializer.Deserialize<AssistantIntent>(
                                cleaned,
                                new JsonSerializerOptions(JsonSerializerDefaults.Web));
                if (intent is null)
                {
                    await Snackbar.Make("Assistant returned empty intent.", duration: TimeSpan.FromSeconds(3)).Show();
                    return;
                }
                if (intent.Items?.Any() == true)
                {
                    var catalog = Products.Select(p => p.Name);
                    foreach (var itm in intent.Items)
                        itm.Name = EntityResolver.ResolveProduct(itm.Name, catalog)
                                   ?? itm.Name;
                }
                if (!string.IsNullOrWhiteSpace(intent.Contact))
                {
                    var names = Contacts.Select(c => c.Name);
                    intent.Contact = EntityResolver.ResolveContact(intent.Contact, names)
                                     ?? intent.Contact;
                }
                await ExecuteIntentAsync(intent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("JSON‑parse failure:");
                Debug.WriteLine(ex);
                await Snackbar.Make($"Assistant sent invalid JSON – see Output window",
                                    duration: TimeSpan.FromSeconds(3)).Show();
            }
        }
        private static string CleanupRaw(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;
            text = text.Trim();
            if (text.StartsWith("```"))
            {
                var open = text.IndexOf('{');
                var close = text.LastIndexOf('}');
                if (open >= 0 && close > open) text = text.Substring(open, close - open + 1);
            }
            var endThink = text.IndexOf("</think>", StringComparison.OrdinalIgnoreCase);
            if (endThink >= 0)
                text = text[(endThink + "</think>".Length)..].Trim();
            if (text.StartsWith("```"))
            {
                var start = text.IndexOf('{');
                var end = text.LastIndexOf('}');
                if (start >= 0 && end > start)
                    text = text[start..(end + 1)];
            }
            if (text.StartsWith('<'))
            {
                var brace = text.IndexOf('{');
                if (brace >= 0)
                    text = text[brace..];
            }
            var actionPos = text.IndexOf("\"action\"", StringComparison.OrdinalIgnoreCase);
            if (actionPos >= 0)
            {
                var braceStart = text.LastIndexOf('{', actionPos);
                var braceEnd = text.LastIndexOf('}');
                if (braceStart >= 0 && braceEnd > braceStart)
                    text = text[braceStart..(braceEnd + 1)];
            }
            return text.Trim();
        }
        public async Task<string?> ExecuteIntentAsync(AssistantIntent intent)
        {
            if (intent == null) return null;
            switch (intent.Action?.ToLowerInvariant())
            {
                case "transactions_today":
                    return $"🧾 Total transactions today: {CountTransactionsToday()}";
                case "transactions_sell_today":
                    return $"📈 Sell transactions today: {CountTransactionsToday("sell")}";
                case "transactions_buy_today":
                    return $"📉 Buy transactions today: {CountTransactionsToday("buy")}";
                case "items_sold_today":
                    return ItemsSoldToday();
                case "sales_stats":
                    var period = intent.Period?.ToLowerInvariant() ?? "last_30_days";
                    return SalesStats(period);
                case "cash_flow_today":
                    return $"💰 Cash flow today: {CashFlowToday():C}";
                case "clear":
                    ClearAllSelections();
                    return "🗑️  Cart cleared.";
                case "show_cart":
                    return CurrentOrderItems.Any()
                        ? string.Join(", ",
                              CurrentOrderItems.Select(i => $"{i.Name} × {i.Stock}"))
                        : "Cart is empty.";
                case "discount_only":
                    if (intent.DiscountAmount > 0)
                    {
                        UpdateDiscount((double)intent.DiscountAmount);
                        return $"💸 Discount of {intent.DiscountAmount:C} applied.";
                    }
                    return "No discount applied.";
                case "check_stock":
                    if (intent.Items?.Any() != true)
                        return "No items specified.";
                    var report = string.Join(", ",
                        intent.Items.Select(i =>
                        {
                            var prod = Products.FirstOrDefault(p =>
                                      p.Name.Equals(i.Name, StringComparison.OrdinalIgnoreCase));
                            return prod == null
                                 ? $"{i.Name}: not found"
                                 : $"{prod.Name}: {GetAvailableStock(prod.Id)} left";
                        }));
                    return report;
                case "inventory_value":
                    var value = Products.Sum(p => GetOriginalStock(p.Id) * p.Price);
                    return $"📦 Inventory value: {value:C}";
                case "sell":
                    return await HandleSellIntentAsync(intent);
                case "add_category":
                    return _inventory.CreateCategory(intent.CategoryName, intent.Description);
                case "edit_category_name":
                    return _inventory.RenameCategory(intent.CategoryName, intent.Description);
                case "edit_category_desc":
                    return _inventory.UpdateCategoryDescription(intent.CategoryName, intent.Description);
                case "add_product":
                    return _inventory.QuickAddProduct(
                               intent.ProductName,
                               intent.Price ?? 0,
                               intent.Cost ?? 0,
                               intent.CategoryName);
                case "edit_product_price":
                    return _inventory.UpdateProductPrice(intent.ProductName, intent.Price);
                case "edit_product_cost":
                    return _inventory.UpdateProductCost(intent.ProductName, intent.Cost);
                case "edit_product_category":
                    return _inventory.UpdateProductCategory(intent.ProductName, intent.CategoryName);
                case "delete_product":
                    return _inventory.DeleteProduct(intent.ProductName);
                case "buy_products":
                    {
                        var list = intent.Items;
                        if ((list == null || list.Count == 0) &&
                            !string.IsNullOrWhiteSpace(intent.ProductName) &&
                            intent.Quantity.HasValue)
                        {
                            list = new List<ItemIntent>
                            {
                                new ItemIntent
                                {
                                    Name     = intent.ProductName,
                                    Quantity = intent.Quantity.Value
                                }
                            };
                        }
                        return _inventory.RecordBuyTransaction(list);
                    };         
                case "add_order":
                    return await AiAddOrderAsync(intent);
                case "query_orders":
                    return await AiQueryOrdersAsync(intent);
                case "confirm_order":
                    return await AiConfirmOrderAsync(intent);
                case "delete_order":
                    return await AiDeleteOrderAsync(intent);
                case "edit_order":
                    return await AiEditOrderAsync(intent);
                case "orders_info":
                    intent.Action = "query_orders";
                    goto case "query_orders";
                default:
                    return "Sorry, I didn’t understand that action.";
            }
        }
        private async Task<string> HandleSellIntentAsync(AssistantIntent intent)
        {
            if (intent.Items?.Any() != true)
                return "No items specified.";
            foreach (var itm in intent.Items)
            {
                var lookupName = _alias.TryGetValue(itm.Name, out var mapped)
                                    ? mapped : itm.Name;
                var prod = Products.FirstOrDefault(p =>
                             p.Name.Equals(lookupName, StringComparison.OrdinalIgnoreCase));
                if (prod == null) continue;
                for (int i = 0; i < itm.Quantity; i++)
                    AddToCurrentOrder(prod);
            }
            if (intent.DiscountAmount > 0)
                UpdateDiscount((double)intent.DiscountAmount);
            switch (intent.Payment?.ToLowerInvariant())
            {
                case "cash":
                    CashPaymentCommand.Execute(true);
                    return "✅ Items added and paid in cash.";
                case "on_tab":
                    var contact = Contacts.FirstOrDefault(c =>
                        c.Name.Equals(intent.Contact, StringComparison.OrdinalIgnoreCase));
                    if (contact != null)
                    {
                        OnTabPaymentSilentCommand.Execute(contact);
                        return $"✅ Items added on {contact.Name}’s tab.";
                    }
                    return "Contact not found.";
                default:
                    return "✅ Items added to cart.";
            }
        }
        protected bool SetProperty<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingField, value))
                return false;
            backingField = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public async Task ProcessAssistantRawAsync(string raw)
        {
            var cleaned = CleanupRaw(raw);
            var intent = JsonSerializer.Deserialize<AssistantIntent>(
                              cleaned, new JsonSerializerOptions(JsonSerializerDefaults.Web));
            if (intent is not null)
                await ExecuteIntentAsync(intent);
        }
        #region Orders automation
        private async Task<string> AiAddOrderAsync(AssistantIntent intent)
        {
            if (intent.Items?.Any() != true)
                return "No items specified.";
            var rawName = (intent.Contact ?? "Unknown").Trim();
            var hasAdjective = rawName.StartsWith("loyal ", StringComparison.OrdinalIgnoreCase) ||
                               rawName.StartsWith("dear ", StringComparison.OrdinalIgnoreCase) ||
                               rawName.StartsWith("valued ", StringComparison.OrdinalIgnoreCase);
            var cleanedName = rawName
                .Replace("loyal ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("dear ", "", StringComparison.OrdinalIgnoreCase)
                .Replace("valued ", "", StringComparison.OrdinalIgnoreCase)
                .Trim();
            Contact contact = null;
            if (hasAdjective)
            {
                contact = Contacts.FirstOrDefault(c =>
                    c.Name.Equals(cleanedName, StringComparison.OrdinalIgnoreCase));
            }
            var order = new Order
            {
                CustomerName = cleanedName,
                ContactId = contact?.Id ?? 0,
                Date = DateTime.Now,
                IsConfirmed = false,
                Discount = intent.DiscountAmount ?? 0,
                Tax = 2,
                Lines = new(),
            };
            double subTotal = 0;
            foreach (var itm in intent.Items)
            {
                var prod = Products.FirstOrDefault(p =>
                    p.Name.Equals(itm.Name, StringComparison.OrdinalIgnoreCase));
                if (prod is null) continue;
                order.Lines.Add(new TransactionLine
                {
                    Name = prod.Name,
                    Price = prod.Price,
                    Stock = itm.Quantity,
                    CategoryName = prod.CategoryName,
                    ProductId = prod.Id
                });
                subTotal += prod.Price * itm.Quantity;
            }
            order.TotalAmount = subTotal - order.Discount + subTotal * 0.02;
            App.OrderRepo.InsertItemWithChildren(order);
            await _order.RefreshOrders();
            return $"🆕 Order #{order.Id} for {order.CustomerName} created.";
        }
        private async Task<string> AiQueryOrdersAsync(AssistantIntent intent)
        {
            var list = App.OrderRepo.GetItemsWithChildren()
                       .Where(o => o.CustomerName.Equals(intent.Contact, StringComparison.OrdinalIgnoreCase))
                       .ToList(); var cust = intent.Contact?.Trim() ?? "";
            var matches = list.Where(o =>
                          o.CustomerName.Equals(cust, StringComparison.OrdinalIgnoreCase));
            if (!matches.Any())
                matches = list.Where(o =>
                          o.CustomerName.Contains(cust, StringComparison.OrdinalIgnoreCase));
            if (!list.Any())
                return "No orders found.";
            if (intent.ResponseMode == "yes_no")
                return list.Any(o => !o.IsConfirmed) ? "Yes." : "No.";
            return string.Join("\n", list.Select(o =>
                   $"#{o.Id} {(o.IsConfirmed ? "✔" : "⏳")} {o.TotalAmount:C} "
                 + string.Join(", ", o.Lines.Select(l => $"{l.Name}×{l.Stock}"))));
        }
        private async Task<string> AiConfirmOrderAsync(AssistantIntent intent)
        {
            /* 0. guards */
            if (intent.OrderId is null)
                return "order_id missing.";

            var order = App.OrderRepo.GetItemsWithChildren()
                       .FirstOrDefault(o => o.Id == intent.OrderId);

            if (order == null)
                return "Order not found.";

            if (!string.IsNullOrWhiteSpace(intent.Contact) &&
                !order.CustomerName.Equals(intent.Contact, StringComparison.OrdinalIgnoreCase))
                return "Contact / order mismatch.";

            if (order.IsConfirmed)
                return "Order already confirmed.";

            if (order.Lines == null || order.Lines.Count == 0)
                return "Order has no products; cannot confirm an empty order.";

            /* 1. Stock‑level validation */
            var dbProducts = App.ProductRepo.GetItems().ToList();
            var stockErrors = new List<string>();

            foreach (var line in order.Lines)
            {
                var dbProd = dbProducts.FirstOrDefault(p => p.Name == line.Name);
                if (dbProd == null)
                {
                    stockErrors.Add($"Product '{line.Name}' not found.");
                    continue;
                }

                if (dbProd.Stock < line.Stock)
                {
                    stockErrors.Add(
                        $"Insufficient stock for '{line.Name}'. " +
                        $"Available: {dbProd.Stock}, required: {line.Stock}");
                }
            }

            if (stockErrors.Any())
                return "Stock validation failed:\n" + string.Join("\n", stockErrors);

            /* 2. Build Transaction (sell or credit) */
            var tx = new Transaction
            {
                TransactionType = "sell",
                IsPaid = order.ContactId == 0,          // paid if no contact
                Tax = order.CalculatedTaxAmount,
                Discount = order.Discount,
                TotalAmount = order.CalculatedTotalAmount,
                ItemCount = order.Lines.Count,
                ContactId = order.ContactId == 0 ? null : order.ContactId,
                Lines = new List<TransactionLine>()
            };

            foreach (var line in order.Lines)
            {
                var dbProd = dbProducts.First(p => p.Name == line.Name);

                tx.Lines.Add(new TransactionLine
                {
                    Name = line.Name,
                    Price = line.Price,
                    Stock = line.Stock,
                    CategoryName = line.CategoryName,
                    ImageUrl = line.ImageUrl,
                    Product = dbProd,
                    ProductId = dbProd.Id
                });

                dbProd.Stock -= line.Stock;
                App.ProductRepo.UpdateItem(dbProd);
            }

            /* 3. Persist transaction */
            if (order.ContactId != 0)
            {
                var customer = App.ContactRepo.GetItemsWithChildren()
                              .FirstOrDefault(c => c.Id == order.ContactId);

                if (customer != null)
                {
                    App.ContactRepo.AddNewChildToParentRecursively(
                        customer,
                        tx,
                        (c, list) => c.Transactions = list.ToList());
                }
            }
            else
            {
                App.TransactionRepo.InsertItemWithChildren(tx);
            }

            /* 4. Mark order confirmed & persist header + children */
            order.IsConfirmed = true;
            App.OrderRepo.UpdateItemWithChildren(order);

            await _order.RefreshOrders();

            return order.ContactId == 0
                   ? $"✅ Order #{order.Id} confirmed and paid in cash."
                   : $"✅ Order #{order.Id} confirmed and added to {order.CustomerName}'s tab.";
        }
        private async Task<string> AiDeleteOrderAsync(AssistantIntent intent)
        {
            if (intent.OrderId is null) return "order_id missing.";
            var order = App.OrderRepo.GetItems()
                        .FirstOrDefault(o => o.Id == intent.OrderId &&
                              o.CustomerName.Equals(intent.Contact, StringComparison.OrdinalIgnoreCase));
            if (order is null) return "Order not found.";
            if (order.IsConfirmed) return "Cannot delete confirmed order.";
            App.OrderRepo.DeleteItem(order);
            await _order.RefreshOrders();
            return $"🗑️ Order #{order.Id} deleted.";
        }
        private async Task<string> AiEditOrderAsync(AssistantIntent intent)
        {
            if (intent.OrderId is null)
                return "order_id missing.";
            var order = App.OrderRepo.GetItemsWithChildren()
                       .FirstOrDefault(o => o.Id == intent.OrderId);
            if (order is null)
                return "Order not found.";
            if (!string.IsNullOrWhiteSpace(intent.Contact) &&
                !order.CustomerName.Equals(intent.Contact, StringComparison.OrdinalIgnoreCase))
                return "Contact / order mismatch.";
            if (order.IsConfirmed)
                return "Order already confirmed – cannot edit.";
            if (!string.IsNullOrWhiteSpace(intent.Contact) &&
    !order.CustomerName.Equals(intent.Contact, StringComparison.OrdinalIgnoreCase))
                return "Contact / order mismatch.";
            var srcItems = intent.New_Items?.Any() == true
                           ? intent.New_Items
                           : (intent.Items?.Any() == true ? intent.Items : null);
            if (srcItems is null && intent.New_Discount is null)
                return "Nothing to change.";
            var dbProducts = App.ProductRepo.GetItems()
                             .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            foreach (var itm in srcItems ?? Enumerable.Empty<ItemIntent>())
            {
                if (!dbProducts.TryGetValue(itm.Name, out var prod))
                    continue;
                var existing = order.Lines.FirstOrDefault(l => l.ProductId == prod.Id);
                if (existing != null)
                {
                    if (itm.Quantity == 0)
                    {
                        App.OrderRepo.RemoveChildFromParent(
                            order, existing,
                            (parent, list) => parent.Lines = list.ToList());
                    }
                    else
                    {
                        existing.Stock = itm.Quantity;
                    }
                }
                else if (itm.Quantity > 0)
                {
                    var newLine = new TransactionLine
                    {
                        Name = prod.Name,
                        Price = prod.Price,
                        Stock = itm.Quantity,
                        CategoryName = prod.CategoryName,
                        ProductId = prod.Id
                    };
                    App.OrderRepo.AddNewChildToParent(
                        order, newLine,
                        (parent, list) => parent.Lines = list.ToList());
                }
            }
            if (intent.New_Discount is not null)
                order.Discount = intent.New_Discount.Value;
            var sub = order.Lines.Sum(l => l.Price * l.Stock);
            order.TotalAmount = sub
                               - order.Discount
                               + sub * order.Tax / 100.0;
            App.OrderRepo.UpdateItemWithChildren(order, recursive: true);
            await _order.RefreshOrders();
            return $"✏️ Order #{order.Id} updated.";
        }
        #endregion
        #endregion
    }
}