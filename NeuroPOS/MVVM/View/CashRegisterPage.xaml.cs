using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.View;

public partial class CashRegisterPage : ContentPage
{
	public CashRegisterPage(CashRegisterVM vm)
	{
		InitializeComponent();
        BindingContext = vm;
	}
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is CashRegisterVM vm && vm.Transactions.Count == 0)
        {
            MainThread.InvokeOnMainThreadAsync
                (vm.LoadDB);
           
        }

    }
}