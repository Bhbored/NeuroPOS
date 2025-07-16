using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.View;

public partial class OrdersPage : ContentPage
{
	public OrdersPage()
	{
		InitializeComponent();
		BindingContext = new OrderVM();
	}

	protected override void OnAppearing()
	{
		base.OnAppearing();
		// Any additional initialization when page appears
	}
}