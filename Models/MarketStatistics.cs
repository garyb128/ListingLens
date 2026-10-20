namespace ListingLens.Models;

public class MarketStatistics
{
    // Valid listings before the IQR outlier filter.
    public int ListingCount { get; set; }
    public decimal AveragePrice { get; set; }
    public decimal MedianPrice { get; set; }
    public decimal LowestPrice { get; set; }
    public decimal HighestPrice { get; set; }
    public string Currency { get; init; } = string.Empty;
    public decimal LowerQuartile { get; init; }
    public decimal UpperQuartile { get; init; }

    // AnalysedListingCount is after the IQR filter; OutlierCount is how many were removed.
    public int OutlierCount { get; init; }
    public int AnalysedListingCount { get; init; }

    public string AverageDisplay => $"{Currency} {AveragePrice:N2}";
    public string MedianDisplay => $"{Currency} {MedianPrice:N2}";
    public string LowestDisplay => $"{Currency} {LowestPrice:N2}";
    public string HighestDisplay => $"{Currency} {HighestPrice:N2}";
    public string TypicalLowDisplay => $"{Currency} {LowerQuartile:N2}";
    public string TypicalHighDisplay => $"{Currency} {UpperQuartile:N2}";
}