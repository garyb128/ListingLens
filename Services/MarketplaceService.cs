using eBaySoldItemAnalyser.Models;

namespace eBaySoldItemAnalyser.Services;

public class MarketplaceService
{
    public List<EbayMarketplace> GetMarketplaces()
    {
        return
        [
            new EbayMarketplace
            {
                Name = "United Kingdom",
                Domain = "ebay.co.uk",
                Currency = "£"
            },

            new EbayMarketplace
            {
                Name = "United States",
                Domain = "ebay.com",
                Currency = "$"
            },

            new EbayMarketplace
            {
                Name = "Germany",
                Domain = "ebay.de",
                Currency = "€"
            },

            new EbayMarketplace
            {
                Name = "France",
                Domain = "ebay.fr",
                Currency = "€"
            },

            new EbayMarketplace
            {
                Name = "Australia",
                Domain = "ebay.com.au",
                Currency = "$"
            }
        ];
    }
}