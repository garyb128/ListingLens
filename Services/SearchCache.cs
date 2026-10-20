using ListingLens.Models;
using System.Collections.Concurrent;

namespace ListingLens.Services;

public readonly record struct SearchCacheKey(
    string MarketplaceId,
    string SearchTerm,
    string? ItemLocationCountry,
    string? DeliveryCountry)
{
    public static SearchCacheKey Create(
        string marketplaceId,
        string searchTerm,
        string? itemLocationCountry,
        string? deliveryCountry)
    {
        return new SearchCacheKey(
            marketplaceId,
            searchTerm.Trim().ToLowerInvariant(),
            itemLocationCountry,
            deliveryCountry);
    }
}

public sealed record SearchCacheEntry(
    EbayBrowseResponse Response,
    DateTimeOffset CachedAt);

public class SearchCache
{
    static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);

    readonly ConcurrentDictionary<SearchCacheKey, SearchCacheEntry> _entries = new();

    public SearchCacheEntry? Get(SearchCacheKey key)
    {
        if (!_entries.TryGetValue(key, out var entry))
            return null;

        if (DateTimeOffset.UtcNow - entry.CachedAt >= CacheDuration)
        {
            _entries.TryRemove(key, out _);
            return null;
        }

        return entry;
    }

    public void Set(SearchCacheKey key, EbayBrowseResponse response)
    {
        _entries[key] = new SearchCacheEntry(
            response,
            DateTimeOffset.UtcNow);
    }

    public void Clear()
    {
        _entries.Clear();
    }
}