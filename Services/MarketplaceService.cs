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
                Currency = "GBP",
                MarketplaceId = "EBAY_GB"
            },

            new EbayMarketplace
            {
                Name = "United States",
                Domain = "ebay.com",
                Currency = "USD",
                MarketplaceId = "EBAY_US"
            },

            new EbayMarketplace
            {
                Name = "Germany",
                Domain = "ebay.de",
                Currency = "EUR",
                MarketplaceId = "EBAY_DE"
            },

            new EbayMarketplace
            {
                Name = "France",
                Domain = "ebay.fr",
                Currency = "EUR",
                MarketplaceId = "EBAY_FR"
            },

            new EbayMarketplace
            {
                Name = "Australia",
                Domain = "ebay.com.au",
                Currency = "AUD",
                MarketplaceId = "EBAY_AU"
            }
        ];
    }
}