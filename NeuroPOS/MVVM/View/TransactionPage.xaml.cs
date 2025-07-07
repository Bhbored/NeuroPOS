using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.View;

public partial class TransactionPage : ContentPage
{
	public TransactionPage(TransactionVM vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}