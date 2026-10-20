using ListingLens.Models;

namespace ListingLens.Statistics
{
    public static class StatisticsCalculator
    {
        public static MarketStatistics Calculate(
            IEnumerable<MarketplaceListing> listings)
        {
            // Auctions and classified listings would distort a "typical price" summary.
            var validListings = listings
                .Where(x => x.Price > 0)
                .Where(x => x.IsComparable)
                .Where(x => !string.Equals(
                    x.BuyingOption,
                    "AUCTION",
                    StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (validListings.Count == 0)
                return new MarketStatistics();

            var prices = validListings
                .Select(x => x.Price)
                .OrderBy(x => x)
                .ToList();

            static decimal CalculateMedian(List<decimal> values)
            {
                var middle = values.Count / 2;

                if (values.Count % 2 == 0)
                {
                    return (values[middle - 1] + values[middle]) / 2m;
                }

                return values[middle];
            }

            var middle = prices.Count / 2;

            var lowerHalf = prices
                .Take(middle)
                .ToList();

            var upperHalf = prices
                .Skip(prices.Count % 2 == 0 ? middle : middle + 1)
                .ToList();

            // A single price has no spread, so both quartiles are that price.
            var lowerQuartile = lowerHalf.Count == 0 ? prices[0] : CalculateMedian(lowerHalf);
            var upperQuartile = upperHalf.Count == 0 ? prices[0] : CalculateMedian(upperHalf);

            var iqr = upperQuartile - lowerQuartile;

            var lowerBound = lowerQuartile - (1.5m * iqr);
            var upperBound = upperQuartile + (1.5m * iqr);

            // Fences come from the raw distribution; the summary below uses the cleaned set.
            var analysedPrices = prices
                .Where(x => x >= lowerBound && x <= upperBound)
                .ToList();

            var outlierCount = prices.Count - analysedPrices.Count;

            return new MarketStatistics
            {
                ListingCount = prices.Count,
                AnalysedListingCount = analysedPrices.Count,
                OutlierCount = outlierCount,

                AveragePrice = analysedPrices.Average(),
                MedianPrice = CalculateMedian(analysedPrices),
                LowestPrice = analysedPrices.First(),
                HighestPrice = analysedPrices.Last(),

                LowerQuartile = lowerQuartile,
                UpperQuartile = upperQuartile,

                Currency = validListings.First().Currency
            };
        }
    }
}
