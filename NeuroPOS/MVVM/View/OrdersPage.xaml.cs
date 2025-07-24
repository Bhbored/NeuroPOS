using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using NeuroPOS.MVVM.ViewModel;
using Microsoft.Maui.Controls;

namespace NeuroPOS.MVVM.View;

public partial class OrdersPage : ContentPage
{
	public OrdersPage()
	{
		InitializeComponent();
		BindingContext = new OrderVM();
		
		// Wire up status filter picker event handler
		StatusFilterPicker.SelectedIndexChanged += OnStatusFilterChanged;
		
		// Ensure the picker is properly initialized
		StatusFilterPicker.SelectedIndex = 0; // Set to "All" by default
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		// Any additional initialization when page appears
	}

	private void OnStatusFilterChanged(object sender, EventArgs e)
	{
		if (BindingContext is OrderVM vm && sender is Picker picker)
		{
			vm.SelectedStatusFilter = picker.SelectedItem?.ToString() ?? "All";
			// The ApplyFilters method will be called automatically through the property setter
		}
	}
}