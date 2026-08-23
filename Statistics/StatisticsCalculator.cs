using eBaySoldItemAnalyser.Models;

namespace eBaySoldItemAnalyser.Statistics
{
    public static class StatisticsCalculator
    {
        public static MarketStatistics Calculate(
            IEnumerable<MarketplaceListing> listings)
        {
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

                if(values.Count % 2 == 0)
                {
                    return (values[middle - 1] + values[middle]) / 2m;
                }

                return values[middle];
            }

            var median = CalculateMedian(prices);

            var middle = prices.Count / 2;

            var lowerHalf = prices
                .Take(middle)
                .ToList();

            var upperHalf = prices
                .Skip(prices.Count % 2 == 0 ? middle : middle + 1)
                .ToList();

            var lowest = prices.First();
            var highest = prices.Last();

            var lowerQuartile = CalculateMedian(lowerHalf);
            var upperQuartile = CalculateMedian(upperHalf);

            var iqr = upperQuartile - lowerQuartile;

            var lowerBound = lowerQuartile - (1.5m * iqr);
            var upperBound = upperQuartile + (1.5m * iqr);

            var analysedPrices = prices
                .Where(x => x >= lowerBound && x <= upperBound)
                .ToList();

            var outlierCount = prices.Count - analysedPrices.Count;

            return new MarketStatistics
            {
                ListingCount = prices.Count,
                AveragePrice = prices.Average(),

                MedianPrice = median,
                LowestPrice = prices.First(),
                HighestPrice = prices.Last(),

                LowerQuartile = lowerQuartile,
                UpperQuartile = upperQuartile,

                Currency = validListings.First().Currency
            };
        }
    }
}
