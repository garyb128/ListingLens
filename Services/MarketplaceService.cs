using ListingLens.Models;

namespace ListingLens.Services;

public class MarketplaceService
{
    public List<EbayMarketplace> GetMarketplaces()
    {
        return
        [
            new EbayMarketplace
            {
                Name = "United Kingdom",
                MarketplaceId = "EBAY_GB",
                CountryCode = "GB"
            },

            new EbayMarketplace
            {
                Name = "United States",
                MarketplaceId = "EBAY_US",
                CountryCode = "US"
            },

            new EbayMarketplace
            {
                Name = "Germany",
                MarketplaceId = "EBAY_DE",
                CountryCode = "DE"
            },

            new EbayMarketplace
            {
                Name = "France",
                MarketplaceId = "EBAY_FR",
                CountryCode = "FR"
            },

            new EbayMarketplace
            {
                Name = "Australia",
                MarketplaceId = "EBAY_AU",
                CountryCode = "AU"
            }
        ];
    }
}
