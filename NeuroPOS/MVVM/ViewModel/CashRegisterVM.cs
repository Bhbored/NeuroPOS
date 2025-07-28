using NeuroPOS.MVVM.Model;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class CashRegisterVM : INotifyPropertyChanged
    {
        private CashRegister _cashRegister = new CashRegister();
        private ObservableCollection<Transaction> _transactions = new ObservableCollection<Transaction>();
        private string _selectedFilter = "All";
        private bool _isNewestFirst = true; // true = newest first, false = oldest first

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

        public bool IsNewestFirst
        {
            get => _isNewestFirst;
            set
            {
                _isNewestFirst = value;
                OnPropertyChanged(nameof(IsNewestFirst));
                FilterTransactions();
            }
        }

        #endregion

        #region Commands

        public ICommand FilterCommand => new Command<string>(filter => SelectedFilter = filter);
        public ICommand ToggleSortCommand => new Command(() => IsNewestFirst = !IsNewestFirst);

        #endregion

        #region Methods

        public async Task LoadDB()
        {
            var dbTransactions = App.TransactionRepo.GetItemsWithChildren();
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                CashRegister = new CashRegister();
                Transactions.Clear();
            });

            foreach (var item in dbTransactions)
            {
                Transactions.Add(item);
            }

            // Set the transactions list in CashRegister to trigger property change notifications
            CashRegister.Transactions = new List<Transaction>(Transactions);

            // Trigger property change notifications for calculated properties
            OnPropertyChanged(nameof(CashRegister));
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

            // Apply sorting
            if (_isNewestFirst)
            {
                filteredTransactions = filteredTransactions.OrderByDescending(t => t.Date);
            }
            else
            {
                filteredTransactions = filteredTransactions.OrderBy(t => t.Date);
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
