using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.Popups;
using NeuroPOS.MVVM.ViewModel;
using Syncfusion.Maui.DataSource;
using Syncfusion.Maui.Inputs;
using Syncfusion.Maui.ListView;
using System;
using System.Linq;
using System.Threading.Tasks;
namespace NeuroPOS.MVVM.View
{
    public partial class HomePage : ContentPage
    {
        public HomePage(HomeVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
            vm.SelectedItems.Clear();
            vm.PageReference = this;
            ConfigureListView();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is HomeVM vm)
            {
                await vm.LoadDB();
            }
        }
        #region filtering logic
        private void ConfigureListView()
        {
            if (listView.DataSource != null)
                listView.DataSource.LiveDataUpdateMode = LiveDataUpdateMode.Default;
            listView.SelectionChanged += ListView_SelectionChanged;
            searchFilterValue.Text = "0";
            selectedValue.Text = "0";
        }
        public void ListView_SelectionChanged(object sender, ItemSelectionChangedEventArgs e)
        {
            if (BindingContext is not HomeVM vm || sender is not SfListView lv)
                return;
            foreach (var prod in e.AddedItems.OfType<Product>())
            {
                if (prod.Stock <= 0)
                {
                    lv.SelectedItems.Remove(prod);
                    continue;
                }
                if (!vm.SelectedItems.Contains(prod))
                {
                    vm.SelectedItems.Add(prod);
                    vm.AddToPersistentSelection(prod.Id);
                }
                var inCart = vm.CurrentOrderItems.FirstOrDefault(x => x.Id == prod.Id);
                if (inCart == null)
                    vm.AddToCurrentOrder(prod, fromListViewSelection: true);
                else
                    vm.IncrementQuantity(inCart);
            }
            foreach (var prod in e.RemovedItems.OfType<Product>())
            {
                vm.SelectedItems.Remove(prod);
                vm.RemoveFromPersistentSelection(prod.Id);
                var inCart = vm.CurrentOrderItems.FirstOrDefault(x => x.Id == prod.Id);
                if (inCart != null)
                    vm.RemoveFromCurrentOrder(inCart);
            }
            vm.UpdateSelectedItemsCountDisplay();
            vm.NotifySelectionChanged();
        }
        internal void RefreshRow(Product p)
        {
            if (listView.DataSource != null)
            {
                var index = listView.DataSource.DisplayItems.IndexOf(p);
                if (index >= 0)
                    listView.RefreshItem(index);
                else
                    listView.RefreshView();
            }
            else
            {
                listView.RefreshView();
            }
        }
        private void Autocomplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
        {
            if (BindingContext is not HomeVM vm)
                return;
            searchFilterValue.Text = autocomplete.SelectedItems?.Count.ToString() ?? "0";
            vm.SelectedProducts = autocomplete.SelectedItems?
                .OfType<Product>()
                .Cast<object>()
                .ToList() ?? new System.Collections.Generic.List<object>();
            if (listView.DataSource == null)
                return;
            listView.DataSource.Filter = FilterProducts;
            listView.DataSource.RefreshFilter();
            vm.RestoreListViewSelections();
            vm.UpdateSelectedItemsCountDisplay();
        }
        private bool FilterProducts(object obj)
        {
            if (obj is not Product p || BindingContext is not HomeVM vm)
                return false;
            return vm.SelectedProducts == null || vm.SelectedProducts.Count == 0
                   || vm.SelectedProducts.OfType<Product>()
                        .Any(sel => sel.Id == p.Id ||
                                    p.Name.Equals(sel.Name, StringComparison.OrdinalIgnoreCase));
        }
        private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            if (BindingContext is not HomeVM vm)
                return;
            Icon.Source = vm.SortState switch
            {
                HomeVM.SortDirectionState.Ascending => "ascending.png",
                HomeVM.SortDirectionState.Descending => "descending.png",
                _ => string.Empty
            };
        }

        #endregion

        #region Discount logic
        private void DiscountEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BindingContext is not HomeVM vm)
                return;
            if (double.TryParse(e.NewTextValue, out var v))
                vm.UpdateDiscount(v);
            else if (string.IsNullOrEmpty(e.NewTextValue))
                vm.UpdateDiscount(0);
        }
        private void QuantityEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not Entry entry ||
                entry.BindingContext is not Product product ||
                BindingContext is not HomeVM vm)
                return;
            if (int.TryParse(e.NewTextValue, out var q))
                vm.ValidateAndUpdateQuantity(product, q.ToString());
            else if (string.IsNullOrEmpty(e.NewTextValue))
                vm.ValidateAndUpdateQuantity(product, "1");
        }




        #endregion

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (BindingContext is HomeVM vm)
            {
                var popup = new ChatAssistantPopup(vm);
                AppShell.Current.ShowPopupAsync(popup);
            }
        }

        
    }
}