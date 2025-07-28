using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS.MVVM.View;

public partial class StatisticsPage : ContentPage
{
	public StatisticsPage(StatisticsVM vm)
	{
		InitializeComponent();
		BindingContext = vm;

    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is StatisticsVM vm)
        {
            await vm.LoadDB();
        }
    }
}