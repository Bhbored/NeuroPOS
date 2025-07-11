using NeuroPOS.MVVM.Popups;
using NeuroPOS.MVVM.ViewModel;
using CommunityToolkit.Maui.Views;

namespace NeuroPOS.MVVM.View;

public partial class TransactionPage : ContentPage
{
	public TransactionPage()
	{
		InitializeComponent();
		BindingContext = new MVVM.ViewModel.TransactionVM();
	}

	
}