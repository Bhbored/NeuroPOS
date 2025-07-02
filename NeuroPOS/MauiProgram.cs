using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using NeuroPOS.Data;
using NeuroPOS.MVVM.Model;
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
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Poppins-Regular.ttf", "PoppinsRegular");
                    fonts.AddFont("Poppins-Bold.ttf", "PoppinsBold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<BaseRepository<Cart>>();
            builder.Services.AddSingleton<BaseRepository<CashRegister>>();
            builder.Services.AddSingleton<BaseRepository<Category>>();
            builder.Services.AddSingleton<BaseRepository<Contact>>();
            builder.Services.AddSingleton<BaseRepository<Person>>();
            builder.Services.AddSingleton<BaseRepository<Product>>();
            builder.Services.AddSingleton<BaseRepository<Transaction>>();
            return builder.Build();
        }
    }
}
