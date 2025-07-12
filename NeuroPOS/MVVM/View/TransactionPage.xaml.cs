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
        Debug.WriteLine("TransactionPage: Constructor called, BindingContext set");
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        Debug.WriteLine("OnAppearing: Called");

        if (BindingContext is TransactionVM vm)
        {
            Debug.WriteLine($"OnAppearing: BindingContext found, IsInitialized: {vm.IsInitialized}");
            vm.LoadData();
        }
        else
        {
            Debug.WriteLine("OnAppearing: BindingContext is not TransactionVM!");
        }
    }


}