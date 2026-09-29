# ListingLens

A .NET MAUI app that searches eBay through the official Browse API and turns the results into a quick price summary, so you can gauge what an item is worth before you buy or sell it.

> **Note:** the eBay Browse API returns *active* listings (asking prices), not completed/sold sales. ListingLens summarises what similar items are currently listed for.

## Features

- Search any item across five eBay marketplaces (UK, US, Germany, France, Australia)
- Filter by item location and delivery country
- Exclude terms from a search
- Market summary: lowest, typical low, median, average, typical high, highest (outliers removed with an IQR filter)
- Listings are auto-classified (bundle / partial / for parts / reproduction) and excluded from the stats so they don't skew prices
- Item price, postage and total price per listing, with sorting

## Tech stack

- C# / .NET 10, .NET MAUI (Windows, Android, iOS, Mac Catalyst targets)
- MVVM with CommunityToolkit.Mvvm
- eBay Browse API with OAuth client-credentials flow
- Dependency injection via `Microsoft.Extensions`

## Getting started

### 1. Get your own eBay API keys

1. Create a free account at the [eBay Developers Program](https://developer.ebay.com/).
2. Create an application keyset (Sandbox and/or Production).
3. Note your **App ID (Client ID)** and **Cert ID (Client Secret)**.

Sandbox keys work out of the box but return very little data; use Production for real listings.

### 2. Add your keys (never commit them)

ListingLens never ships with keys. You supply your own, in either of these ways:

**In the app (easiest).** Click **API keys** at the top of the window, paste your Client ID and Client Secret, choose `Production` (real listings) or `Sandbox`, and press **Save**. The app opens this panel automatically the first time you run it without keys. Keys are saved with the platform's secure storage on your device only. They are never written to the project folder, so they can't end up in git. **Remove saved keys** deletes them again.

**For development (optional).** If nothing is saved in the app, ListingLens falls back to .NET [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or environment variables. From the project folder:

```
dotnet user-secrets set "Ebay:ClientId" "YOUR_EBAY_CLIENT_ID"
dotnet user-secrets set "Ebay:ClientSecret" "YOUR_EBAY_CLIENT_SECRET"
dotnet user-secrets set "Ebay:Environment" "Production"
```

Environment variables work too (`Ebay__ClientId`, `Ebay__ClientSecret`, `Ebay__Environment`). `Ebay:Environment` is `Sandbox` (default when not set) or `Production`.

`Resources/Raw/appsettings.example.json` shows the shape of the configuration with placeholder values. Do not put real keys in any file tracked by git.

Troubleshooting: an "eBay rejected the API keys" error usually means the keys were entered wrongly, or the Environment (Sandbox/Production) doesn't match the keyset they came from.

### 3. Run

Requires the .NET 10 SDK and the MAUI workload (`dotnet workload install maui`).

```
dotnet build -f net10.0-windows10.0.19041.0
dotnet run -f net10.0-windows10.0.19041.0
```

## Project layout

```
API/          eBay OAuth + Browse API clients
Interfaces/   Provider abstractions
Models/       API DTOs and app models
Services/     Marketplace definitions
Statistics/   Listing classifier and price statistics
ViewModels/   MVVM view models
MainPage.xaml Search UI
```

## Roadmap

- Caching of search results
- Saved searches and price alerts
- Better error handling and retry

## Disclaimer

ListingLens is an independent project and is not affiliated with or endorsed by eBay Inc. Use of the eBay API is subject to the [eBay API License Agreement](https://developer.ebay.com/join/api_license_agreement).

## License

MIT - see [LICENSE](LICENSE).
