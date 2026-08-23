namespace eBaySoldItemAnalyser.Models;

public class MarketStatistics
{
    public int ListingCount { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MedianPrice { get; set; }
    public decimal LowestPrice { get; set; }
    public decimal HighestPrice { get; set; }
    public string Currency { get; init; } = string.Empty;
    public decimal LowerQuartile { get; init; }
    public decimal UpperQuartile { get; init; }
    public int OutlierCount { get; init; }
    public int AnalysedListingCount { get; init; }
    public ListingClassification Classification { get; set; }
    public bool IsComparable => Classification == ListingClassification.Standard;
    public string LowerQuartileDisplay => $"{Currency} {LowerQuartile:N2}";
    public string UpperQuartileDisplay => $"{Currency} {UpperQuartile:N2}";
    public string TypicalRangeDisplay => $"{Currency} {LowerQuartile:N2} - {UpperQuartile:N2}";
    public string AverageDisplay => $"{Currency} {AveragePrice:N2}";
    public string MedianDisplay => $"{Currency} {MedianPrice:N2}";
    public string LowestDisplay => $"{Currency} {LowestPrice:N2}";
    public string HighestDisplay => $"{Currency} {HighestPrice:N2}";
    public string TypicalLowDisplay => $"{Currency} {LowerQuartile:N2}";
    public string TypicalHighDisplay => $"{Currency} {UpperQuartile:N2}";
}