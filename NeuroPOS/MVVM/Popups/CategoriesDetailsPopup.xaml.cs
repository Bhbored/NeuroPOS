using CommunityToolkit.Maui.Views;
using NeuroPOS.MVVM.ViewModel;
using System.Diagnostics;

namespace NeuroPOS.MVVM.Popups
{
    public partial class CategoriesDetailsPopup : Popup
    {
        public CategoriesDetailsPopup(InventoryVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
       
    }
}