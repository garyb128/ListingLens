using System.Text.Json.Serialization;

namespace ListingLens.Models;

public class EbayImage
{
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
}