using eBaySoldItemAnalyser.Interfaces;
using eBaySoldItemAnalyser.Models;

namespace eBaySoldItemAnalyser.API;

/// <summary>
/// Stub for the official eBay developer API provider.
///
/// TODO: implement SearchSoldListingsAsync using the eBay Browse API:
/// https://developer.ebay.com/api-docs/buy/browse/resources/item_summary/methods/search
///
/// Notes for the real implementation:
/// - The eBay API authenticates with OAuth (Client ID + Client Secret) and
///   returns an access token, which must be sent as a Bearer token.
/// - eBay uses marketplace IDs (e.g. "EBAY_GB", "EBAY_US") instead of domains
///   like "ebay.co.uk". You may want to change the ISoldListingProvider
///   signature (or add a marketplace-to-id mapping) when implementing this.
/// - The eBay API returns currency per item in the price object, so the
///   currency parameter may no longer be needed.
/// - Search sold items with the filter parameter, e.g.
///   filter=soldItems:{true}
/// </summary>
public class EbayApiProvider : ISoldListingProvider
{
    public Task<SearchResult> SearchSoldListingsAsync(
        string searchTerm,
        string ebayDomain,
        string currency)
    {
        throw new NotImplementedException(
            "EbayApiProvider not implemented yet. Implement SearchSoldListingsAsync using the eBay developer API.");
    }
}
