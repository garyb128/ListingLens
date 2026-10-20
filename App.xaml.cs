using Microsoft.Extensions.DependencyInjection;

namespace ListingLens;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services)
    {
        InitializeComponent();

        _services = services;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Resolve the shell after app resources have loaded.
        return new Window(_services.GetRequiredService<AppShell>());
    }
}
