
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ListingLens.API;
using ListingLens.Statistics;
using ListingLens.Models;
using ListingLens.Services;
using Microsoft.Maui.ApplicationModel;
using System.Collections.ObjectModel;

namespace ListingLens.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    readonly EbayApiProvider _ebayApiProvider;
    readonly MarketplaceService _marketplaceService;
    readonly EbayCredentialStore _credentialStore;

    [ObservableProperty]
    string searchTerm = string.Empty;

    [ObservableProperty]
    EbayMarketplace? selectedMarketplace;

    [ObservableProperty]
    bool isSearching;

    [ObservableProperty]
    string errorMessage = string.Empty;
    [ObservableProperty]
    string excludeTerms = string.Empty;

    [ObservableProperty]
    CountryOption? selectedItemLocationCountry;

    [ObservableProperty]
    CountryOption? selectedDeliveryCountry;

    [ObservableProperty]
    string searchQualityMessage = string.Empty;

    [ObservableProperty]
    MarketStatistics? statistics;

    [ObservableProperty]
    bool hasStatistics;

    // ----- eBay API key settings -----

    [ObservableProperty]
    bool isSettingsVisible;

    [ObservableProperty]
    string clientIdInput = string.Empty;

    [ObservableProperty]
    string clientSecretInput = string.Empty;

    [ObservableProperty]
    string selectedEnvironment = "Production";

    [ObservableProperty]
    string settingsMessage = string.Empty;

    [ObservableProperty]
    bool hasSavedCredentials;

    public ObservableCollection<string> Environments { get; } =
        ["Production", "Sandbox"];

    public ObservableCollection<EbayMarketplace> MarketPlaces { get; }

    public ObservableCollection<CountryOption> ItemLocationCountries { get; } =
    [
            new() { Name = "Any country", Code = null },
            new() { Name = "United Kingdom", Code = "GB" },
            new() { Name = "Japan", Code = "JP" },
            new() { Name = "United States", Code = "US" },
            new() { Name = "Germany", Code = "DE" },
            new() { Name = "France", Code = "FR" },
            new() { Name = "Australia", Code = "AU" }
];

    public ObservableCollection<CountryOption> DeliveryCountries { get; } =
    [
            new() { Name = "United Kingdom", Code = "GB" },
            new() { Name = "Japan", Code = "JP" },
            new() { Name = "United States", Code = "US" },
            new() { Name = "Germany", Code = "DE" },
            new() { Name = "France", Code = "FR" },
            new() { Name = "Australia", Code = "AU" }
    ];


    public ObservableCollection<MarketplaceListing> Listings { get; } = [];
    List<MarketplaceListing> _originalListings = [];
    public ObservableCollection<string> SortOptions { get; } =
        [
            "Best Match",
            "Price: Low to High",
            "Price: High to Low",
            "Closest to Median"
        ];

    [ObservableProperty]
    string selectedSortOption = "Best Match";

    public SearchViewModel(
        EbayApiProvider ebayApiProvider,
        MarketplaceService marketplaceService,
        EbayCredentialStore credentialStore)
    {
        _ebayApiProvider = ebayApiProvider;
        _marketplaceService = marketplaceService;
        _credentialStore = credentialStore;

        MarketPlaces = new ObservableCollection<EbayMarketplace>(_marketplaceService.GetMarketplaces());

        SelectedMarketplace = MarketPlaces.First();

        SelectedItemLocationCountry = ItemLocationCountries.First();

        SelectedDeliveryCountry = DeliveryCountries.FirstOrDefault(x => x.Code == SelectedMarketplace.CountryCode);

        _ = LoadCredentialsAsync();
    }

    async Task LoadCredentialsAsync()
    {
        var credentials = await _credentialStore.GetAsync();

        HasSavedCredentials = credentials != null;

        if (credentials == null)
        {
            // First run: prompt for the user's own keys.
            IsSettingsVisible = true;
            SettingsMessage = "Enter your own eBay API keys to get started.";
            return;
        }

        ClientIdInput = credentials.ClientId;
        SelectedEnvironment = credentials.IsProduction ? "Production" : "Sandbox";
    }

    [RelayCommand]
    void ToggleSettings()
    {
        IsSettingsVisible = !IsSettingsVisible;
        SettingsMessage = string.Empty;
    }

    [RelayCommand]
    async Task SaveCredentials()
    {
        if (string.IsNullOrWhiteSpace(ClientIdInput))
        {
            SettingsMessage = "Enter your eBay Client ID (App ID).";
            return;
        }

        if (string.IsNullOrWhiteSpace(ClientSecretInput) && !HasSavedCredentials)
        {
            SettingsMessage = "Enter your eBay Client Secret (Cert ID).";
            return;
        }

        try
        {
            await _credentialStore.SaveAsync(
                ClientIdInput,
                ClientSecretInput,
                SelectedEnvironment);

            ClientSecretInput = string.Empty;
            HasSavedCredentials = true;
            SettingsMessage = "Saved. Your keys are stored on this device only.";
            ErrorMessage = string.Empty;
        }
        catch (Exception ex)
        {
            SettingsMessage = $"Couldn't store the keys securely on this device: {ex.Message}";
        }
    }

    [RelayCommand]
    void ClearCredentials()
    {
        _credentialStore.Clear();

        ClientIdInput = string.Empty;
        ClientSecretInput = string.Empty;
        HasSavedCredentials = false;
        SettingsMessage = "Saved keys removed.";
    }

    [RelayCommand]
    async Task OpenListing(MarketplaceListing listing)
    {
        if (string.IsNullOrWhiteSpace(listing.ListingUrl))
            return;

        if (!Uri.TryCreate(listing.ListingUrl, UriKind.Absolute, out var uri))
            return;

        try
        {
            await Launcher.Default.OpenAsync(uri);
        }
        catch(Exception ex)
        {
            ErrorMessage = $"Unable to open listing: {ex.Message}";
        }
    }

    [RelayCommand]
    async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm) ||
            SelectedMarketplace == null)
            return;

        IsSearching = true;
        ErrorMessage = string.Empty;
        SearchQualityMessage = string.Empty;
        HasStatistics = false;
        Statistics = null;

        try
        {
            var result = await _ebayApiProvider.SearchListingsAsync(
                SearchTerm,
                SelectedMarketplace.MarketplaceId,
                SelectedItemLocationCountry?.Code,
                SelectedDeliveryCountry?.Code);

            System.Diagnostics.Debug.WriteLine($"eBay returned {result.ItemSummaries.Count} listings.");

            Listings.Clear();

            var exclusions = ExcludeTerms
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            foreach (var item in result.ItemSummaries)
            {
                var shouldExclude = exclusions.Any(exclusion =>
                    item.Title.Contains(exclusion,
                    StringComparison.OrdinalIgnoreCase));

                if (shouldExclude)
                    continue;

                decimal.TryParse(item.Price?.Value, out var price);

                decimal? shippingPrice = null;

                if (item.ShippingOptions.ValueKind == System.Text.Json.JsonValueKind.Array)
                {
                    foreach (var shippingOption in item.ShippingOptions.EnumerateArray())
                    {
                        if (shippingOption.TryGetProperty("shippingCost", out var shippingCost))
                        {
                            if(shippingCost.TryGetProperty(
                                "value",
                                out var shippingValue) &&
                                decimal.TryParse(
                                    shippingValue.GetString(),
                                    out var shipping))
                            {
                                shippingPrice = shipping;
                            }
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine(
    $"{item.Title} | Item: {price:N2} | P&P: {(shippingPrice.HasValue ? shippingPrice.Value.ToString("N2") : "N/A")}");

                Listings.Add(new MarketplaceListing
                {
                    Title = item.Title,
                    Price = price,
                    Currency = item.Price?.Currency ?? string.Empty,
                    ShippingPrice = shippingPrice ?? 0,
                    HasShippingPrice = shippingPrice.HasValue,
                    Condition = item.Condition,
                    ImageUrl = item.Image?.ImageUrl ?? string.Empty,
                    ListingUrl = item.ItemWebUrl,
                    BuyingOption = item.BuyingOptions.FirstOrDefault() ?? string.Empty,

                    Classification = ListingClassifier.Classify(
                        item.Title,
                        SearchTerm)
                });
            }

            _originalListings = Listings.ToList();

            Statistics = StatisticsCalculator.Calculate(Listings);
            HasStatistics = Statistics.ListingCount > 0;

            var comparableCount = Listings.Count(x => x.IsComparable);

            var comparableRatio = Listings.Count > 0
                ? (decimal)comparableCount / Listings.Count
                : 0m;

            var relativePriceSpread = Statistics.MedianPrice > 0
                ? (Statistics.UpperQuartile - Statistics.LowerQuartile)
                    / Statistics.MedianPrice
                : 0m;


            if (Listings.Count == 0)
            {
                ErrorMessage = "No listings were returned for this search.";
            }
        }
        catch (MissingCredentialsException ex)
        {
            ErrorMessage = ex.Message;
            IsSettingsVisible = true;
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "The search took too long. Please try again.";
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = $"HTTP error: {ex.Message}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Something went wrong: {ex.Message}";
        }
        finally
        {
            IsSearching = false;
        }
    }

    partial void OnSelectedSortOptionChanged(string value)
    {
        ApplySort();
    }

    void ApplySort()
    {
        if (_originalListings.Count == 0)
            return;

        IEnumerable<MarketplaceListing> sortedListings =
            SelectedSortOption switch
            {
                "Price: Low to High" =>
                    _originalListings.OrderBy(x => x.Price),

                "Price: High to Low" =>
                    _originalListings.OrderByDescending(x => x.Price),

                "Closest to Median" when Statistics != null =>
                    _originalListings.OrderBy(
                        x => Math.Abs(x.Price - Statistics.MedianPrice)),

                _ => _originalListings
            };

        Listings.Clear();

        foreach (var listing in sortedListings)
        {
            Listings.Add(listing);
        }
    }
}