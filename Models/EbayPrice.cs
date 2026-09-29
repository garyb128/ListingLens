using System.Text.Json.Serialization;

namespace ListingLens.Models;

public class EbayPrice
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
}