using System.Net.Http.Json;
using eBaySoldItemAnalyser.Interfaces;
using eBaySoldItemAnalyser.Models;
using Microsoft.Extensions.Configuration;

namespace eBaySoldItemAnalyser.API;

public class SerpAPIClient : ISoldListingProvider
{
    readonly HttpClient _httpClient;
    readonly string _apiKey;
    readonly int _resultsPerPage;

    public SerpAPIClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["SerpApi:ApiKey"] ?? throw new Exception("SerpApi API key is missing.");
        _resultsPerPage = int.TryParse(configuration["SerpApi:ResultsPerPage"],
            out var result) ? result : 100;
    }

    public async Task<SearchResult> SearchSoldListingsAsync(
    string searchTerm,
    string ebayDomain)
    {
        var url = "https://serpapi.com/search" +
            $"?engine=ebay" +
            $"&ebay_domain={Uri.EscapeDataString(ebayDomain)}" +
            $"&_nkw={Uri.EscapeDataString(searchTerm)}" +
            $"&show_only=Sold" +
            $"&_ipg={_resultsPerPage}" +
            $"&api_key={_apiKey}";

        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        return new SearchResult
        {
            SearchTerm = searchTerm,
            Listings =
            [
                new SoldListing
                {
                    Title = json
                }
            ]
        };
    }
}