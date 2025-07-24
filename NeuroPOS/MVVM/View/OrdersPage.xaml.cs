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
		StatusFilterPicker.SelectedIndexChanged += OnStatusFilterChanged;	
		StatusFilterPicker.SelectedIndex = 0;
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
	}

	private void OnStatusFilterChanged(object sender, EventArgs e)
	{
		if (BindingContext is OrderVM vm && sender is Picker picker)
		{
			vm.SelectedStatusFilter = picker.SelectedItem?.ToString() ?? "All";
		}
	}
}