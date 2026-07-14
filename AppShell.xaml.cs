namespace eBaySoldItemAnalyser;

public partial class AppShell : Shell
{
    public AppShell(MainPage mainPage)
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));

        Items.Add(new ShellContent
        {
            Content = mainPage
        });
    }
}