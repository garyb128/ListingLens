using ListingLens;
using ListingLens.API;
using ListingLens.Services;
using ListingLens.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<EbayCredentialStore>();
        builder.Services.AddHttpClient<EbayAuthService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
        });
        builder.Services.AddHttpClient<EbayApiProvider>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
        });
        builder.Services.AddSingleton<MarketplaceService>();
        builder.Services.AddSingleton<SearchCache>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<MainPage>();

        var assembly = typeof(MauiProgram).Assembly;

        // In-app SecureStorage is the primary key source; these are development fallbacks.
        builder.Configuration.AddUserSecrets(assembly);
        builder.Configuration.AddEnvironmentVariables();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}