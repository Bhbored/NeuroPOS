using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.Popups;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using PropertyChanged;
using Contact = NeuroPOS.MVVM.Model.Contact;
namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class OrderVM : INotifyPropertyChanged
    {

        #region Private Fields

        private ObservableCollection<Order> _orders;
        private ObservableCollection<Order> _filteredOrders;
        private ObservableCollection<Order> _displayedOrders;
        private Order _selectedOrder;
        private string _searchText = "";
        private bool _isRefreshing = false;
        private int _isEditMode = 0;
        private int _viewMode = 0;
        private Order _editingOrder;
        private bool _isLoading = false;
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;
        private int _totalOrders = 0;
        private string _selectedStatusFilter = "All";
        private DateTime? _filterStartDate;
        private DateTime? _filterEndDate;
        private bool _isDateFilterActive = false;
        private string _dateFilterSummary = string.Empty;
        private bool _isStatusFilterActive = false;
        private string _editCustomerName = "";
        private DateTime _editDate = DateTime.Now;
        private bool _editIsConfirmed = false;
        private double _editTotalAmount = 0;
        private int _newOrderId = 0;
        private string _newOrderCustomerName = "";
        private DateTime _newOrderDate = DateTime.Now;
        private bool _newOrderIsConfirmed = false;
        private double _newOrderTotalAmount = 0;
        private double _newOrderSubTotalAmount = 0;
        private double _newOrderDiscount = 0;
        private double _newOrderTaxRate = 2.0;
        private ObservableCollection<TransactionLine> _newOrderLines;
        private bool _useExistingContact = false;
        private Contact _selectedContact;
        private Product _selectedProduct;
        private int _selectedProductQuantity = 1;
        #endregion

        public OrderVM()
        {
            LoadTestData();
        }

        #region Properties

        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
                ApplyFilters();
            }
        }
        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Contact> Contacts { get; set; }
        public ObservableCollection<Order> FilteredOrders
        {
            get => _filteredOrders;
            set
            {
                _filteredOrders = value;
                OnPropertyChanged(nameof(FilteredOrders));
                UpdatePagination();
            }
        }
        public ObservableCollection<Order> DisplayedOrders
        {
            get => _displayedOrders;
            set
            {
                _displayedOrders = value;
                OnPropertyChanged(nameof(DisplayedOrders));
            }
        }
        public Order SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
                if (value != null)
                {
                    ShowOrderDetails(value);
                }
                else
                {
                    ShowEmptyState();
                }
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                ApplyFilters();
            }
        }
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        public int IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                OnPropertyChanged(nameof(IsEditMode));
                UpdateViewMode();
            }
        }
        public int ViewMode
        {
            get => _viewMode;
            set
            {
                _viewMode = value;
                OnPropertyChanged(nameof(ViewMode));
            }
        }
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }
        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdateDisplayedOrders();
            }
        }
        public int PageSize
        {
            get => _pageSize;
            set
            {
                OnPropertyChanged(nameof(PageSize));
            }
        }
        public int TotalPages
        {
            get => _totalPages;
            set
            {
                _totalPages = value;
                OnPropertyChanged(nameof(TotalPages));
            }
        }
        public int TotalOrders
        {
            get => _totalOrders;
            set
            {
                _totalOrders = value;
                OnPropertyChanged(nameof(TotalOrders));
            }
        }
        public bool HasNextPage => CurrentPage < TotalPages;
        public bool HasPreviousPage => CurrentPage > 1;
        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                _selectedStatusFilter = value;
                OnPropertyChanged(nameof(SelectedStatusFilter));
                ApplyFilters();
            }
        }
        public DateTime? FilterStartDate
        {
            get => _filterStartDate;
            set
            {
                _filterStartDate = value;
                OnPropertyChanged(nameof(FilterStartDate));
                ApplyFilters();
            }
        }
        public DateTime? FilterEndDate
        {
            get => _filterEndDate;
            set
            {
                _filterEndDate = value;
                OnPropertyChanged(nameof(FilterEndDate));
                ApplyFilters();
            }
        }
        public bool IsDateFilterActive
        {
            get => _isDateFilterActive;
            set
            {
                _isDateFilterActive = value;
                OnPropertyChanged(nameof(IsDateFilterActive));
                OnPropertyChanged(nameof(HasAnyActiveFilter));
            }
        }
        public string DateFilterSummary
        {
            get => _dateFilterSummary;
            set
            {
                _dateFilterSummary = value;
                OnPropertyChanged(nameof(DateFilterSummary));
            }
        }
        public bool IsStatusFilterActive
        {
            get => _isStatusFilterActive;
            set
            {
                _isStatusFilterActive = value;
                OnPropertyChanged(nameof(IsStatusFilterActive));
                OnPropertyChanged(nameof(HasAnyActiveFilter));
            }
        }
        public bool HasAnyActiveFilter
        {
            get => _isDateFilterActive || _isStatusFilterActive;
        }
        public string EditCustomerName
        {
            get => _editCustomerName;
            set
            {
                _editCustomerName = value;
                OnPropertyChanged(nameof(EditCustomerName));
            }
        }
        public DateTime EditDate
        {
            get => _editDate;
            set
            {
                _editDate = value;
                OnPropertyChanged(nameof(EditDate));
            }
        }
        public bool EditIsConfirmed
        {
            get => _editIsConfirmed;
            set
            {
                _editIsConfirmed = value;
                OnPropertyChanged(nameof(EditIsConfirmed));
            }
        }
        public double EditTotalAmount
        {
            get => _editTotalAmount;
            set
            {
                _editTotalAmount = value;
                OnPropertyChanged(nameof(EditTotalAmount));
            }
        }
        public int NewOrderId
        {
            get => _newOrderId;
            set
            {
                _newOrderId = value;
                OnPropertyChanged(nameof(NewOrderId));
            }
        }
        public string NewOrderCustomerName
        {
            get => _newOrderCustomerName;
            set
            {
                _newOrderCustomerName = value;
                OnPropertyChanged(nameof(NewOrderCustomerName));
            }
        }
        public DateTime NewOrderDate
        {
            get => _newOrderDate;
            set
            {
                _newOrderDate = value;
                OnPropertyChanged(nameof(NewOrderDate));
            }
        }
        public bool NewOrderIsConfirmed
        {
            get => _newOrderIsConfirmed;
            set
            {
                _newOrderIsConfirmed = value;
                OnPropertyChanged(nameof(NewOrderIsConfirmed));
            }
        }
        public double NewOrderTotalAmount
        {
            get => _newOrderTotalAmount;
            set
            {
                _newOrderTotalAmount = value;
                OnPropertyChanged(nameof(NewOrderTotalAmount));
            }
        }
        public double NewOrderSubTotalAmount
        {
            get => _newOrderSubTotalAmount;
            set
            {
                _newOrderSubTotalAmount = value;
                OnPropertyChanged(nameof(NewOrderSubTotalAmount));
            }
        }
        public double NewOrderDiscount
        {
            get => _newOrderDiscount;
            set
            {
                _newOrderDiscount = value;
                OnPropertyChanged(nameof(NewOrderDiscount));
            }
        }
        public double NewOrderTaxRate
        {
            get => _newOrderTaxRate;
            set
            {
                _newOrderTaxRate = value;
                OnPropertyChanged(nameof(NewOrderTaxRate));
            }
        }
        public ObservableCollection<TransactionLine> NewOrderLines
        {
            get => _newOrderLines;
            set
            {
                _newOrderLines = value;
                OnPropertyChanged(nameof(NewOrderLines));
            }
        }
        public bool UseExistingContact
        {
            get => _useExistingContact;
            set
            {
                _useExistingContact = value;
                OnPropertyChanged(nameof(UseExistingContact));
            }
        }
        public Contact SelectedContact
        {
            get => _selectedContact;
            set
            {
                _selectedContact = value;
                OnPropertyChanged(nameof(SelectedContact));
                if (value != null)
                {
                    NewOrderCustomerName = value.Name;
                }
            }
        }
        public Product SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged(nameof(SelectedProduct));
            }
        }
        public int SelectedProductQuantity
        {
            get => _selectedProductQuantity;
            set
            {
                _selectedProductQuantity = value;
                OnPropertyChanged(nameof(SelectedProductQuantity));
            }
        }

        public List<string> StatusFilters => new List<string> { "All", "Completed", "Pending" };
        #endregion

        #region Commands

        public ICommand AddNewOrderCommand => new Command(async () => await AddNewOrder());
        public ICommand EditOrderCommand => new Command<Order>(async order => await EditOrder(order));
        public ICommand DeleteOrderCommand => new Command<Order>(async order => await DeleteOrder(order));
        public ICommand SaveOrderCommand => new Command(async () => await SaveOrder());
        public ICommand CancelEditCommand => new Command(() => CancelEdit());
        public ICommand RefreshOrdersCommand => new Command(async () => await RefreshOrders());
        public ICommand NextPageCommand => new Command(() => NextPage(), () => HasNextPage);
        public ICommand PreviousPageCommand => new Command(() => PreviousPage(), () => HasPreviousPage);
        public ICommand FirstPageCommand => new Command(() => FirstPage(), () => HasPreviousPage);
        public ICommand LastPageCommand => new Command(() => LastPage(), () => HasNextPage);
        public ICommand ConfirmOrderCommand => new Command<Order>(async order => await ConfirmOrder(order));
        public ICommand ClearFiltersCommand => new Command(() => ClearFilters());
        public ICommand SelectOrderCommand => new Command<Order>(order => SelectOrder(order));
        public ICommand AddProductToOrderCommand => new Command(async () => await AddProductToOrder());
        public ICommand RemoveProductFromOrderCommand => new Command<TransactionLine>(line => RemoveProductFromOrder(line));
        public ICommand AddSelectedProductToOrderCommand => new Command(() => AddSelectedProductToOrder());
        public ICommand IncreaseTaxRateCommand => new Command(() => IncreaseTaxRate());
        public ICommand DecreaseTaxRateCommand => new Command(() => DecreaseTaxRate());
        public ICommand ShowDatePickerCommand => new Command(async () => await ShowDatePicker());
        #endregion

        #region Methods

        private void LoadTestData()
        {
            var testOrders = new List<Order>();
            var customerNames = new[] { "Sophie Carter", "Owen Bennett", "Olivia Hayes", "Jackson Hayes", "Emma Wilson",
"Liam Davis", "Ava Miller", "Noah Garcia", "Mia Anderson", "Ethan Thompson",
"Isabella Martinez", "William Johnson", "Sophia Brown", "James Wilson", "Charlotte Davis",
"Benjamin Miller", "Amelia Garcia", "Lucas Rodriguez", "Harper Martinez", "Henry Anderson",
"Evelyn Taylor", "Alexander Thomas", "Abigail Hernandez", "Michael Lopez", "Emily Gonzalez",
"Daniel Perez", "Elizabeth Torres", "Matthew Sanchez", "Sofia Ramirez", "David Flores" };
            var productNames = new[] { "Laptop", "Smartphone", "Tablet", "Headphones", "Mouse", "Keyboard", "Monitor",
"Speaker", "Camera", "Printer", "Scanner", "Router", "Cable", "Adapter", "Charger" };
            var random = new Random();
            var startDate = new DateTime(2024, 1, 1);
            for (int i = 1; i <= 50; i++)
            {
                var orderDate = startDate.AddDays(random.Next(0, 365));
                var customerName = customerNames[random.Next(customerNames.Length)];
                var isConfirmed = random.Next(2) == 1;
                var itemCount = random.Next(1, 6);
                var totalAmount = 0.0;
                var orderLines = new List<TransactionLine>();
                for (int j = 0; j < itemCount; j++)
                {
                    var productName = productNames[random.Next(productNames.Length)];
                    var price = Math.Round(random.NextDouble() * 50 + 5, 2);
                    var quantity = random.Next(1, 4);
                    var cost = Math.Round(price * 0.6, 2);
                    var line = new TransactionLine
                    {
                        Id = i * 100 + j,
                        Name = productName,
                        Price = price,
                        Cost = cost,
                        Stock = quantity,
                        DateAdded = orderDate,
                        CategoryName = "Electronics",
                        OrderId = i
                    };
                    orderLines.Add(line);
                    totalAmount += price * quantity;
                }
                testOrders.Add(new Order
                {
                    Id = i,
                    Date = orderDate,
                    CustomerName = customerName,
                    IsConfirmed = isConfirmed,
                    TotalAmount = Math.Round(totalAmount, 2),
                    ItemCount = itemCount,
                    Lines = orderLines
                });
            }
            Orders = new ObservableCollection<Order>(testOrders);
            FilteredOrders = new ObservableCollection<Order>(testOrders);
            DisplayedOrders = new ObservableCollection<Order>();
            LoadTestProducts();
            LoadTestContacts();
            UpdatePagination();
        }
        private void LoadTestProducts()
        {
            var testProducts = new List<Product>();
            var productNames = new[] { "Laptop", "Smartphone", "Tablet", "Headphones", "Mouse", "Keyboard", "Monitor",
"Speaker", "Camera", "Printer", "Scanner", "Router", "Cable", "Adapter", "Charger" };
            var random = new Random();
            for (int i = 1; i <= 15; i++)
            {
                var productName = productNames[i - 1];
                var price = Math.Round(random.NextDouble() * 50 + 5, 2);
                var cost = Math.Round(price * 0.6, 2);
                testProducts.Add(new Product
                {
                    Id = i,
                    Name = productName,
                    Price = price,
                    Cost = cost,
                    Stock = random.Next(1, 50),
                    CategoryName = "Electronics",
                    DateAdded = DateTime.Now
                });
            }
            Products = new ObservableCollection<Product>(testProducts);
        }
        private void LoadTestContacts()
        {
            var testContacts = new List<Contact>();
            var contactNames = new[] { "Sophie Carter", "Owen Bennett", "Olivia Hayes", "Jackson Hayes", "Emma Wilson",
"Liam Davis", "Ava Miller", "Noah Garcia", "Mia Anderson", "Ethan Thompson",
"Isabella Martinez", "William Johnson", "Sophia Brown", "James Wilson", "Charlotte Davis" };
            var random = new Random();
            for (int i = 1; i <= 15; i++)
            {
                testContacts.Add(new Contact
                {
                    Id = i,
                    Name = contactNames[i - 1],
                    Email = $"{contactNames[i - 1].ToLower().Replace(" ", ".")}@example.com",
                    PhoneNumber = $"+1-555-{random.Next(100, 999)}-{random.Next(1000, 9999)}",
                    Address = $"{random.Next(100, 9999)} Main St, City {i}",
                    DateAdded = DateTime.Now
                });
            }
            Contacts = new ObservableCollection<Contact>(testContacts);
        }
        private void ShowOrderDetails(Order order)
        {
            if (order == null) return;
            IsEditMode = 0;
            UpdateViewMode();
        }
        private void ShowEmptyState()
        {
            IsEditMode = 0;
            UpdateViewMode();
        }
        private void UpdateViewMode()
        {
            if (IsEditMode == 1)
            {
                ViewMode = 2;
            }
            else if (SelectedOrder != null)
            {
                ViewMode = 1;
            }
            else
            {
                ViewMode = 0;
            }
        }
        private void SelectOrder(Order order)
        {
            SelectedOrder = order;
        }
        private void ApplyFilters()
        {
            if (Orders == null) return;
            var filtered = Orders.AsEnumerable();
            if (!string.IsNullOrEmpty(SearchText))
            {
                filtered = filtered.Where(o =>
                o.CustomerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                o.Id.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }
            if (SelectedStatusFilter != "All")
            {
                bool isConfirmed = SelectedStatusFilter == "Completed";
                filtered = filtered.Where(o => o.IsConfirmed == isConfirmed);
                IsStatusFilterActive = true;
            }
            else
            {
                IsStatusFilterActive = false;
            }
            if (FilterStartDate.HasValue || FilterEndDate.HasValue)
            {
                if (FilterStartDate.HasValue && FilterEndDate.HasValue)
                {
                    filtered = filtered.Where(o => o.Date.Date >= FilterStartDate.Value.Date && o.Date.Date <= FilterEndDate.Value.Date);
                }
                else if (FilterStartDate.HasValue)
                {
                    filtered = filtered.Where(o => o.Date.Date >= FilterStartDate.Value.Date);
                }
                else if (FilterEndDate.HasValue)
                {
                    filtered = filtered.Where(o => o.Date.Date <= FilterEndDate.Value.Date);
                }
                IsDateFilterActive = true;
            }
            else
            {
                IsDateFilterActive = false;
            }
            FilteredOrders = new ObservableCollection<Order>(filtered);
            CurrentPage = 1;
        }
        private void UpdatePagination()
        {
            if (FilteredOrders == null) return;
            TotalOrders = FilteredOrders.Count;
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)TotalOrders / PageSize));
            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
            else if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }
            UpdateDisplayedOrders();
            OnPropertyChanged(nameof(HasNextPage));
            OnPropertyChanged(nameof(HasPreviousPage));
            UpdatePaginationCommands();
        }
        private void UpdateDisplayedOrders()
        {
            if (FilteredOrders == null) return;
            var startIndex = (CurrentPage - 1) * PageSize;
            var itemsToTake = Math.Min(PageSize, FilteredOrders.Count - startIndex);
            if (startIndex >= FilteredOrders.Count)
            {
                DisplayedOrders = new ObservableCollection<Order>();
                return;
            }
            var displayed = FilteredOrders.Skip(startIndex).Take(itemsToTake);
            DisplayedOrders = new ObservableCollection<Order>(displayed);
        }
        private async void NextPage()
        {
            if (HasNextPage)
            {
                CurrentPage++;
                await ShowPaginationInfo();
            }
        }
        private async void PreviousPage()
        {
            if (HasPreviousPage)
            {
                CurrentPage--;
                await ShowPaginationInfo();
            }
        }
        private async void FirstPage()
        {
            CurrentPage = 1;
            await ShowPaginationInfo();
        }
        private async void LastPage()
        {
            CurrentPage = TotalPages;
            await ShowPaginationInfo();
        }

        private void UpdatePaginationCommands()
        {
            if (NextPageCommand is Command nextCmd)
                nextCmd.ChangeCanExecute();
            if (PreviousPageCommand is Command prevCmd)
                prevCmd.ChangeCanExecute();
            if (FirstPageCommand is Command firstCmd)
                firstCmd.ChangeCanExecute();
            if (LastPageCommand is Command lastCmd)
                lastCmd.ChangeCanExecute();
        }
        private void ClearFilters()
        {
            SearchText = "";
            SelectedStatusFilter = "All";
            FilterStartDate = null;
            FilterEndDate = null;
            IsDateFilterActive = false;
            IsStatusFilterActive = false;
            DateFilterSummary = string.Empty;
            ApplyFilters();
        }
        private void LoadOrderDetails(Order order)
        {
            if (order == null) return;
            EditCustomerName = order.CustomerName;
            EditDate = order.Date;
            EditIsConfirmed = order.IsConfirmed;
            EditTotalAmount = order.TotalAmount;
            IsEditMode = 1;
        }

        private void ClearNewOrderForm()
        {
            NewOrderId = Orders.Count + 1;
            NewOrderCustomerName = "";
            NewOrderDate = DateTime.Now;
            NewOrderIsConfirmed = false;
            NewOrderTotalAmount = 0;
            NewOrderSubTotalAmount = 0;
            NewOrderDiscount = 0;
            NewOrderTaxRate = 2.0;
            NewOrderLines = new ObservableCollection<TransactionLine>();
            UseExistingContact = false;
            SelectedContact = null;
            SelectedProduct = null;
            SelectedProductQuantity = 1;
        }
        private bool ValidateNewOrder()
        {
            return !string.IsNullOrWhiteSpace(NewOrderCustomerName) &&
            NewOrderTotalAmount > 0;
        }
        private void AddNewOrderToCollection()
        {
            var newOrder = new Order
            {
                Id = NewOrderId,
                CustomerName = NewOrderCustomerName,
                Date = NewOrderDate,
                IsConfirmed = NewOrderIsConfirmed,
                TotalAmount = NewOrderTotalAmount,
                Discount = NewOrderDiscount,
                Tax = NewOrderTaxRate,
                ItemCount = NewOrderLines?.Count ?? 0,
                Lines = NewOrderLines?.ToList() ?? new List<TransactionLine>()
            };
            Orders.Add(newOrder);
        }
        private async void ShowSuccessSnackbar(string message)
        {
            var snackbar = Snackbar.Make(message, duration: TimeSpan.FromSeconds(2));
            await snackbar.Show();
        }

        private void CancelEdit()
        {
            IsEditMode = 0;
            _editingOrder = null;
            EditCustomerName = "";
            EditDate = DateTime.Now;
            EditIsConfirmed = false;
            EditTotalAmount = 0;
        }
        private void RemoveProductFromOrder(TransactionLine line)
        {
            try
            {
                if (NewOrderLines != null && line != null)
                {
                    NewOrderLines.Remove(line);
                    UpdateNewOrderTotal();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void UpdateNewOrderTotal()
        {
            if (NewOrderLines != null)
            {
                NewOrderSubTotalAmount = NewOrderLines.Sum(p => p.Price * p.Stock);
                NewOrderTotalAmount = NewOrderSubTotalAmount - NewOrderDiscount + (NewOrderSubTotalAmount * NewOrderTaxRate / 100);
            }
        }
        private void AddSelectedProductToOrder()
        {
            if (SelectedProduct != null && SelectedProductQuantity > 0)
            {
                if (NewOrderLines == null)
                {
                    NewOrderLines = new ObservableCollection<TransactionLine>();
                }
                var lineToAdd = new TransactionLine
                {
                    Id = DateTime.Now.Millisecond,
                    Name = SelectedProduct.Name,
                    Price = SelectedProduct.Price,
                    Cost = SelectedProduct.Cost,
                    Stock = SelectedProductQuantity,
                    CategoryName = SelectedProduct.CategoryName,
                    DateAdded = DateTime.Now,
                    ProductId = SelectedProduct.Id
                };
                NewOrderLines.Add(lineToAdd);
                UpdateNewOrderTotal();
                SelectedProduct = null;
                SelectedProductQuantity = 1;
            }
        }
        private void IncreaseTaxRate()
        {
            NewOrderTaxRate += 0.5;
        }
        private void DecreaseTaxRate()
        {
            if (NewOrderTaxRate > 0)
            {
                NewOrderTaxRate -= 0.5;
            }
        }
        private async Task ShowDatePicker()
        {

            var tempTransactionVM = new TransactionVM();
            var popup = new DatePickerPopup(tempTransactionVM);
            var result = await Application.Current.MainPage.ShowPopupAsync(popup);
            if (popup.BindingContext is DatePickerVM datePickerVM)
            {
                FilterStartDate = datePickerVM.StartDate;
                FilterEndDate = datePickerVM.EndDate;
                if (FilterStartDate.HasValue && FilterEndDate.HasValue)
                {
                    if (FilterStartDate.Value.Date == FilterEndDate.Value.Date)
                    {
                        DateFilterSummary = $"Filtering orders for {FilterStartDate.Value:MMM dd, yyyy}";
                    }
                    else
                    {
                        DateFilterSummary = $"Filtering orders from {FilterStartDate.Value:MMM dd} to {FilterEndDate.Value:MMM dd, yyyy}";
                    }
                }
                else if (FilterStartDate.HasValue)
                {
                    DateFilterSummary = $"Filtering orders from {FilterStartDate.Value:MMM dd, yyyy}";
                }
                else if (FilterEndDate.HasValue)
                {
                    DateFilterSummary = $"Filtering orders until {FilterEndDate.Value:MMM dd, yyyy}";
                }
                ApplyFilters();
            }

        }
        #endregion

        #region OnPropertyChanged interface
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Tasks

        private async Task ShowPaginationInfo()
        {
            var startItem = (CurrentPage - 1) * PageSize + 1;
            var endItem = Math.Min(CurrentPage * PageSize, TotalOrders);
            var message = $"Showing orders {startItem}-{endItem} of {TotalOrders}";
            var snackbar = Snackbar.Make(message, duration: TimeSpan.FromSeconds(1.5));
            await snackbar.Show();
        }

        private async Task AddNewOrder()
        {
            try
            {
                ClearNewOrderForm();
                var popup = new AddNewOrderPopup();
                popup.BindingContext = this;
                await Application.Current.MainPage.ShowPopupAsync(popup);
                if (popup.Result == AddNewOrderResult.Add)
                {
                    if (ValidateNewOrder())
                    {
                        var orderName = NewOrderCustomerName;
                        AddNewOrderToCollection();
                        ApplyFilters();
                        ShowSuccessSnackbar($"Order for '{orderName}' added successfully");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Validation Error",
                        "Please fill in all required fields correctly.", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error adding order: {ex.Message}",
                duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }

        private async Task EditOrder(Order order)
        {
            try
            {
                _editingOrder = order;
                LoadOrderDetails(order);
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error editing order: {ex.Message}",
                duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        private async Task DeleteOrder(Order order)
        {
            try
            {
                Orders.Remove(order);
                ApplyFilters();
                var snackbar = Snackbar.Make($"Deleted order #{order.Id}",
                duration: TimeSpan.FromSeconds(2));
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error deleting order: {ex.Message}",
                duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        private async Task SaveOrder()
        {
            try
            {
                if (_editingOrder != null)
                {
                    var orderId = _editingOrder.Id;
                    _editingOrder.CustomerName = EditCustomerName;
                    _editingOrder.Date = EditDate;
                    _editingOrder.IsConfirmed = EditIsConfirmed;
                    _editingOrder.TotalAmount = EditTotalAmount;
                    ApplyFilters();
                    CancelEdit();
                    var snackbar = Snackbar.Make($"Order #{orderId} saved successfully",
                    duration: TimeSpan.FromSeconds(2));
                    await snackbar.Show();
                }
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error saving order: {ex.Message}",
                duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }

        private async Task RefreshOrders()
        {
            try
            {
                IsRefreshing = true;
                await Task.Delay(1000);
                ApplyFilters();
                IsRefreshing = false;
                var totalConfirmed = Orders.Count(o => o.IsConfirmed);
                var totalPending = Orders.Count(o => !o.IsConfirmed);
                var totalRevenue = Orders.Where(o => o.IsConfirmed).Sum(o => o.TotalAmount);
                var message = $"Orders refreshed - {totalConfirmed} confirmed, {totalPending} pending, ${totalRevenue:N2} total revenue";
                var snackbar = Snackbar.Make(message, duration: TimeSpan.FromSeconds(2));
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                var snackbar = Snackbar.Make($"Error refreshing orders: {ex.Message}",
                duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        private async Task ConfirmOrder(Order order)
        {
            try
            {
                order.IsConfirmed = true;
                ApplyFilters();
                var snackbar = Snackbar.Make($"Order #{order.Id} confirmed",
                duration: TimeSpan.FromSeconds(2));
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error confirming order: {ex.Message}",
                duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        private async Task AddProductToOrder()
        {
            try
            {
                var sampleLine = new TransactionLine
                {
                    Id = NewOrderLines?.Count + 1 ?? 1,
                    Name = $"Product {NewOrderLines?.Count + 1 ?? 1}",
                    Price = 25.99,
                    Cost = 15.50,
                    Stock = 1,
                    CategoryName = "Electronics",
                    DateAdded = DateTime.Now
                };
                if (NewOrderLines == null)
                {
                    NewOrderLines = new ObservableCollection<TransactionLine>();
                }
                NewOrderLines.Add(sampleLine);
                UpdateNewOrderTotal();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error adding product: {ex.Message}",
                duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        #endregion

    }
}
