namespace eBaySoldItemAnalyser.Models;

public class MarketplaceListing
{
    public string Title { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public decimal ShippingPrice { get; set; }

    public bool HasShippingPrice { get; set; }

    public decimal TotalPrice => Price + ShippingPrice;

    public string Currency { get; set; } = string.Empty;

    public string Condition { get; set; } = string.Empty;

    public string ImageUrl { get; set; } = string.Empty;

    public string ListingUrl { get; set; } = string.Empty;

    public string BuyingOption { get; set; } = string.Empty;

    public ListingClassification Classification { get; set; }

    public bool IsComparable =>
        Classification == ListingClassification.Standard;


    public string ItemPriceDisplay =>
        $"Item: {Currency} {Price:N2}";


    public string ShippingPriceDisplay
    {
        get
        {
            if (!HasShippingPrice)
                return "P&P: See listing";

            if (ShippingPrice == 0)
                return "P&P: Free";

            return $"P&P: {Currency} {ShippingPrice:N2}";
        }
    }


    public string TotalPriceDisplay =>
        HasShippingPrice
            ? $"{Currency} {TotalPrice:N2}"
            : "See listing";
}