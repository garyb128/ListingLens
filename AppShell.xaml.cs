namespace ListingLens;

public partial class AppShell : Shell
{
    public AppShell(MainPage mainPage)
    {
        InitializeComponent();

        // Use the DI-created page so its SearchViewModel is injected.
        Items.Add(new ShellContent
        {
            Title = "Home",
            Route = "MainPage",
            Content = mainPage
        });
    }
}