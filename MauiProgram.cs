using eBaySoldItemAnalyser;
using eBaySoldItemAnalyser.API;
using eBaySoldItemAnalyser.Interfaces;
using eBaySoldItemAnalyser.Services;
using eBaySoldItemAnalyser.ViewModels;
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
            });

        builder.Services.AddHttpClient();
        builder.Services.AddSingleton<ISoldListingProvider, SerpAPIClient>();
        builder.Services.AddSingleton<MarketplaceService>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<AppShell>();
        builder.Services.AddTransient<MainPage>();

        var assembly = typeof(MauiProgram).Assembly;

        using var stream = assembly.GetManifestResourceStream("eBaySoldItemAnalyser.appsettings.json");

        if (stream != null)
            builder.Configuration.AddJsonStream(stream);

        builder.Configuration.AddUserSecrets(assembly);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}