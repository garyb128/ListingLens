
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using eBaySoldItemAnalyser.Interfaces;
using eBaySoldItemAnalyser.Models;
using eBaySoldItemAnalyser.Services;
using System.Collections.ObjectModel;

namespace eBaySoldItemAnalyser.ViewModels;

public partial class SearchViewModel : ObservableObject
{
    readonly ISoldListingProvider _listingProvider;
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
    public ObservableCollection<SoldListing> Listings { get; } = [];

    public SearchViewModel(
        ISoldListingProvider listingProvider,
        MarketplaceService marketplaceService)
    {
        _listingProvider = listingProvider;
        _marketplaceService = marketplaceService;


        MarketPlaces = new ObservableCollection<EbayMarketplace>(
            _marketplaceService.GetMarketplaces());

        selectedMarketplace = MarketPlaces.First();
    }

    [RelayCommand]
    async Task Search()
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || selectedMarketplace == null)
            return;

        IsSearching = true;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _listingProvider.SearchSoldListingsAsync(
                SearchTerm,
                SelectedMarketplace.Domain,
                SelectedMarketplace.Currency);

            System.Diagnostics.Debug.WriteLine(
                $"Listings returned: {result.Listings.Count}");

            if (result.Listings.Count == 0)
            {
                ErrorMessage = "The search completed, but no sold listings were returned.";
                return;
            }

            Listings.Clear();

            foreach (var listing in result.Listings)
            {
                Listings.Add(listing);
            }
        }
        catch(TaskCanceledException)
        {
            ErrorMessage = "The search took too long. Please try again.";
        }
        catch (HttpRequestException ex)
        {
            if(ex.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                ErrorMessage = "The sold listings service is currently unavailable. Please try again later.";
            }
            else
            {
                ErrorMessage = $"HTTP error: {ex.Message}";
            }
        }
        catch (Exception)
        {
            ErrorMessage = "Something went wrong while searching.";
        }
        finally
        {
            IsSearching = false;
        }
    }
}