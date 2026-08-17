
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using eBaySoldItemAnalyser.API;
using eBaySoldItemAnalyser.Interfaces;
using eBaySoldItemAnalyser.Models;
using eBaySoldItemAnalyser.Services;
using System.Collections.ObjectModel;

namespace eBaySoldItemAnalyser.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    readonly EbayApiProvider _ebayApiProvider;
    readonly MarketplaceService _marketplaceService;

    [ObservableProperty]
    string searchTerm = string.Empty;

    [ObservableProperty]
    EbayMarketplace? selectedMarketplace;

    [ObservableProperty]
    bool isSearching;

    [ObservableProperty]
    string errorMessage = string.Empty;

    public ObservableCollection<EbayMarketplace> MarketPlaces { get; }
    public ObservableCollection<MarketplaceListing> Listings { get; } = [];

    public SearchViewModel(
        EbayApiProvider ebayApiProvider,
        MarketplaceService marketplaceService)
    {
        _ebayApiProvider = ebayApiProvider;
        _marketplaceService = marketplaceService;

        MarketPlaces = new ObservableCollection<EbayMarketplace>(
        _marketplaceService.GetMarketplaces());

        SelectedMarketplace = MarketPlaces.First();
    }

    [RelayCommand]
    async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchTerm) ||
            SelectedMarketplace == null)
            return;

        IsSearching = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _ebayApiProvider.SearchListingsAsync(
                SearchTerm,
                SelectedMarketplace.MarketplaceId);

            System.Diagnostics.Debug.WriteLine($"eBay returned {result.ItemSummaries.Count} listings.");

            Listings.Clear();

            foreach (var item in result.ItemSummaries)
            {
                decimal.TryParse(item.Price?.Value, out var price);

                Listings.Add(new MarketplaceListing
                {
                    Title = item.Title,
                    Price = price,
                    Currency = item.Price?.Currency ?? string.Empty,
                    Condition = item.Condition,
                    ImageUrl = item.Image?.ImageUrl ?? string.Empty,
                    ListingUrl = item.ItemWebUrl,
                    BuyingOption = item.BuyingOptions.FirstOrDefault() ?? string.Empty
                });
            }

            if (Listings.Count == 0)
            {
                ErrorMessage = "No listings were returned for this search.";
            }
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
}