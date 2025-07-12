using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using Syncfusion.Licensing;
using Contact = NeuroPOS.MVVM.Model.Contact;

namespace NeuroPOS
{
    public partial class App : Application
    {
        #region injection
        public static BaseRepository<Cart>? CartRepo { get; private set; }
        public static BaseRepository<CashRegister>? CashRegisterRepo { get; private set; }
        public static BaseRepository<Category>? CategoryRepo { get; private set; }
        public static BaseRepository<Contact>? ContactRepo { get; private set; }
        public static BaseRepository<Person>? PersonRepo { get; private set; }
        public static BaseRepository<Product>? ProductRepo { get; private set; }
        public static BaseRepository<Transaction>? TransactionRepo { get; private set; }
        public static BaseRepository<Order>? OrderRepo { get; private set; }

        public static HomeVM? HomeRepo { get; set; }
        public static TransactionVM? TransactionRepoVM { get; set; }
        public static InventoryVM? InventoryRepo { get; set; }
        #endregion
        public App(BaseRepository<Cart> _cart, BaseRepository<CashRegister> _cashregister,
            BaseRepository<Category> _category,
            BaseRepository<Contact> _contact, BaseRepository<Person> _person,
            BaseRepository<Product> _product,
            BaseRepository<Transaction> _transaction, BaseRepository<Order> _order ,HomeVM _homeVM,
            TransactionVM _transactionVM,InventoryVM _inventoryVM)
        {
            InitializeComponent();
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXlceHRTQ2ZYWUN/XkFWYEk=");
            CartRepo = _cart;
            CashRegisterRepo = _cashregister;
            CategoryRepo = _category;
            ContactRepo = _contact;
            PersonRepo = _person;
            ProductRepo = _product;
            TransactionRepo = _transaction;
            OrderRepo = _order;
            HomeRepo = _homeVM;
            TransactionRepoVM = _transactionVM;
            InventoryRepo = _inventoryVM;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}