using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using Syncfusion.Maui.ListView;

namespace NeuroPOS.MVVM.View;

public partial class InventoryPage : ContentPage
{
	private InventoryVM _viewModel;

	public InventoryPage(InventoryVM vm)
	{
		InitializeComponent();
		_viewModel = vm;
		BindingContext = vm;

		// Set page reference for callbacks
		vm.PageReference = this;

		// Subscribe to ListView events
		listView.SelectionChanged += ListView_SelectionChanged;
		autocomplete.SelectionChanged += Autocomplete_SelectionChanged;
	}

	private void ListView_SelectionChanged(object sender, ItemSelectionChangedEventArgs e)
	{
		if (e.AddedItems?.Count > 0)
		{
			foreach (var item in e.AddedItems)
			{
				if (item is Product product)
				{
					_viewModel.AddToPersistentSelection(product.Id);
				}
			}
		}

		if (e.RemovedItems?.Count > 0)
		{
			foreach (var item in e.RemovedItems)
			{
				if (item is Product product)
				{
					_viewModel.RemoveFromPersistentSelection(product.Id);
				}
			}
		}

		_viewModel.UpdateSelectedItemsCountDisplay();
	}

	private void Autocomplete_SelectionChanged(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
	{
		// Handle autocomplete selection changes if needed
		// The search functionality is now handled by the binding
	}

	private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
	{
		// Update sort icon based on sort state
		SortIcon.Source = _viewModel.SortState switch
		{
			InventoryVM.SortDirectionState.Ascending => "ascending.png",
			InventoryVM.SortDirectionState.Descending => "descending.png",
			InventoryVM.SortDirectionState.None => "ascending.png",
			_ => "ascending.png"
		};
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		_viewModel.RefreshUI();
	}
}