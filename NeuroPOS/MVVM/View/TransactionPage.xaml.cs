using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.View;

public partial class TransactionPage : ContentPage
{
	public TransactionPage()
	{
		InitializeComponent();
		BindingContext = new TransactionVM();
	}
}