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
                Domain = "ebay.co.uk"
            },

            new EbayMarketplace
            {
                Name = "United States",
                Domain = "ebay.com"
            },

            new EbayMarketplace
            {
                Name = "Germany",
                Domain = "ebay.de"
            },

            new EbayMarketplace
            {
                Name = "France",
                Domain = "ebay.fr"
            },

            new EbayMarketplace
            {
                Name = "Australia",
                Domain = "ebay.com.au"
            }
        ];
    }
}