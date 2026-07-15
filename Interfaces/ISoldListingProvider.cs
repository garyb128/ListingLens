using eBaySoldItemAnalyser.Models;

namespace eBaySoldItemAnalyser.Interfaces;

public interface ISoldListingProvider
{
    Task<SearchResult> SearchSoldListingsAsync(
        string searchTerm,
        string ebayDomain,
        string currency);
}