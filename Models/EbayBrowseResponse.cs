using System.Text.Json.Serialization;

namespace eBaySoldItemAnalyser.Models;

public class EbayBrowseResponse
{
    [JsonPropertyName("itemSummaries")]
    public List<EbayItemSummary> ItemSummaries { get; set; } = [];

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }

    [JsonPropertyName("offset")]
    public int Offset { get; set; }
}