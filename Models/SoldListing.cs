namespace eBaySoldItemAnalyser.Models;

public class SoldListing
{
    public string Title { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public string Currency { get; set; } = "GBP";

    public DateTime SoldDate { get; set; }

    public string Condition { get; set; } = string.Empty;

    public string Seller { get; set; } = string.Empty;

    public string ListingUrl { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;
}