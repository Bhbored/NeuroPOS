using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using NeuroPOS.MVVM.Model;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using PropertyChanged;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class OrderVM : INotifyPropertyChanged
    {
        private ObservableCollection<Order> _orders;
        private ObservableCollection<Order> _filteredOrders;
        private ObservableCollection<Order> _displayedOrders;
        private Order _selectedOrder;
        private string _searchText = "";
        private bool _isRefreshing = false;
        private int _isEditMode = 0; // 0 = not editing, 1 = editing
        private Order _editingOrder;
        private bool _isLoading = false;

        // Pagination properties
        private int _currentPage = 1;
        private int _pageSize = 10; // Fixed page size
        private int _totalPages = 1;
        private int _totalOrders = 0;

        // Filter properties
        private string _selectedStatusFilter = "All";
        private DateTime _selectedDateFilter = DateTime.Now;

        // Edit properties
        private string _editCustomerName = "";
        private DateTime _editDate = DateTime.Now;
        private bool _editIsConfirmed = false;
        private double _editTotalAmount = 0;

        public OrderVM()
        {
            InitializeCommands();
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
                    LoadOrderDetails(value);
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

        // Pagination properties
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
                // Page size is fixed at 10, no changes allowed
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

        // Filter properties
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

        public DateTime SelectedDateFilter
        {
            get => _selectedDateFilter;
            set
            {
                _selectedDateFilter = value;
                OnPropertyChanged(nameof(SelectedDateFilter));
                ApplyFilters();
            }
        }

        // Edit properties
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

        #endregion

        #region Commands

        public ICommand AddNewOrderCommand { get; private set; }
        public ICommand EditOrderCommand { get; private set; }
        public ICommand DeleteOrderCommand { get; private set; }
        public ICommand SaveOrderCommand { get; private set; }
        public ICommand CancelEditCommand { get; private set; }
        public ICommand RefreshOrdersCommand { get; private set; }
        public ICommand NextPageCommand { get; private set; }
        public ICommand PreviousPageCommand { get; private set; }
        public ICommand FirstPageCommand { get; private set; }
        public ICommand LastPageCommand { get; private set; }
        public ICommand ConfirmOrderCommand { get; private set; }
        public ICommand ClearFiltersCommand { get; private set; }

        private void InitializeCommands()
        {
            AddNewOrderCommand = new Command(async () => await AddNewOrder());
            EditOrderCommand = new Command<Order>(async (order) => await EditOrder(order));
            DeleteOrderCommand = new Command<Order>(async (order) => await DeleteOrder(order));
            SaveOrderCommand = new Command(async () => await SaveOrder());
            CancelEditCommand = new Command(() => CancelEdit());
            RefreshOrdersCommand = new Command(async () => await RefreshOrders());
            NextPageCommand = new Command(() => NextPage(), () => HasNextPage);
            PreviousPageCommand = new Command(() => PreviousPage(), () => HasPreviousPage);
            FirstPageCommand = new Command(() => FirstPage(), () => HasPreviousPage);
            LastPageCommand = new Command(() => LastPage(), () => HasNextPage);
            ConfirmOrderCommand = new Command<Order>(async (order) => await ConfirmOrder(order));
            ClearFiltersCommand = new Command(() => ClearFilters());
        }

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

            // Generate 50 test orders
            for (int i = 1; i <= 50; i++)
            {
                var orderDate = startDate.AddDays(random.Next(0, 365));
                var customerName = customerNames[random.Next(customerNames.Length)];
                var isConfirmed = random.Next(2) == 1;
                var itemCount = random.Next(1, 6);
                var totalAmount = Math.Round(random.NextDouble() * 200 + 10, 2);

                var orderItems = new List<Product>();
                for (int j = 0; j < itemCount; j++)
                {
                    var productName = productNames[random.Next(productNames.Length)];
                    var price = Math.Round(random.NextDouble() * 50 + 5, 2);
                    var quantity = random.Next(1, 4);

                    orderItems.Add(new Product
                    {
                        Id = i * 100 + j,
                        Name = productName,
                        Price = price,
                        Stock = quantity
                    });
                }

                testOrders.Add(new Order
                {
                    Id = i,
                    Date = orderDate,
                    CustomerName = customerName,
                    IsConfirmed = isConfirmed,
                    TotalAmount = totalAmount,
                    ItemCount = itemCount,
                    OrderItems = orderItems
                });
            }

            Orders = new ObservableCollection<Order>(testOrders);
            FilteredOrders = new ObservableCollection<Order>(testOrders);
            DisplayedOrders = new ObservableCollection<Order>();
            UpdatePagination();
        }

        private void ApplyFilters()
        {
            if (Orders == null) return;

            var filtered = Orders.AsEnumerable();

            // Apply search filter
            if (!string.IsNullOrEmpty(SearchText))
            {
                filtered = filtered.Where(o =>
                    o.CustomerName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                    o.Id.ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            // Apply status filter
            if (SelectedStatusFilter != "All")
            {
                bool isConfirmed = SelectedStatusFilter == "Confirmed";
                filtered = filtered.Where(o => o.IsConfirmed == isConfirmed);
            }

            // Apply date filter
            filtered = filtered.Where(o => o.Date.Date == SelectedDateFilter.Date);

            FilteredOrders = new ObservableCollection<Order>(filtered);

            // Reset to first page when filters are applied
            CurrentPage = 1;
        }

        private void UpdatePagination()
        {
            if (FilteredOrders == null) return;

            TotalOrders = FilteredOrders.Count;
            TotalPages = Math.Max(1, (int)Math.Ceiling((double)TotalOrders / PageSize));

            // Ensure current page is within valid range
            if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
            }
            else if (CurrentPage < 1)
            {
                CurrentPage = 1;
            }

            UpdateDisplayedOrders();

            // Notify command availability changes
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

        private async Task ShowPaginationInfo()
        {
            var startItem = (CurrentPage - 1) * PageSize + 1;
            var endItem = Math.Min(CurrentPage * PageSize, TotalOrders);
            var message = $"Showing orders {startItem}-{endItem} of {TotalOrders}";

            var snackbar = Snackbar.Make(message, duration: TimeSpan.FromSeconds(1.5));
            await snackbar.Show();
        }

        private void UpdatePaginationCommands()
        {
            // Force command re-evaluation
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
            SelectedDateFilter = DateTime.Now;
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

        private async Task AddNewOrder()
        {
            var snackbar = Snackbar.Make("Add New Order functionality will be implemented",
                duration: TimeSpan.FromSeconds(2));
            await snackbar.Show();
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
                    _editingOrder.CustomerName = EditCustomerName;
                    _editingOrder.Date = EditDate;
                    _editingOrder.IsConfirmed = EditIsConfirmed;
                    _editingOrder.TotalAmount = EditTotalAmount;

                    ApplyFilters();
                    CancelEdit();

                    var snackbar = Snackbar.Make($"Order #{_editingOrder.Id} saved successfully",
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

        private void CancelEdit()
        {
            IsEditMode = 0;
            _editingOrder = null;
            EditCustomerName = "";
            EditDate = DateTime.Now;
            EditIsConfirmed = false;
            EditTotalAmount = 0;
        }

        private async Task RefreshOrders()
        {
            try
            {
                IsRefreshing = true;
                await Task.Delay(1000); // Simulate refresh
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

        #endregion

        #region Static Resources

        public static List<string> StatusFilters => new List<string> { "All", "Confirmed", "Pending" };

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
