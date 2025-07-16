using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using NeuroPOS.MVVM.Model;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Syncfusion.Maui.DataSource;
using PropertyChanged;
using Contact = NeuroPOS.MVVM.Model.Contact;

namespace NeuroPOS.MVVM.ViewModel
{
    [AddINotifyPropertyChangedInterface]
    public class ContactVM : INotifyPropertyChanged
    {
        private ObservableCollection<Contact> _contacts;
        private ObservableCollection<Contact> _selectedContacts;
        private ObservableCollection<Contact> _autocompleteSelectedContacts;
        private ObservableCollection<object> _selectedItems;
        private string _searchText = "";
        private bool _selectAllContacts = false;
        private bool _isRefreshing = false;
        private int _isEditMode = 0; // 0 = not editing, 1 = editing
        private Contact _editingContact;
        private bool _isSyncingSelection = false;

        // Edit properties
        private string _editName = "";
        private string _editEmail = "";
        private string _editPhoneNumber = "";
        private string _editAddress = "";
        private DateTime _editDateAdded = DateTime.Now;

        public ContactVM()
        {
            InitializeCommands();
            LoadTestData();
        }

        #region Properties

        public ObservableCollection<Contact> Contacts
        {
            get => _contacts;
            set
            {
                _contacts = value;
                OnPropertyChanged(nameof(Contacts));
                // Reinitialize DataSource when Contacts changes
                if (value != null)
                {
                    DataSource = new DataSource() { Source = value };
                    DataSource.Refresh();
                    // Apply current filter if any
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

        // Edit properties
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

        #endregion

        #region Commands

        public ICommand AddNewContactCommand { get; private set; }
        public ICommand EditContactCommand { get; private set; }
        public ICommand DeleteContactCommand { get; private set; }
        public ICommand DeleteSelectedContactsCommand { get; private set; }
        public ICommand ClearAllSelectionsCommand { get; private set; }
        public ICommand SaveContactCommand { get; private set; }
        public ICommand CancelEditCommand { get; private set; }
        public ICommand RefreshContactsCommand { get; private set; }
        public ICommand ToggleSortCommand { get; private set; }

        private void InitializeCommands()
        {
            AddNewContactCommand = new Command(async () => await AddNewContact());
            EditContactCommand = new Command<Contact>(async (contact) => await EditContact(contact));
            DeleteContactCommand = new Command<Contact>(async (contact) => await DeleteContact(contact));
            DeleteSelectedContactsCommand = new Command(async () => await DeleteSelectedContacts());
            ClearAllSelectionsCommand = new Command(() => ClearAllSelections());
            SaveContactCommand = new Command(async () => await SaveContact());
            CancelEditCommand = new Command(() => CancelEdit());
            RefreshContactsCommand = new Command(async () => await RefreshContacts());
            ToggleSortCommand = new Command(() => ToggleSort());
        }

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

        private void LoadTestData()
        {
            var testContacts = new List<Contact>
            {
                new Contact
                {
                    Id = 1,
                    Name = "Ethan Harper",
                    Email = "ethan.harper@email.com",
                    PhoneNumber = "+1 (555) 123-4567",
                    Address = "123 Maple Street, Anytown",
                    DateAdded = new DateTime(2023, 1, 15),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 3200, IsPaid = true },
                        new Transaction { TotalAmount = 2000, IsPaid = true },
                        new Transaction { TotalAmount = 200, IsPaid = false }
                    }
                },
                new Contact
                {
                    Id = 2,
                    Name = "Olivia Bennett",
                    Email = "olivia.bennett@email.com",
                    PhoneNumber = "+1 (555) 234-5678",
                    Address = "456 Oak Avenue, Anytown",
                    DateAdded = new DateTime(2023, 2, 20),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 2500, IsPaid = true },
                        new Transaction { TotalAmount = 1300, IsPaid = true },
                        new Transaction { TotalAmount = 150, IsPaid = false }
                    }
                },
                new Contact
                {
                    Id = 3,
                    Name = "Noah Carter",
                    Email = "noah.carter@email.com",
                    PhoneNumber = "+1 (555) 345-6789",
                    Address = "789 Pine Lane, Anytown",
                    DateAdded = new DateTime(2023, 3, 25),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 1800, IsPaid = true },
                        new Transaction { TotalAmount = 700, IsPaid = true },
                        new Transaction { TotalAmount = 100, IsPaid = false }
                    }
                },
                new Contact
                {
                    Id = 4,
                    Name = "Ava Rodriguez",
                    Email = "ava.rodriguez@email.com",
                    PhoneNumber = "+1 (555) 456-7890",
                    Address = "321 Elm Street, Anytown",
                    DateAdded = new DateTime(2023, 4, 10),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 4200, IsPaid = true },
                        new Transaction { TotalAmount = 800, IsPaid = false }
                    }
                },
                new Contact
                {
                    Id = 5,
                    Name = "Liam Thompson",
                    Email = "liam.thompson@email.com",
                    PhoneNumber = "+1 (555) 567-8901",
                    Address = "654 Birch Road, Anytown",
                    DateAdded = new DateTime(2023, 5, 5),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 1500, IsPaid = true },
                        new Transaction { TotalAmount = 300, IsPaid = false }
                    }
                },
                new Contact
                {
                    Id = 6,
                    Name = "Sophia Martinez",
                    Email = "sophia.martinez@email.com",
                    PhoneNumber = "+1 (555) 678-9012",
                    Address = "987 Cedar Lane, Anytown",
                    DateAdded = new DateTime(2023, 6, 15),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 2800, IsPaid = true },
                        new Transaction { TotalAmount = 500, IsPaid = false }
                    }
                },
                new Contact
                {
                    Id = 7,
                    Name = "Mason Johnson",
                    Email = "mason.johnson@email.com",
                    PhoneNumber = "+1 (555) 789-0123",
                    Address = "147 Willow Way, Anytown",
                    DateAdded = new DateTime(2023, 7, 8),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 3600, IsPaid = true },
                        new Transaction { TotalAmount = 400, IsPaid = false }
                    }
                },
                new Contact
                {
                    Id = 8,
                    Name = "Isabella Davis",
                    Email = "isabella.davis@email.com",
                    PhoneNumber = "+1 (555) 890-1234",
                    Address = "258 Spruce Street, Anytown",
                    DateAdded = new DateTime(2023, 8, 22),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 1900, IsPaid = true },
                        new Transaction { TotalAmount = 250, IsPaid = false }
                    }
                },
                new Contact
                {
                    Id = 9,
                    Name = "William Wilson",
                    Email = "william.wilson@email.com",
                    PhoneNumber = "+1 (555) 901-2345",
                    Address = "369 Poplar Avenue, Anytown",
                    DateAdded = new DateTime(2023, 9, 12),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 3100, IsPaid = true },
                        new Transaction { TotalAmount = 350, IsPaid = false }
                    }
                },
                new Contact
                {
                    Id = 10,
                    Name = "Mia Anderson",
                    Email = "mia.anderson@email.com",
                    PhoneNumber = "+1 (555) 012-3456",
                    Address = "741 Aspen Drive, Anytown",
                    DateAdded = new DateTime(2023, 10, 3),
                    Transactions = new List<Transaction>
                    {
                        new Transaction { TotalAmount = 2200, IsPaid = true },
                        new Transaction { TotalAmount = 180, IsPaid = false }
                    }
                }
            };

            Contacts = new ObservableCollection<Contact>(testContacts);
            SelectedContacts = new ObservableCollection<Contact>();
            AutocompleteSelectedContacts = new ObservableCollection<Contact>();
            SelectedItems = new ObservableCollection<object>();

            // Initialize DataSource after Contacts are loaded
            DataSource = new DataSource() { Source = Contacts };
            DataSource.Refresh();
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
                // If no search text, show all contacts
                if (string.IsNullOrEmpty(SearchText))
                    return true;

                // If search text contains commas, it's multiple selected contacts
                if (SearchText.Contains(","))
                {
                    var selectedNames = SearchText.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => s.Trim())
                        .ToList();

                    // Show only the selected contacts
                    return selectedNames.Any(name =>
                        contact.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    // Single search term - filter by name or email
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
                // Select all contacts from the DataSource
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

                // Update SelectAllContacts state based on whether all items are selected
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
            // Implement sorting logic
            var snackbar = Snackbar.Make("Sort functionality will be implemented",
                duration: TimeSpan.FromSeconds(2));
            _ = snackbar.Show();
        }

        private async Task AddNewContact()
        {
            var snackbar = Snackbar.Make("Add New Contact functionality will be implemented",
                duration: TimeSpan.FromSeconds(2));
            await snackbar.Show();
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
                Contacts.Remove(contact);
                if (SelectedContacts.Contains(contact))
                    SelectedContacts.Remove(contact);

                DataSource.Refresh();

                var snackbar = Snackbar.Make($"Deleted contact: {contact.Name}",
                    duration: TimeSpan.FromSeconds(2));
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error deleting contact: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }

        private async Task DeleteSelectedContacts()
        {
            try
            {
                // Get contacts from both SelectedContacts and SelectedItems
                var contactsToDelete = new List<Contact>();

                // Add from SelectedContacts
                if (SelectedContacts != null)
                    contactsToDelete.AddRange(SelectedContacts);

                // Add from SelectedItems (in case SelectedContacts is not synced)
                if (SelectedItems != null)
                {
                    var itemsContacts = SelectedItems.OfType<Contact>().ToList();
                    foreach (var contact in itemsContacts)
                    {
                        if (!contactsToDelete.Contains(contact))
                            contactsToDelete.Add(contact);
                    }
                }

                if (contactsToDelete.Count == 0)
                {
                    var noSelectionSnackbar = Snackbar.Make("No contacts selected to delete",
                        duration: TimeSpan.FromSeconds(2));
                    await noSelectionSnackbar.Show();
                    return;
                }

                // Store the deleted contacts for potential undo
                var deletedContacts = contactsToDelete.ToList();

                foreach (var contact in contactsToDelete)
                {
                    Contacts.Remove(contact);
                }

                SelectedContacts.Clear();
                SelectedItems.Clear();
                DataSource.Refresh();
                OnPropertyChanged(nameof(HasSelectedItems));

                // Create snackbar with undo action
                var snackbar = Snackbar.Make(
                    $"Deleted {contactsToDelete.Count} contacts",
                    async () => await UndoDeleteContacts(deletedContacts),
                    "UNDO",
                    TimeSpan.FromSeconds(4));
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error deleting contacts: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
            }
        }

        private async Task UndoDeleteContacts(List<Contact> deletedContacts)
        {
            try
            {
                foreach (var contact in deletedContacts)
                {
                    Contacts.Add(contact);
                }

                DataSource.Refresh();

                var snackbar = Snackbar.Make($"Restored {deletedContacts.Count} contacts",
                    duration: TimeSpan.FromSeconds(2));
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                var snackbar = Snackbar.Make($"Error restoring contacts: {ex.Message}",
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

                    DataSource.Refresh();
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
        }

        private async Task RefreshContacts()
        {
            try
            {
                IsRefreshing = true;
                await Task.Delay(1000); // Simulate refresh
                DataSource.Refresh();
                IsRefreshing = false;

                var snackbar = Snackbar.Make("Contacts refreshed",
                    duration: TimeSpan.FromSeconds(1));
                await snackbar.Show();
            }
            catch (Exception ex)
            {
                IsRefreshing = false;
                var snackbar = Snackbar.Make($"Error refreshing contacts: {ex.Message}",
                    duration: TimeSpan.FromSeconds(3));
                await snackbar.Show();
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
    }
}
