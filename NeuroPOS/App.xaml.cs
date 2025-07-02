using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
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
        #endregion
        public App(BaseRepository<Cart> _cart, BaseRepository<CashRegister> _cashregister, BaseRepository<Category> _category,
            BaseRepository<Contact> _contact, BaseRepository<Person> _person, BaseRepository<Product> _product,
            BaseRepository<Transaction> _transaction)
        {
            InitializeComponent();
            CartRepo = _cart;
            CashRegisterRepo = _cashregister;
            CategoryRepo = _category;
            ContactRepo = _contact;
            PersonRepo = _person;
            ProductRepo = _product;
            TransactionRepo = _transaction;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}