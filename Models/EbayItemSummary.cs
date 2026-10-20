using System.Text.Json;
using System.Text.Json.Serialization;

namespace ListingLens.Models;

public class EbayItemSummary
{
    [JsonPropertyName("itemId")]
    public string ItemId { get; set; } = string.Empty;

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    public EbayPrice? Price { get; set; }

    [JsonPropertyName("shippingOptions")]
    public JsonElement ShippingOptions { get; set; }

    [JsonPropertyName("condition")]
    public string Condition { get; set; } = string.Empty;

    [JsonPropertyName("image")]
    public EbayImage? Image { get; set; }

    [JsonPropertyName("itemWebUrl")]
    public string ItemWebUrl { get; set; } = string.Empty;

    [JsonPropertyName("buyingOptions")]
    public List<string> BuyingOptions { get; set; } = [];
}
