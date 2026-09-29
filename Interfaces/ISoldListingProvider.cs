using ListingLens.Models;

namespace ListingLens.Interfaces;

public interface ISoldListingProvider
{
    Task<SearchResult> SearchSoldListingsAsync(
        string searchTerm,
        string ebayDomain,
        string currency);
}