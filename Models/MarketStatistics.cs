namespace eBaySoldItemAnalyser.Models;

public class MarketStatistics
{
    public int ListingCount { get; set; }

    public decimal AveragePrice { get; set; }

    public decimal MedianPrice { get; set; }

    public decimal LowestPrice { get; set; }

    public decimal HighestPrice { get; set; }
}