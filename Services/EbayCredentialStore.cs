using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Storage;

namespace ListingLens.Services;

public record EbayCredentials(
    string ClientId,
    string ClientSecret,
    string Environment)
{
    public bool IsProduction =>
        Environment.Equals("Production", StringComparison.OrdinalIgnoreCase);

    public string ApiBaseUrl =>
        IsProduction
            ? "https://api.ebay.com"
            : "https://api.sandbox.ebay.com";
}

public class MissingCredentialsException : Exception
{
    public MissingCredentialsException()
        : base("No eBay API keys found. Open \"API keys\" and enter your own eBay Client ID and Client Secret.")
    {
    }
}

/// <summary>
/// Holds the user's own eBay API keys. Keys entered in the app are kept in the
/// platform's secure storage (never in the project files). If nothing has been
/// entered in the app, falls back to configuration (user secrets or
/// environment variables such as Ebay__ClientId / Ebay__ClientSecret).
/// </summary>
public class EbayCredentialStore
{
    const string ClientIdKey = "ebay_client_id";
    const string ClientSecretKey = "ebay_client_secret";
    const string EnvironmentKey = "ebay_environment";

    readonly IConfiguration _configuration;

    public EbayCredentialStore(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<EbayCredentials?> GetAsync()
    {
        string? clientId = null;
        string? clientSecret = null;

        try
        {
            clientId = await SecureStorage.Default.GetAsync(ClientIdKey);
            clientSecret = await SecureStorage.Default.GetAsync(ClientSecretKey);
        }
        catch
        {
            // Secure storage can be unavailable on some platforms; use configuration instead.
        }

        if (string.IsNullOrWhiteSpace(clientId) ||
            string.IsNullOrWhiteSpace(clientSecret))
        {
            clientId = _configuration["Ebay:ClientId"];
            clientSecret = _configuration["Ebay:ClientSecret"];
        }

        if (IsMissing(clientId) || IsMissing(clientSecret))
            return null;

        var environment = Preferences.Default.Get(EnvironmentKey, string.Empty);

        if (string.IsNullOrWhiteSpace(environment))
            environment = _configuration["Ebay:Environment"] ?? "Sandbox";

        return new EbayCredentials(
            clientId!.Trim(),
            clientSecret!.Trim(),
            environment);
    }

    /// <summary>Saves the keys. A blank secret keeps the previously saved one.</summary>
    public async Task SaveAsync(
        string clientId,
        string? clientSecret,
        string environment)
    {
        await SecureStorage.Default.SetAsync(ClientIdKey, clientId.Trim());

        if (!string.IsNullOrWhiteSpace(clientSecret))
            await SecureStorage.Default.SetAsync(ClientSecretKey, clientSecret.Trim());

        Preferences.Default.Set(EnvironmentKey, environment);
    }

    public void Clear()
    {
        try
        {
            SecureStorage.Default.Remove(ClientIdKey);
            SecureStorage.Default.Remove(ClientSecretKey);
        }
        catch
        {
        }

        Preferences.Default.Remove(EnvironmentKey);
    }

    static bool IsMissing(string? value) =>
        string.IsNullOrWhiteSpace(value) ||
        value.StartsWith("YOUR_", StringComparison.OrdinalIgnoreCase);
}
