using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.Services;
using Syncfusion.Licensing;
using System.Diagnostics;
using Contact = NeuroPOS.MVVM.Model.Contact;

namespace NeuroPOS
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        #region injection
        public static BaseRepository<CashRegister>? CashRegisterRepo { get; private set; }
        public static BaseRepository<Category>? CategoryRepo { get; private set; }
        public static BaseRepository<Contact>? ContactRepo { get; private set; }
        public static BaseRepository<Product>? ProductRepo { get; private set; }
        public static BaseRepository<Transaction>? TransactionRepo { get; private set; }
        public static BaseRepository<TransactionLine>? TransactionLineRepo { get; private set; }
        public static BaseRepository<Order>? OrderRepo { get; private set; }
        public static BaseRepository<InventorySnapshot>? InventorySnapShotRepo { get; private set; }
        public static BaseRepository<CashFlowSnapshot>? CashFlowSnapshotRepo { get ; private set; }

        public static HomeVM? HomeVM { get; set; }
        public static TransactionVM? TransactionVM { get; set; }
        public static InventoryVM? InventoryVM { get; set; }
        public static OrderVM? OrderVM { get; set; }
        public static ContactVM? ContactVM { get; set; }
        public static CashRegisterVM? CashRegisterVM { get; set; }
        public static StatisticsVM? StatisticsVM { get; set; }
        #endregion

        public App(BaseRepository<CashRegister> _cashregister,
            BaseRepository<Category> _category,
            BaseRepository<Contact> _contact,
            BaseRepository<Product> _product,
            BaseRepository<Transaction> _transaction,
            BaseRepository<TransactionLine> _transactionLine,
            BaseRepository<Order> _order,
            BaseRepository<InventorySnapshot> _snapshot ,BaseRepository<CashFlowSnapshot> _cashFlowsnap , HomeVM _homeVM,
            TransactionVM _transactionVM, InventoryVM _inventoryVM,
            ContactVM _contactVM, OrderVM _orderVM,
            CashRegisterVM _cashRegisterVM, StatisticsVM _statisticsVM, AuthService authService)
        {
            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXlceHRTQ2ZYWUN/XkFWYEk=");
            CashRegisterRepo = _cashregister;
            CategoryRepo = _category;
            ContactRepo = _contact;
            ProductRepo = _product;
            TransactionRepo = _transaction;
            TransactionLineRepo = _transactionLine;
            OrderRepo = _order;
            HomeVM = _homeVM;
            TransactionVM = _transactionVM;
            InventoryVM = _inventoryVM;
            OrderVM = _orderVM;
            ContactVM = _contactVM;
            CashRegisterVM = _cashRegisterVM;
            StatisticsVM = _statisticsVM;
            InventorySnapShotRepo = _snapshot;
            CashFlowSnapshotRepo = _cashFlowsnap;
            _authService = authService;
            SaveInventorySnapshotIfNeeded();
            SaveCashFlowSnapshotIfNeeded();
            //var populator = new TestDataPopulator();
            //populator.PopulateAllTestData();
            // SeedCashFlowSnapshots();
            //_ = SeedSnapshotsAsync();

        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var authService = MauiProgram.Host.Services.GetRequiredService<AuthService>();

            var window = new Window(new AppShell(authService));

            Task.Run(async () => await HandleStartAsync(authService));

            return window;
        }

        #region Snapshot

        public void SaveInventorySnapshotIfNeeded()
        {
            var now = DateTime.Now;
            var lastSnapshot = App.InventorySnapShotRepo
                .GetItems()
                .OrderByDescending(x => x.Date)
                .FirstOrDefault();

            if (lastSnapshot != null && (now - lastSnapshot.Date).TotalHours < 24)
            {
                Debug.WriteLine($"Inventory snapshot too recent: {lastSnapshot.Date}");
                return;
            }

            var products = App.ProductRepo.GetItems();
            double total = products.Sum(p => p.Stock * p.Price);

            var snapshot = new InventorySnapshot
            {
                Date = now,
                TotalValue = total
            };

            App.InventorySnapShotRepo.InsertItem(snapshot);
            Debug.WriteLine($"[INVENTORY SNAPSHOT] {now}: {total}");
        }

        public void SaveCashFlowSnapshotIfNeeded()
        {
            var now = DateTime.Now;
            var lastSnapshot = App.CashFlowSnapshotRepo
                .GetItems()
                .OrderByDescending(x => x.Date)
                .FirstOrDefault();

            if (lastSnapshot != null && (now - lastSnapshot.Date).TotalHours < 24)
            {
                Debug.WriteLine($"Cash flow snapshot too recent: {lastSnapshot.Date}");
                return;
            }

            var startTime = lastSnapshot?.Date ?? DateTime.MinValue;
            var endTime = now;

            var transactions = App.TransactionRepo.GetItems();
            double totalFlow = 0;

            foreach (var t in transactions)
            {
                if (t.Date >= startTime && t.Date <= endTime)
                {
                    if (t.TransactionType == "sell" && t.IsPaid)
                        totalFlow += t.TotalAmount;
                    else if (t.TransactionType == "buy")
                        totalFlow -= t.TotalAmount;
                }
            }

            var snapshot = new CashFlowSnapshot
            {
                Date = now,
                TotalValue = totalFlow
            };

            App.CashFlowSnapshotRepo.InsertItem(snapshot);
            Debug.WriteLine($"[CASH FLOW SNAPSHOT] {now}: {totalFlow}");
        }

        #endregion

        #region seed snapshots

        public async Task SeedSnapshotsAsync()
        {
            var random = new Random();
            var today = DateTime.Today;

            for (int i = 0; i < 20; i++)
            {
                var snapshotDate = today.AddDays(-i);
                var snapshot = new InventorySnapshot
                {
                    Date = snapshotDate,
                    TotalValue = random.NextDouble() * 10000
                };

                var existing = App.InventorySnapShotRepo.GetItems()
                    .Where(x => x.Date == snapshotDate)
                    .FirstOrDefault();

                if (existing == null)
                {
                    App.InventorySnapShotRepo.InsertItem(snapshot);
                    Debug.WriteLine($"Inserted snapshot for {snapshotDate.ToShortDateString()}");
                }
                else
                {
                    Debug.WriteLine($"Snapshot for {snapshotDate.ToShortDateString()} already exists");
                }
            }
        }

        public void SeedCashFlowSnapshots()
        {
            var random = new Random();
            var today = DateTime.Today;

            for (int i = 0; i < 100; i++)
            {
                var date = today.AddDays(-i);
                var value = random.Next(-5000, 10000);

                var existing = App.CashFlowSnapshotRepo.GetItems().FirstOrDefault(x => x.Date == date);
                if (existing == null)
                {
                    App.CashFlowSnapshotRepo.InsertItem(new CashFlowSnapshot
                    {
                        Date = date,
                        TotalValue = value
                    });
                }
            }
        }
        #endregion

        private async Task HandleStartAsync(AuthService authService)
        {
            await Task.Delay(100);

            if (authService.IsTokenValid())
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    if (Shell.Current is AppShell appShell)
                    {
                        appShell.UpdateFlyoutItems();
                        await Shell.Current.GoToAsync("//HomePage");
                    }
                });
            }
        }


    }

}

