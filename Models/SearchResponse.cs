using System.Text.Json.Serialization;

namespace eBaySoldItemAnalyser.Models;

public class SearchResponse
{
    [JsonPropertyName("organic_results")]
    public List<EbayListing>? OrganicResults { get; set; }
}

public class EbayListing
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("price")]
    public EbayPrice? Price { get; set; }

    [JsonPropertyName("condition")]
    public string? Condition { get; set; }

    [JsonPropertyName("thumbnail")]
    public string? Thumbnail { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("sold_date")]
    public string? SoldDate { get; set; }
}

public class EbayPrice
{
    [JsonPropertyName("raw")]
    public string? Raw { get; set; }

    [JsonPropertyName("extracted")]
    public decimal Extracted { get; set; }
}
