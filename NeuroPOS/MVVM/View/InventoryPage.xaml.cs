using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.View;

public partial class InventoryPage : ContentPage
{
	public InventoryPage(InventoryVM vm)
	{
		InitializeComponent();
		BindingContext = vm;

	}
}