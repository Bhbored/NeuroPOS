using NeuroPOS.MVVM.Popups;
using NeuroPOS.MVVM.ViewModel;
using CommunityToolkit.Maui.Views;
using System.Diagnostics;

namespace NeuroPOS.MVVM.View;

public partial class TransactionPage : ContentPage
{
    public TransactionPage(TransactionVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TransactionVM vm)
        {
            vm.LoadData();
        }
    }


}