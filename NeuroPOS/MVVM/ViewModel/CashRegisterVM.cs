using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NeuroPOS.MVVM.Model;

namespace NeuroPOS.MVVM.ViewModel
{
    public class CashRegisterVM : INotifyPropertyChanged
    {
        private CashRegister _cashRegister;
        private ObservableCollection<Transaction> _transactions;
        private string _selectedFilter = "All";

        public CashRegisterVM()
        {
            InitializeData();
            InitializeCommands();
        }

        #region Properties

        public CashRegister CashRegister
        {
            get => _cashRegister;
            set
            {
                _cashRegister = value;
                OnPropertyChanged(nameof(CashRegister));
            }
        }

        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set
            {
                _transactions = value;
                OnPropertyChanged(nameof(Transactions));
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
                FilterTransactions();
            }
        }

        #endregion

        #region Commands

        public ICommand FilterCommand { get; private set; }

        private void InitializeCommands()
        {
            FilterCommand = new Command<string>(filter => SelectedFilter = filter);
        }

        #endregion

        #region Methods

        private void InitializeData()
        {
            // Initialize CashRegister with test data
            _cashRegister = new CashRegister();

            // Create test transactions that match the image values
            var testTransactions = new List<Transaction>
            {
                new Transaction
                {
                    Date = DateTime.Now.AddDays(-1),
                    TransactionType = "sell",
                    TotalAmount = 250.00,
                    IsPaid = true,
                    Lines = new List<TransactionLine>
                    {
                        new TransactionLine { Stock = 1, Price = 250.00 }
                    }
                },
                new Transaction
                {
                    Date = DateTime.Now.AddDays(-1),
                    TransactionType = "sell",
                    TotalAmount = 180.00,
                    IsPaid = true,
                    Lines = new List<TransactionLine>
                    {
                        new TransactionLine { Stock = 1, Price = 180.00 }
                    }
                },
                new Transaction
                {
                    Date = DateTime.Now.AddDays(-2),
                    TransactionType = "buy",
                    TotalAmount = 300.00,
                    IsPaid = false,
                    Lines = new List<TransactionLine>
                    {
                        new TransactionLine { Stock = 1, Cost = 300.00 }
                    }
                },
                new Transaction
                {
                    Date = DateTime.Now.AddDays(-2),
                    TransactionType = "sell",
                    TotalAmount = 220.00,
                    IsPaid = true,
                    Lines = new List<TransactionLine>
                    {
                        new TransactionLine { Stock = 1, Price = 220.00 }
                    }
                },
                new Transaction
                {
                    Date = DateTime.Now.AddDays(-3),
                    TransactionType = "buy",
                    TotalAmount = 400.00,
                    IsPaid = true,
                    Lines = new List<TransactionLine>
                    {
                        new TransactionLine { Stock = 1, Cost = 400.00 }
                    }
                },
                new Transaction
                {
                    Date = DateTime.Now.AddDays(-4),
                    TransactionType = "sell",
                    TotalAmount = 150.00,
                    IsPaid = true,
                    Lines = new List<TransactionLine>
                    {
                        new TransactionLine { Stock = 1, Price = 150.00 }
                    }
                },
                new Transaction
                {
                    Date = DateTime.Now.AddDays(-5),
                    TransactionType = "buy",
                    TotalAmount = 500.00,
                    IsPaid = false,
                    Lines = new List<TransactionLine>
                    {
                        new TransactionLine { Stock = 1, Cost = 500.00 }
                    }
                }
            };

            // Add transactions to cash register
            _cashRegister.Transactions = testTransactions;

            // Initialize collections
            _transactions = new ObservableCollection<Transaction>(testTransactions);
        }

        private void FilterTransactions()
        {
            var filteredTransactions = _cashRegister.Transactions.AsEnumerable();

            switch (_selectedFilter)
            {
                case "Paid":
                    filteredTransactions = filteredTransactions.Where(t => t.IsPaid);
                    break;
                case "Pending":
                    filteredTransactions = filteredTransactions.Where(t => !t.IsPaid);
                    break;
                default:
                    // "All" - no filtering needed
                    break;
            }

            Transactions = new ObservableCollection<Transaction>(filteredTransactions);
        }

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
