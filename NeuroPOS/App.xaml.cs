using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using Syncfusion.Licensing;
using System.Diagnostics;
using Contact = NeuroPOS.MVVM.Model.Contact;

namespace NeuroPOS
{
    public partial class App : Application
    {
        #region injection
        public static BaseRepository<CashRegister>? CashRegisterRepo { get; private set; }
        public static BaseRepository<Category>? CategoryRepo { get; private set; }
        public static BaseRepository<Contact>? ContactRepo { get; private set; }
        public static BaseRepository<Product>? ProductRepo { get; private set; }
        public static BaseRepository<Transaction>? TransactionRepo { get; private set; }
        public static BaseRepository<TransactionLine>? TransactionLineRepo { get; private set; }
        public static BaseRepository<Order>? OrderRepo { get; private set; }
        public static HomeVM? HomeVM { get; set; }
        public static TransactionVM? TransactionVM { get; set; }
        public static InventoryVM? InventoryVM { get; set; }
        public static OrderVM? OrderVM { get; set; }
        public static ContactVM? ContactVM { get; set; }
        public static CashRegisterVM? CashRegisterVM { get; set; }
        #endregion

        public App(BaseRepository<CashRegister> _cashregister,
            BaseRepository<Category> _category,
            BaseRepository<Contact> _contact,
            BaseRepository<Product> _product,
            BaseRepository<Transaction> _transaction,
            BaseRepository<TransactionLine> _transactionLine,
            BaseRepository<Order> _order, HomeVM _homeVM,
            TransactionVM _transactionVM, InventoryVM _inventoryVM,
            ContactVM _contactVM, OrderVM _orderVM,
            CashRegisterVM _cashRegisterVM)
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
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        #region Test Data
      

       






        #endregion


    }

}

