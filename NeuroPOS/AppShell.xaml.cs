using NeuroPOS.MVVM.View;
using NeuroPOS.MVVM.ViewModel;

namespace NeuroPOS
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes for pages that need ViewModels
            Routing.RegisterRoute(nameof(TransactionPage), typeof(TransactionPage));
        }
    }
}
