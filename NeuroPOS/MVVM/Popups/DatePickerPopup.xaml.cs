using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.ViewModel;
using System.Windows.Input;

namespace NeuroPOS.MVVM.Popups;

public partial class DatePickerPopup :Popup
{
    public DatePickerPopup()
    {
        InitializeComponent();
        DatePickerLimitations();
        BindingContext = new DatePickerVM();
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.CurrentPage.ClosePopupAsync();
    }

    private void DatePickerLimitations() {
        this.calendar.MinimumDate = DateTime.Now.AddYears(-1);
        this.calendar.MaximumDate = DateTime.Now;
    }
}