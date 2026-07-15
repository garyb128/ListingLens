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

        isSearching = true;

        try
        {
            var result = await _listingProvider.SearchSoldListingsAsync(searchTerm, SelectedMarketplace.Domain,selectedMarketplace.Currency);

            Listings.Clear();

            foreach (var listing in result.Listings)
            {
                Listings.Add(listing);
            }
        }
        finally
        {
            isSearching = false;
        }
    }
}