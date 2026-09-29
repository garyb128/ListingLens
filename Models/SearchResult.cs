namespace ListingLens.Models;

public class SearchResult
{
    public string SearchTerm { get; set; } = string.Empty;

    public List<SoldListing> Listings { get; set; } = [];
}