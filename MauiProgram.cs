using ListingLens;
using ListingLens.API;
using ListingLens.Interfaces;
using ListingLens.Services;
using ListingLens.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

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
        builder.Services.AddHttpClient<EbayAuthService>();
        builder.Services.AddHttpClient<EbayApiProvider>();
        builder.Services.AddSingleton<MarketplaceService>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<MainPage>();

        var assembly = typeof(MauiProgram).Assembly;

        builder.Configuration.AddUserSecrets(assembly);
        builder.Configuration.AddEnvironmentVariables();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}