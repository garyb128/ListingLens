using ListingLens.Models;
using ListingLens.Services;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ListingLens.API;

public class EbayApiProvider
{
    readonly HttpClient _httpClient;
    readonly EbayAuthService _authService;
    readonly EbayCredentialStore _credentialStore;

    public EbayApiProvider(
        HttpClient httpClient,
        EbayAuthService authService,
        EbayCredentialStore credentialStore)
    {
        _httpClient = httpClient;
        _authService = authService;
        _credentialStore = credentialStore;
    }

    public async Task<EbayBrowseResponse> SearchListingsAsync(
        string searchTerm,
        string marketplaceId,
        string? itemLocationCountry = null,
        string? deliveryCountry = null)
    {
        var credentials = await _credentialStore.GetAsync()
            ?? throw new MissingCredentialsException();

        var accessToken = await _authService.GetAccessTokenAsync(credentials);

        var baseUrl = credentials.ApiBaseUrl;

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

        HttpRequestMessage CreateRequest()
        {
            var request = new HttpRequestMessage(
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

            return request;
        }

        using var response = await HttpRetry.SendAsync(_httpClient, CreateRequest);

        var json = await response.Content.ReadAsStringAsync();

        System.Diagnostics.Debug.WriteLine(
            $"eBay response: {(int)response.StatusCode} {response.StatusCode}");

        System.Diagnostics.Debug.WriteLine(json);

        // Keep the response body; EnsureSuccessStatusCode() would discard eBay's error details.
        if (!response.IsSuccessStatusCode)
        {
            throw new EbayApiException(
                (int)response.StatusCode,
                $"eBay API error ({(int)response.StatusCode} {response.StatusCode}): {json}");
        }

        var result = JsonSerializer.Deserialize<EbayBrowseResponse>(json);

        if (result == null)
        {
            throw new Exception("Unable to read eBay search response.");
        }

        return result;
    }
}
