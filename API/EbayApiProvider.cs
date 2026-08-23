using eBaySoldItemAnalyser.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text.Json;

namespace eBaySoldItemAnalyser.API;

public class EbayApiProvider
{
    readonly HttpClient _httpClient;
    readonly EbayAuthService _authService;
    readonly string _environment;

    public EbayApiProvider(
        HttpClient httpClient,
        EbayAuthService authService,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _authService = authService;
        _environment = configuration["Ebay:Environment"] ?? "Sandbox";
    }

    public async Task<EbayBrowseResponse> SearchListingsAsync(
        string searchTerm,
        string marketplaceId,
        string? itemLocationCountry = null,
        string? deliveryCountry = null)
    {
        var accessToken = await _authService.GetAccessTokenAsync();

        var baseUrl = _environment.Equals(
            "Production",
            StringComparison.OrdinalIgnoreCase)
                ? "https://api.ebay.com"
                : "https://api.sandbox.ebay.com";

        var url =
            $"{baseUrl}/buy/browse/v1/item_summary/search" +
            $"?q={Uri.EscapeDataString(searchTerm)}" +
            "&limit=100" +
            "&fieldgroups=FULL";

        var filters = new List<string>();

        if (!string.IsNullOrWhiteSpace(itemLocationCountry))
        {
            filters.Add(
                $"itemLocationCountry:{itemLocationCountry}");
        }

        if (!string.IsNullOrWhiteSpace(deliveryCountry))
        {
            filters.Add(
                $"deliveryCountry:{deliveryCountry}");
        }

        if (filters.Count > 0)
        {
            url += $"&filter={Uri.EscapeDataString(
                string.Join(",", filters))}";
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        request.Headers.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                accessToken);

        request.Headers.Add(
            "X-EBAY-C-MARKETPLACE-ID",
            marketplaceId);

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue(
                "application/json"));
        
        var response = await _httpClient.SendAsync(request);

        var json = await response.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine(
            $"eBay response: {(int)response.StatusCode} {response.StatusCode}");

        System.Diagnostics.Debug.WriteLine(json);

        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<EbayBrowseResponse>(json);

        if (result == null)
        {
            throw new Exception("Unable to read eBay search response.");
        }

        return result;
    }


}