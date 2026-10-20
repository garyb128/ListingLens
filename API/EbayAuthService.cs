using ListingLens.Models;
using ListingLens.Services;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ListingLens.API;

public class EbayAuthService
{
    // Cached per app run, shared across the transient instances created by the HttpClient factory.
    static string? _accessToken;
    static string? _tokenKey;
    static DateTimeOffset _tokenExpiresAt;

    readonly HttpClient _httpClient;

    public EbayAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync(EbayCredentials credentials)
    {
        var tokenKey = $"{credentials.Environment}|{credentials.ClientId}|{credentials.ClientSecret}";

        if (!string.IsNullOrWhiteSpace(_accessToken) &&
            _tokenKey == tokenKey &&
            DateTimeOffset.UtcNow < _tokenExpiresAt)
        {
            return _accessToken;
        }

        var tokenUrl = $"{credentials.ApiBaseUrl}/identity/v1/oauth2/token";

        var encoded = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{credentials.ClientId}:{credentials.ClientSecret}"));

        HttpRequestMessage CreateRequest()
        {
            var request = new HttpRequestMessage(
                HttpMethod.Post,
                tokenUrl);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Basic", encoded);

            request.Content = new FormUrlEncodedContent(
                [
                    new KeyValuePair<string, string>(
                "grant_type",
                "client_credentials"),

            new KeyValuePair<string, string>(
                "scope",
                "https://api.ebay.com/oauth/api_scope")
                ]);

            return request;
        }

        using var response = await HttpRetry.SendAsync(
            _httpClient,
            CreateRequest);

        if (!response.IsSuccessStatusCode)
        {
            if (response.StatusCode is
                HttpStatusCode.BadRequest or
                HttpStatusCode.Unauthorized or
                HttpStatusCode.Forbidden)
            {
                throw new Exception(
                    $"eBay rejected the API keys ({(int)response.StatusCode}). " +
                    "Check the Client ID, Client Secret and that Environment (Sandbox/Production) matches your keyset.");
            }

            throw new Exception(
                $"eBay could not issue an access token right now ({(int)response.StatusCode}). " +
                "Please try again.");
        }

        var json = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonSerializer.Deserialize<EbayTokenResponse>(json);

        if (tokenResponse == null ||
            string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
        {
            throw new Exception("eBay did not return a valid access token");
        }

        _accessToken = tokenResponse.AccessToken;
        _tokenKey = tokenKey;

        _tokenExpiresAt = DateTimeOffset.UtcNow
            .AddSeconds(tokenResponse.ExpiresIn)
            .AddMinutes(-1);

        return _accessToken;
    }
}
