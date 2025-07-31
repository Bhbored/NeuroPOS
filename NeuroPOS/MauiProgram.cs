using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
using NeuroPOS.MVVM.ViewModel;
using NeuroPOS.MVVM.View;
using Syncfusion.Maui.Core.Hosting;
using Contact = NeuroPOS.MVVM.Model.Contact;
using NeuroPOS.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace NeuroPOS
{
    public static class MauiProgram
    {
        
        public static MauiApp Host;
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.Configuration
           .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables();

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
            builder.Services.AddSingleton<BaseRepository<Product>>();
            builder.Services.AddSingleton<BaseRepository<Transaction>>();
            builder.Services.AddSingleton<BaseRepository<TransactionLine>>();
            builder.Services.AddSingleton<BaseRepository<Order>>();
            builder.Services.AddSingleton<BaseRepository<InventorySnapshot>>();
            builder.Services.AddSingleton<BaseRepository<CashFlowSnapshot>>();
            builder.Services.AddSingleton<AssistantClient>();

            // ViewModels
            builder.Services.AddSingleton<HomeVM>();
            builder.Services.AddSingleton<TransactionVM>();
            builder.Services.AddSingleton<InventoryVM>();
            builder.Services.AddSingleton<OrderVM>();
            builder.Services.AddSingleton<ContactVM>();
            builder.Services.AddSingleton<CashRegisterVM>();
            builder.Services.AddSingleton<StatisticsVM>();

            // Views
            builder.Services.AddTransient<TransactionPage>();

            //Auth
            builder.Services.AddSingleton<AuthService>();

            Host = builder.Build();
            return Host;
        }
    }
}