using System.Text.Json.Serialization;

namespace eBaySoldItemAnalyser.Models;

public class EbayImage
{
    [JsonPropertyName("imageUrl")]
    public string ImageUrl { get; set; } = string.Empty;
}