namespace eBaySoldItemAnalyser.Models;

public class MarketplaceListing
{
    public string Title { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string Condition { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string ListingUrl { get; set; } = string.Empty;

    public string BuyingOption { get; set; } = string.Empty;

    public string DisplayPrice =>
        $"{Currency} {Price:N2}";
}