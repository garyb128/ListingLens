using eBaySoldItemAnalyser.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace eBaySoldItemAnalyser.API;

public class EbayAuthService
{
    readonly HttpClient _httpClient;
    readonly string _clientId;
    readonly string _clientSecret;
    readonly string _environment;

    string? _accessToken;
    DateTimeOffset _tokenExpiresAt;

    public EbayAuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        _clientId = configuration["Ebay:ClientId"]
            ?? throw new Exception("eBay Client ID is missing");

        _clientSecret = configuration["Ebay:ClientSecret"]
            ?? throw new Exception("eBay Client Secret is missing");

        _environment = configuration["Ebay:Environment"] ?? "Sandbox";
    }

    public async Task<string> GetAccessTokenAsync()
    {
        if(!string.IsNullOrWhiteSpace(_accessToken) &&
            DateTimeOffset.UtcNow < _tokenExpiresAt)
        {
            return _accessToken;
        }

        var tokenUrl = _environment.Equals(
            "Production",
            StringComparison.OrdinalIgnoreCase)
                ? "https://api.ebay.com/identity/v1/oauth2/token"
                : "https://api.sandbox.ebay.com/identity/v1/oauth2/token";

        var credentials = Convert.ToBase64String(
            Encoding.UTF8.GetBytes($"{_clientId}:{_clientSecret}"));

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            tokenUrl);

        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        request.Content = new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>(
                    "grant_type",
                    "client_credentials"
                    ),
                
                new KeyValuePair<string, string>(
                    "scope",
                    "https://api.ebay.com/oauth/api_scope"
                    )
            ]);

        var response = await _httpClient.SendAsync(request);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var tokenResponse = JsonSerializer.Deserialize<EbayTokenResponse>(json);

        if(tokenResponse == null ||
            string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
        {
            throw new Exception("eBay did not return a valid access token");
        }

        _accessToken = tokenResponse.AccessToken;

        _tokenExpiresAt = DateTimeOffset.UtcNow
            .AddSeconds(tokenResponse.ExpiresIn)
            .AddMinutes(-1);

        return _accessToken;
    }
}