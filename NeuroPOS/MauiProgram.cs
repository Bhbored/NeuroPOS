using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.MVVM.View;
using Syncfusion.Maui.Core.Hosting;
using Contact = NeuroPOS.MVVM.Model.Contact;

namespace NeuroPOS
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit(options => options.SetShouldEnableSnackbarOnWindows(true))
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            
            builder.Services.AddSingleton<BaseRepository<CashRegister>>();
            builder.Services.AddSingleton<BaseRepository<Category>>();
            builder.Services.AddSingleton<BaseRepository<Contact>>();
            builder.Services.AddSingleton<BaseRepository<Person>>();
            builder.Services.AddSingleton<BaseRepository<Product>>();
            builder.Services.AddSingleton<BaseRepository<Transaction>>();
            builder.Services.AddSingleton<BaseRepository<Order>>();

            // ViewModels
            builder.Services.AddSingleton<HomeVM>();
            builder.Services.AddSingleton<TransactionVM>();
            builder.Services.AddSingleton<InventoryVM>();

            return builder.Build();
        }
    }
}
