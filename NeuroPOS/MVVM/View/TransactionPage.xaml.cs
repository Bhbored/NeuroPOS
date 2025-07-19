using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.Popups;
using NeuroPOS.MVVM.ViewModel;
using System.Diagnostics;

namespace NeuroPOS.MVVM.View;

public partial class TransactionPage : ContentPage
{
    public TransactionPage(TransactionVM vm)
    {
        InitializeComponent();
        BindingContext = vm;
        _=vm.LoadData();
    }
   

    private async void ShowDatePicker(object sender, EventArgs e)
    {
        if (BindingContext is TransactionVM transactionVM)
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new DatePickerPopup(transactionVM));
        }
    }


}