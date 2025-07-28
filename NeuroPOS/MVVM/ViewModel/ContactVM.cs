using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.Popups;
using PropertyChanged;
using Syncfusion.Maui.DataSource;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Input;
using Contact = NeuroPOS.MVVM.Model.Contact;
using Transaction = NeuroPOS.MVVM.Model.Transaction;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class ContactVM : INotifyPropertyChanged
    {
        #region Private Fields
        private ObservableCollection<Contact> _contacts;
        private ObservableCollection<Contact> _selectedContacts;
        private ObservableCollection<Contact> _autocompleteSelectedContacts;
        private ObservableCollection<object> _selectedItems;
        private string _searchText = "";
        private bool _selectAllContacts = false;
        private bool _isRefreshing = false;
        private int _isEditMode = 0;
        private Contact _editingContact;
        private bool _isSyncingSelection = false;
        private SortDirectionState _sortState = SortDirectionState.None;
        private bool _isAddContactPopupVisible = false;
        private string _newContactName = "";
        private string _newContactEmail = "";
        private string _newContactPhoneNumber = "";
        private string _newContactAddress = "";
        private bool _isViewContactDetailsPopupVisible = false;
        private Contact _selectedContactForDetails;
        private ObservableCollection<Transaction> _pendingTransactions;
        private string _editName = "";
        private string _editEmail = "";
        private string _editPhoneNumber = "";
        private string _editAddress = "";
        private DateTime _editDateAdded = DateTime.Now;
        #endregion

        #region enums
        public enum SortDirectionState
        {
            None,
            Ascending,
            Descending
        }
        #endregion

        #region Properties
        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set
            {
                _contacts = value;
                OnPropertyChanged(nameof(Contacts));
                if (value != null)
                {
                    DataSource = new DataSource() { Source = value };
                    DataSource.Refresh();
                    if (!string.IsNullOrEmpty(SearchText))
                    {
                        FilterContacts();
                    }
                }
            }
        }
        public ObservableCollection<Contact> SelectedContacts
        {
            get => _selectedContacts;
            set
            {
                _selectedContacts = value;
                OnPropertyChanged(nameof(SelectedContacts));
                OnPropertyChanged(nameof(HasSelectedItems));
            }
        }
        public ObservableCollection<Contact> AutocompleteSelectedContacts
        {
            get => _autocompleteSelectedContacts;
            set
            {
                _autocompleteSelectedContacts = value;
                OnPropertyChanged(nameof(AutocompleteSelectedContacts));
            }
        }
        public ObservableCollection<object> SelectedItems
        {
            get => _selectedItems;
            set
            {
                if (_selectedItems != null)
                    _selectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
                _selectedItems = value;
                if (_selectedItems != null)
                    _selectedItems.CollectionChanged += SelectedItems_CollectionChanged;
                OnPropertyChanged(nameof(SelectedItems));
                OnPropertyChanged(nameof(HasSelectedItems));
                UpdateSelectedContactsFromItems();
            }
        }
        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_isSyncingSelection) return;
            _isSyncingSelection = true;
            try
            {
                UpdateSelectedContactsFromItems();
                OnPropertyChanged(nameof(HasSelectedItems));
            }
            finally
            {
                _isSyncingSelection = false;
            }
        }
        private DataSource _dataSource;
        public DataSource DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                OnPropertyChanged(nameof(DataSource));
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterContacts();
            }
        }
        public bool SelectAllContacts
        {
            get => _selectAllContacts;
            set
            {
                _selectAllContacts = value;
                OnPropertyChanged(nameof(SelectAllContacts));
                SelectAllContactsChanged();
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
        public string EditName
        {
            get => _editName;
            set
            {
                _editName = value;
                OnPropertyChanged(nameof(EditName));
            }
        }
        public string EditEmail
        {
            get => _editEmail;
            set
            {
                _editEmail = value;
                OnPropertyChanged(nameof(EditEmail));
            }
        }
        public string EditPhoneNumber
        {
            get => _editPhoneNumber;
            set
            {
                _editPhoneNumber = value;
                OnPropertyChanged(nameof(EditPhoneNumber));
            }
        }
        public string EditAddress
        {
            get => _editAddress;
            set
            {
                _editAddress = value;
                OnPropertyChanged(nameof(EditAddress));
            }
        }
        public DateTime EditDateAdded
        {
            get => _editDateAdded;
            set
            {
                _editDateAdded = value;
                OnPropertyChanged(nameof(EditDateAdded));
            }
        }
        public bool HasSelectedItems
        {
            get => (SelectedContacts?.Count > 0) || (SelectedItems?.Count > 0);
        }
        public SortDirectionState SortState
        {
            get => _sortState;
            set
            {
                if (_sortState != value)
                {
                    _sortState = value;
                    ApplySorting();
                    OnPropertyChanged(nameof(SortLabel));
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
        public bool IsAddContactPopupVisible
        {
            get => _isAddContactPopupVisible;
            set
            {
                _isAddContactPopupVisible = value;
                OnPropertyChanged(nameof(IsAddContactPopupVisible));
            }
        }
        public string NewContactName
        {
            get => _newContactName;
            set
            {
                _newContactName = value;
                OnPropertyChanged(nameof(NewContactName));
            }
        }
        public string NewContactEmail
        {
            get => _newContactEmail;
            set
            {
                _newContactEmail = value;
                OnPropertyChanged(nameof(NewContactEmail));
            }
        }
        public string NewContactPhoneNumber
        {
            get => _newContactPhoneNumber;
            set
            {
                _newContactPhoneNumber = value;
                OnPropertyChanged(nameof(NewContactPhoneNumber));
            }
        }
        public string NewContactAddress
        {
            get => _newContactAddress;
            set
            {
                _newContactAddress = value;
                OnPropertyChanged(nameof(NewContactAddress));
            }
        }
        public bool IsViewContactDetailsPopupVisible
        {
            get => _isViewContactDetailsPopupVisible;
            set
            {
                _isViewContactDetailsPopupVisible = value;
                OnPropertyChanged(nameof(IsViewContactDetailsPopupVisible));
            }
        }
        public Contact SelectedContactForDetails
        {
            get => _selectedContactForDetails;
            set
            {
                _selectedContactForDetails = value;
                OnPropertyChanged(nameof(SelectedContactForDetails));
            }
        }
        public ObservableCollection<Transaction> PendingTransactions
        {
            get => _pendingTransactions;
            set
            {
                _pendingTransactions = value;
                OnPropertyChanged(nameof(PendingTransactions));
                OnPropertyChanged(nameof(HasPendingTransactions));
            }
        }
        public bool HasPendingTransactions => PendingTransactions?.Count > 0;
        #endregion

        #region Methods
        private void InitializeDataSource()
        {
            if (Contacts != null)
            {
                DataSource = new DataSource() { Source = Contacts };
                DataSource.Refresh();
            }
        }
        private void FilterContacts()
        {
            if (DataSource != null)
            {
                DataSource.Filter = FilterBySearch;
                DataSource.RefreshFilter();
            }
        }
        private bool FilterBySearch(object item)
        {
            if (item is Contact contact)
            {
                if (string.IsNullOrEmpty(SearchText))
                    return true;
                if (SearchText.Contains(","))
                {
                    var selectedNames = SearchText.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .ToList();
                    return selectedNames.Any(name =>
                        contact.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    return contact.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                           contact.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true;
                }
            }
            return false;
        }
        private void SelectAllContactsChanged()
        {
            if (SelectAllContacts)
            {
                SelectedContacts.Clear();
                SelectedItems.Clear();
                if (DataSource?.DisplayItems != null)
                {
                    foreach (var item in DataSource.DisplayItems)
                    {
                        if (item is Contact contact)
                        {
                            SelectedContacts.Add(contact);
                            SelectedItems.Add(contact);
                        }
                    }
                }
            }
            else
            {
                SelectedContacts.Clear();
                SelectedItems.Clear();
            }
            OnPropertyChanged(nameof(HasSelectedItems));
        }
        private void ClearAllSelections()
        {
            SelectedContacts.Clear();
            SelectedItems.Clear();
            SelectAllContacts = false;
            OnPropertyChanged(nameof(HasSelectedItems));
        }
        private void UpdateSelectedContactsFromItems()
        {
            if (_isSyncingSelection) return;
            _isSyncingSelection = true;
            try
            {
                var newSelection = SelectedItems?.OfType<Contact>().ToList() ?? new List<Contact>();
                SelectedContacts.Clear();
                foreach (var contact in newSelection)
                    SelectedContacts.Add(contact);
                if (DataSource?.DisplayItems != null && DataSource.DisplayItems.Count > 0)
                {
                    var allItemsSelected = DataSource.DisplayItems.All(item =>
                        item is Contact contact && SelectedContacts.Contains(contact));
                    SelectAllContacts = allItemsSelected;
                }
                else
                {
                    SelectAllContacts = false;
                }
                OnPropertyChanged(nameof(HasSelectedItems));
            }
            finally
            {
                _isSyncingSelection = false;
            }
        }
        private void ToggleSort()
        {
            SortState = SortState switch
            {
                SortDirectionState.None => SortDirectionState.Ascending,
                SortDirectionState.Ascending => SortDirectionState.Descending,
                SortDirectionState.Descending => SortDirectionState.None,
                _ => SortDirectionState.None
            };
        }
        private void ApplySorting()
        {
            if (DataSource != null)
            {
                DataSource.SortDescriptors.Clear();
                if (SortState == SortDirectionState.Ascending)
                {
                    DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "Name",
                        Direction = Syncfusion.Maui.DataSource.ListSortDirection.Ascending
                    });
                }
                else if (SortState == SortDirectionState.Descending)
                {
                    DataSource.SortDescriptors.Add(new SortDescriptor()
                    {
                        PropertyName = "Name",
                        Direction = Syncfusion.Maui.DataSource.ListSortDirection.Descending
                    });
                }
                DataSource.Refresh();
            }
        }
        private async Task AddNewContact()
        {
            try
            {
                NewContactName = "";
                NewContactEmail = "";
                NewContactPhoneNumber = "";
                NewContactAddress = "";
                var popup = new AddContactPopup(this);
                await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error opening add contact form: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        public async Task ConfirmAddContact()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewContactName))
                {
                    var snackbar = Snackbar.Make("Contact name is required",
                        duration: TimeSpan.FromSeconds(2));
                    await snackbar.Show();
                    return;
                }
                var newContact = new Contact
                {
                    Name = NewContactName.Trim(),
                    Email = string.IsNullOrWhiteSpace(NewContactEmail) ? null : NewContactEmail.Trim(),
                    PhoneNumber = string.IsNullOrWhiteSpace(NewContactPhoneNumber) ? null : NewContactPhoneNumber.Trim(),
                    Address = string.IsNullOrWhiteSpace(NewContactAddress) ? null : NewContactAddress.Trim(),
                    DateAdded = DateTime.Now,
                    Transactions = new List<Transaction>()
                };
                App.ContactRepo.InsertItem(newContact);
                DataSource.Refresh();
                NewContactName = "";
                NewContactEmail = "";
                NewContactPhoneNumber = "";
                NewContactAddress = "";
                IsAddContactPopupVisible = false;
                var successSnackbar = Snackbar.Make($"Contact '{newContact.Name}' added successfully",
                    duration: TimeSpan.FromSeconds(2));
                await successSnackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error adding contact: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        public void CancelAddContact()
        {
            try
            {
                NewContactName = "";
                NewContactEmail = "";
                NewContactPhoneNumber = "";
                NewContactAddress = "";
                IsAddContactPopupVisible = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error canceling add contact: {ex.Message}");
            }
        }
        public async Task ViewContactDetails(Contact contact)
        {
            try
            {
                if (contact == null)
                {
                    var snackbar = Snackbar.Make("Contact not found",
                        duration: TimeSpan.FromSeconds(2));
                    await snackbar.Show();
                    return;
                }
                SelectedContactForDetails = contact;
                var popup = new ViewContactDetailsPopup(this);
                await Shell.Current.CurrentPage.ShowPopupAsync(popup);
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error viewing contact details: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        private async Task EditContact(Contact contact)
        {
            try
            {
                _editingContact = contact;
                EditName = contact.Name;
                EditEmail = contact.Email ?? "";
                EditPhoneNumber = contact.PhoneNumber ?? "";
                EditAddress = contact.Address ?? "";
                EditDateAdded = contact.DateAdded;
                IsEditMode = 1;
                LoadPendingTransactions(contact);
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error editing contact: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        private async Task DeleteContact(Contact contact)
        {
            try
            {
                var oldTransactions = contact.Transactions?.ToList() ?? new List<Transaction>();
                App.ContactRepo.DeleteItem(contact);
                LoadDB();
                if (SelectedContacts.Contains(contact))
                    SelectedContacts.Remove(contact);
                DataSource.Refresh();

                var snackbar = Snackbar.Make(
                    $"Deleted contact: {contact.Name}",
                    async () => await UndoDeleteContact(contact, oldTransactions),
                    "UNDO",
                    TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make(
                    $"Error deleting contact: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }

        private async Task DeleteSelectedContacts()
        {
            try
            {
                // 1. Build a distinct list of contacts to delete
                var contactsToDelete = new List<Contact>();

                if (SelectedContacts != null)
                    contactsToDelete.AddRange(SelectedContacts);

                if (SelectedItems != null)
                {
                    foreach (var c in SelectedItems.OfType<Contact>())
                        if (!contactsToDelete.Contains(c))
                            contactsToDelete.Add(c);
                }

                if (contactsToDelete.Count == 0)
                {
                    await Snackbar.Make("No contacts selected to delete",
                                         duration: TimeSpan.FromSeconds(2))
                                  .Show();
                    return;
                }

                // 2. Snapshot each contact’s transactions for the undo
                var backups = contactsToDelete
                    .Select(c => new
                    {
                        Contact = c,
                        Transactions = c.Transactions?.ToList() ?? new List<Transaction>()
                    })
                    .ToList();

                // 3. Delete the contacts
                foreach (var c in contactsToDelete)
                    App.ContactRepo.DeleteItem(c);

                LoadDB();
                DataSource.Refresh();
                OnPropertyChanged(nameof(HasSelectedItems));

                // 4. Show snackbar with UNDO that calls UndoDeleteContact per entry
                var snackbar = Snackbar.Make(
                    $"Deleted {contactsToDelete.Count} contacts",
                    async () =>
                    {
                        foreach (var b in backups)
                            await UndoDeleteContact(b.Contact, b.Transactions);
                    },
                    "UNDO",
                    TimeSpan.FromSeconds(4));

                await snackbar.Show();
            }
            catch (Exception ex)
            {
                await Snackbar.Make($"Error deleting contacts: {ex.Message}",
                                     duration: TimeSpan.FromSeconds(3))
                              .Show();
            }
        }

        private async Task UndoDeleteContact(Contact deletedContact, List<Transaction> transactions)
        {
            try
            {
                App.ContactRepo.InsertItemWithExistingChildren<Transaction>(
                    deletedContact,
                    transactions,
                    (contact, txs) => contact.Transactions = (List<Transaction>?)txs
                );
                foreach (var tx in transactions)
                {
                    if (tx.Lines != null)
                    {
                        foreach (var line in tx.Lines)
                        {
                            line.TransactionId = tx.Id;
                            App.TransactionLineRepo.InsertItem(line);
                        }
                    }
                }

                LoadDB();
                DataSource.Refresh();

                var snackbar = Snackbar.Make(
                    $"Restored contact: {deletedContact.Name}",
                    duration: TimeSpan.FromSeconds(2));
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make(
                    $"Error restoring contact: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }


        private async Task SaveContact()
        {
            try
            {
                if (_editingContact != null)
                {
                    _editingContact.Name = EditName;
                    _editingContact.Email = EditEmail;
                    _editingContact.PhoneNumber = EditPhoneNumber;
                    _editingContact.Address = EditAddress;
                    _editingContact.DateAdded = EditDateAdded;
                    App.ContactRepo.UpdateItemWithChildren(_editingContact);
                    LoadDB(); // Refresh the contacts list
                    CancelEdit();
                    var snackbar = Snackbar.Make($"Contact {EditName} saved successfully",
                        duration: TimeSpan.FromSeconds(2));
                    await snackbar.Show();
                }
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error saving contact: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        private void CancelEdit()
        {
            IsEditMode = 0;
            _editingContact = null;
            EditName = "";
            EditEmail = "";
            EditPhoneNumber = "";
            EditAddress = "";
            EditDateAdded = DateTime.Now;
            PendingTransactions?.Clear();
        }
        private async Task RefreshContacts()
        {
            try
            {
                IsRefreshing = true;
                await Task.Delay(1000);
                LoadDB();
                IsRefreshing = false;

            }
            catch (Exception ex)
            {
                IsRefreshing = false;
            }
        }
        private async Task MarkTransactionAsPaid(Transaction transaction)
        {
            try
            {
                transaction.IsPaid = true;
                if (PendingTransactions != null)
                {
                    PendingTransactions.Remove(transaction);
                }
                if (_editingContact?.Transactions != null)
                {
                    var contactTransaction = _editingContact.Transactions.FirstOrDefault(t => t.Id == transaction.Id);
                    if (contactTransaction != null)
                    {
                        contactTransaction.IsPaid = true;
                    }
                    App.ContactRepo.UpdateChildOnly(transaction);
                }
                var snackbar = Snackbar.Make("Transaction marked as paid!",
                    duration: TimeSpan.FromSeconds(2));
                await snackbar.Show();
                OnPropertyChanged(nameof(Contacts));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error marking transaction as paid: {ex.Message}");
                var snackbar = Snackbar.Make($"Error marking transaction as paid: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }
        private void LoadPendingTransactions(Contact contact)
        {
            try
            {
                if (contact?.Transactions != null)
                {
                    var pendingTransactions = contact.Transactions
                        .Where(t => !t.IsPaid)
                        .ToList();
                    PendingTransactions = new ObservableCollection<Transaction>(pendingTransactions);
                }
                else
                {
                    PendingTransactions = new ObservableCollection<Transaction>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading pending transactions: {ex.Message}");
                PendingTransactions = new ObservableCollection<Transaction>();
            }
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Commands
        public ICommand AddNewContactCommand => new Command(async () => await AddNewContact());
        public ICommand EditContactCommand => new Command<Contact>(async (contact) => await EditContact(contact));
        public ICommand DeleteContactCommand => new Command<Contact>(async (contact) => await DeleteContact(contact));
        public ICommand DeleteSelectedContactsCommand => new Command(async () => await DeleteSelectedContacts());
        public ICommand ClearAllSelectionsCommand => new Command(() => ClearAllSelections());
        public ICommand SaveContactCommand => new Command(async () => await SaveContact());
        public ICommand CancelEditCommand => new Command(() => CancelEdit());
        public ICommand RefreshContactsCommand => new Command(async () => await RefreshContacts());
        public ICommand ToggleSortCommand => new Command(() => ToggleSort());
        public ICommand ConfirmAddContactCommand => new Command(async () => await ConfirmAddContact());
        public ICommand CancelAddContactCommand => new Command(() => CancelAddContact());
        public ICommand ViewContactDetailsCommand => new Command<Contact>(async (contact) => await ViewContactDetails(contact));
        public ICommand MarkTransactionAsPaidCommand => new Command<Transaction>(async (transaction) => await MarkTransactionAsPaid(transaction));
        #endregion

        #region Task

        public void LoadDB()
        {
            Contacts = new ObservableCollection<Contact>();
            var DBContacts = App.ContactRepo.GetItemsWithChildren() ?? new List<Contact>();
            foreach (var contact in DBContacts)
            {
                Contacts.Add(contact);
            }
            SelectedContacts = new ObservableCollection<Contact>();
            AutocompleteSelectedContacts = new ObservableCollection<Contact>();
            SelectedItems = new ObservableCollection<object>();
            InitializeDataSource();

        }
        #endregion
    }
}