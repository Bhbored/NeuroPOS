using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
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
using Application = Microsoft.Maui.Controls.Application;
using Contact = NeuroPOS.MVVM.Model.Contact;
namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class OrderVM : INotifyPropertyChanged
    {
        #region Private Fields
        private ObservableCollection<Order> _orders = new ObservableCollection<Order>();
        private ObservableCollection<Order> _filteredOrders = new ObservableCollection<Order>();
        private ObservableCollection<Order> _displayedOrders = new ObservableCollection<Order>();
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
        private double _editDiscount = 0;
        private double _editTaxRate = 2.0;
        private ObservableCollection<TransactionLine> _editOrderLines;
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
        public ObservableCollection<Product> Products { get; set; } = new ObservableCollection<Product>();
        public ObservableCollection<Contact> Contacts { get; set; } = new ObservableCollection<Contact>();
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
        public double EditDiscount
        {
            get => _editDiscount;
            set
            {
                _editDiscount = value;
                OnPropertyChanged(nameof(EditDiscount));
                UpdateEditOrderTotal();
            }
        }
        public double EditTaxRate
        {
            get => _editTaxRate;
            set
            {
                _editTaxRate = value;
                OnPropertyChanged(nameof(EditTaxRate));
                UpdateEditOrderTotal();
            }
        }
        public ObservableCollection<TransactionLine> EditOrderLines
        {
            get => _editOrderLines;
            set
            {
                _editOrderLines = value;
                OnPropertyChanged(nameof(EditOrderLines));
                UpdateEditOrderTotal();
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
        public ICommand RemoveProductFromOrderCommand => new Command<TransactionLine>(line => RemoveProductFromOrder(line));
        public ICommand AddSelectedProductToOrderCommand => new Command(() => AddSelectedProductToOrder());
        public ICommand IncreaseTaxRateCommand => new Command(() => IncreaseTaxRate());
        public ICommand DecreaseTaxRateCommand => new Command(() => DecreaseTaxRate());
        public ICommand ShowDatePickerCommand => new Command(async () => await ShowDatePicker());
        public ICommand IncreaseEditLineQuantityCommand => new Command<TransactionLine>(line => IncreaseEditLineQuantity(line));
        public ICommand DecreaseEditLineQuantityCommand => new Command<TransactionLine>(line => DecreaseEditLineQuantity(line));
        public ICommand DeleteEditLineCommand => new Command<TransactionLine>(line => DeleteEditLine(line));
        #endregion

        #region Methods
        private async Task LoadDB()
        {
            var DBOrders = App.OrderRepo.GetItemsWithChildren() ?? new List<Order>();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Orders.Clear();
                FilteredOrders.Clear();
                DisplayedOrders.Clear();
            });
            foreach (var item in DBOrders)
            {
                Orders.Add(item);
            }
            FilteredOrders = new ObservableCollection<Order>(Orders);
            DisplayedOrders = new ObservableCollection<Order>();
            var DBProducts = App.ProductRepo.GetItems() ?? new List<Product>();
            var DBContacts = App.ContactRepo.GetItemsWithChildren() ?? new List<Contact>();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Products.Clear();
            });
            foreach (var item in DBProducts)
            {
                Products.Add(item);
            }
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Contacts.Clear();
            });
            foreach (var item in DBContacts)
            {
                Contacts.Add(item);
            }
            ApplyFilters();
            UpdatePagination();
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
        public void ClearFilters()
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
            EditDiscount = order.Discount;
            EditTaxRate = order.Tax;
            EditOrderLines = new ObservableCollection<TransactionLine>(order.Lines?.ToList() ?? new List<TransactionLine>());
            UpdateEditOrderTotal();
            IsEditMode = 1;
        }
        private void ClearNewOrderForm()
        {
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
            if (UseExistingContact == true)
            {
                var existingCustomer = App.ContactRepo.GetItemsWithChildren().Where(x => x.Name == NewOrderCustomerName).FirstOrDefault();
                var newOrder = new Order
                {
                    CustomerName = NewOrderCustomerName,
                    Date = NewOrderDate,
                    IsConfirmed = NewOrderIsConfirmed,
                    TotalAmount = NewOrderTotalAmount,
                    Discount = NewOrderDiscount,
                    Tax = NewOrderTaxRate,
                    ContactId = existingCustomer.Id,
                    Lines = NewOrderLines?.ToList() ?? new List<TransactionLine>()
                };
                App.OrderRepo.InsertItemWithChildren(newOrder);
                _ = RefreshOrders();
            }
            else
            {
                var newOrder = new Order
                {
                    CustomerName = NewOrderCustomerName,
                    Date = NewOrderDate,
                    IsConfirmed = NewOrderIsConfirmed,
                    TotalAmount = NewOrderTotalAmount,
                    Discount = NewOrderDiscount,
                    Tax = NewOrderTaxRate,
                    Lines = NewOrderLines?.ToList() ?? new List<TransactionLine>()
                };
                App.OrderRepo.InsertItemWithChildren(newOrder);
                _ = RefreshOrders();
            }
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
            EditDiscount = 0;
            EditTaxRate = 2.0;
            EditOrderLines = null;
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
        private void UpdateEditOrderTotal()
        {
            if (EditOrderLines != null)
            {
                var subTotal = EditOrderLines.Sum(p => p.Price * p.Stock);
                EditTotalAmount = subTotal - EditDiscount + (subTotal * EditTaxRate / 100);
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
        private void IncreaseEditLineQuantity(TransactionLine line)
        {
            if (line != null && EditOrderLines != null)
            {
                line.Stock++;
                UpdateEditOrderTotal();
            }
        }
        private void DecreaseEditLineQuantity(TransactionLine line)
        {
            if (line != null && EditOrderLines != null && line.Stock > 1)
            {
                line.Stock--;
                UpdateEditOrderTotal();
            }
        }
        private void DeleteEditLine(TransactionLine line)
        {
            if (line != null && EditOrderLines != null)
            {
                EditOrderLines.Remove(line);
                UpdateEditOrderTotal();
            }
        }
        private async Task ShowDatePicker()
        {
            var popup = new DatePickerPopup(
                async (startDate, endDate) => await ApplyDateFilter(startDate, endDate),
                "orders");
            var result = await Application.Current.MainPage.ShowPopupAsync(popup);
        }
        public async Task<bool> ApplyDateFilter(DateTime startDate, DateTime endDate)
        {
            try
            {
                FilterStartDate = startDate;
                FilterEndDate = endDate;
                IsDateFilterActive = true;
                if (startDate.Date == endDate.Date)
                {
                    DateFilterSummary = $"Showing orders for {startDate:MMM dd, yyyy}";
                }
                else
                {
                    DateFilterSummary = $"Showing orders from {startDate:MMM dd} to {endDate:MMM dd, yyyy}";
                }
                ApplyFilters();
                if (FilteredOrders.Count == 0)
                {
                    FilterStartDate = null;
                    FilterEndDate = null;
                    IsDateFilterActive = false;
                    DateFilterSummary = string.Empty;
                    ApplyFilters();
                    return false;
                }
                string successMessage;
                if (startDate.Date == endDate.Date)
                {
                    successMessage = $"Found {FilteredOrders.Count} orders for {startDate:MMM dd, yyyy}";
                }
                else
                {
                    successMessage = $"Found {FilteredOrders.Count} orders from {startDate:MMM dd} to {endDate:MMM dd, yyyy}";
                }
                var snackbarOptions = new SnackbarOptions
                {
                    BackgroundColor = Colors.Green,
                    TextColor = Colors.White
                };
                await Snackbar.Make(successMessage, null, "ok", TimeSpan.FromSeconds(3), snackbarOptions).Show();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in ApplyDateFilter: {ex.Message}");
                return false;
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
                var oldLines = order.Lines;
                App.OrderRepo.DeleteItem(order);
                await RefreshOrders();
                await Snackbar.Make(
                    $"Deleted order #{order.Id}",
                    async () => await UndoDeleteOrder(order),
                    "UNDO",
                    TimeSpan.FromSeconds(2)
                ).Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error deleting order: {ex.Message}",
                duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        private async Task UndoDeleteOrder(Order order)
        {
            App.OrderRepo.InsertItemWithChildren(order);
            await RefreshOrders();
            await Task.CompletedTask;
            await Snackbar.Make(
                  $"Restored order #{order.Id}",
                  duration: TimeSpan.FromSeconds(2)
              ).Show();
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
                    _editingOrder.Discount = EditDiscount;
                    _editingOrder.Tax = EditTaxRate;
                    _editingOrder.TotalAmount = EditTotalAmount;
                    _editingOrder.Lines = EditOrderLines?.ToList() ?? new List<TransactionLine>();
                    App.OrderRepo.UpdateItemWithChildren(_editingOrder);
                    await RefreshOrders();
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
        public async Task RefreshOrders()
        {
            try
            {
                IsRefreshing = true;
                await Task.Delay(500);
                await LoadDB();
                ApplyFilters();
                IsRefreshing = false;
                var totalConfirmed = Orders.Count(o => o.IsConfirmed);
                var totalPending = Orders.Count(o => !o.IsConfirmed);
                var totalRevenue = Orders.Where(o => o.IsConfirmed).Sum(o => o.TotalAmount);
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                Debug.WriteLine($"couldn't refresh because : {ex.Message} ");
            }
        }
        private async Task ConfirmOrder(Order order)
        {
            try
            {
                if (order.Lines == null || order.Lines.Count == 0)
                {
                    await Snackbar.Make("Order has no products. Cannot confirm empty order.",
                                        duration: TimeSpan.FromSeconds(3)).Show();
                    return;
                }
                var dbProducts = App.ProductRepo.GetItems().ToList();
                var stockValidationErrors = new List<string>();
                foreach (var orderLine in order.Lines)
                {
                    var dbProduct = dbProducts.FirstOrDefault(p => p.Name == orderLine.Name);
                    if (dbProduct == null)
                    {
                        stockValidationErrors.Add($"Product '{orderLine.Name}' not found in database");
                        continue;
                    }
                    if (dbProduct.Stock < orderLine.Stock)
                    {
                        stockValidationErrors.Add($"Insufficient stock for '{orderLine.Name}'. Available: {dbProduct.Stock}, Required: {orderLine.Stock}");
                    }
                }
                if (stockValidationErrors.Any())
                {
                    var errorMessage = "Stock validation failed:\n" + string.Join("\n", stockValidationErrors);
                    await Snackbar.Make(errorMessage, duration: TimeSpan.FromSeconds(5)).Show();
                    return;
                }
                var transaction = new Transaction
                {
                    TransactionType = "sell",
                    IsPaid = order.ContactId == 0,
                    Tax = order.CalculatedTaxAmount,
                    Discount = order.Discount,
                    TotalAmount = order.CalculatedTotalAmount,
                    ItemCount = order.Lines.Count,
                    ContactId = order.ContactId == 0 ? null : order.ContactId,
                    Lines = new List<TransactionLine>()
                };
                foreach (var orderLine in order.Lines)
                {
                    var dbProduct = dbProducts.FirstOrDefault(p => p.Name == orderLine.Name);
                    if (dbProduct != null)
                    {
                        transaction.Lines.Add(new TransactionLine
                        {
                            Name = orderLine.Name,
                            Price = orderLine.Price,
                            Stock = orderLine.Stock,
                            CategoryName = orderLine.CategoryName,
                            ImageUrl = orderLine.ImageUrl,
                            Product = dbProduct,
                            ProductId = dbProduct.Id
                        });
                        dbProduct.Stock -= orderLine.Stock;
                        App.ProductRepo.UpdateItem(dbProduct);
                    }
                }
                if (order.ContactId != 0)
                {
                    var customer = App.ContactRepo.GetItemsWithChildren()?.FirstOrDefault(c => c.Id == order.ContactId);
                    if (customer != null)
                    {
                        App.ContactRepo.AddNewChildToParentRecursively(
                            customer,
                            transaction,
                            (contact, transactions) => contact.Transactions = transactions.ToList());
                    }
                }
                else
                {
                    App.TransactionRepo.InsertItemWithChildren(transaction);
                }
                order.IsConfirmed = true;
                App.OrderRepo.UpdateItemWithChildren(order);
                await RefreshOrders();
                var message = order.ContactId == 0
                    ? $"Order #{order.Id} confirmed and transaction processed!"
                    : $"Order #{order.Id} confirmed and credit transaction created!";
                await Snackbar.Make(message, duration: TimeSpan.FromSeconds(3)).Show();
            }
            catch (Exception ex)
            {
                await Snackbar.Make($"Error confirming order: {ex.Message}",
                                    duration: TimeSpan.FromSeconds(3)).Show();
            }
        }
        #endregion

    }
}