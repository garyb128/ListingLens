# Handover: eBay Sold Item Market Analyser

Status: **In development (learning project).** Last code change 2026-07-15.
Last activity 2026-08-14 (diagnosed SerpApi outage — see below).

## Latest status (2026-08-14)

**Search appears broken but the code is fine — SerpApi's backend is failing.**

Diagnosis (tested directly against the API with curl):

- SerpApi base site and non-sold eBay searches: HTTP 200, fast
- SerpApi **sold** eBay searches: take ~90 seconds, then either succeed or
  return HTTP 503 with body `"We couldn't get valid results for this search.
  Please try again later."` — this is SerpApi's own scraping failure, not a bug
  in this project
- Not a quota problem: only 5/250 monthly searches used
- Smaller `_ipg` (10) succeeded where `_ipg` (100) failed — may be intermittent
  SerpApi-side; could improve later in the month

Why the app appears broken when this happens:

1. `SerpAPIClient` calls `EnsureSuccessStatusCode()` → throws on 503
2. `SearchViewModel.Search()` has `try/finally` but **no `catch`** — exception
   escapes, user sees nothing
3. `HttpClient` default timeout is 100s — request fails around the same time
   SerpApi's 503 arrives (~90s)

**Pending code fix (learning task):** add `catch` + error display so failures
are shown to the user instead of failing silently; consider lowering
`ResultsPerPage` to 10 and adding a retry. (See Roadmap step 2.)

## Provider switch (2026-08-14)

**SerpApi gutted.** Root cause of the search failure: SerpApi's scraper was
hitting an eBay sign-in page instead of results (hence "couldn't get valid
results"). Decision: move to the **official eBay developer API**.

What changed:

- Deleted `API/SerpAPIClient.cs` (SerpApi HTTP client) and
  `Models/SearchResponse.cs` (SerpApi-specific JSON shapes)
- Added `API/EbayApiProvider.cs` — a **stub** implementing
  `ISoldListingProvider` that throws `NotImplementedException`; comments inside
  point at the eBay Browse API docs and the things to adapt (OAuth bearer
  token, marketplace IDs like `EBAY_GB` instead of domains, per-item currency)
- `MauiProgram.cs`: DI now registers `AddSingleton<ISoldListingProvider, EbayApiProvider>()`
- `Resources/Raw/appsettings.json`: `SerpApi` section replaced with an `Ebay`
  placeholder (`ClientId` / `ClientSecret`)
- Old `SerpApi:ApiKey` / `SerpApi:ResultsPerPage` entries still in user secrets
  — harmless, can be removed

The seam for swapping providers is unchanged: everything goes through
`ISoldListingProvider.SearchSoldListingsAsync(searchTerm, ebayDomain, currency)`.
Implementing the eBay API = fill in `EbayApiProvider` (and adjust the interface
signature if needed for marketplace IDs). Build compiles; clicking Search
currently throws `NotImplementedException` until the eBay provider is done.

## What this project is

A .NET MAUI app that searches recently **sold** eBay items via the SerpApi eBay
Search API and displays the results so the user can gauge what items are worth
selling. Built as a learning project for C#/.NET, MAUI, MVVM, data binding,
commands, DI, and async API calls.

> Note: an earlier plan (see scope doc "eBay Sold Item Analyser.txt") proposed a
> WPF-first build. **Decision: this has been superseded — the MAUI project IS the
> project.** No WPF version will be created.

## Tech stack

- .NET MAUI, target `net10.0` (Windows, Android, iOS, MacCatalyst)
- C# with nullable reference types enabled
- CommunityToolkit.Mvvm 8.4.2 (source-generated MVVM)
- Microsoft.Extensions.Http + Configuration.UserSecrets
- Data source: official eBay developer API (planned — provider currently a
  stub). SerpApi previously used, now removed.

## Project layout

```
eBaySoldItemAnalyser/
|-- API/                  EbayApiProvider.cs     — stub for official eBay API (TODO)
|-- Interfaces/           ISoldListingProvider.cs — data source abstraction
|-- Models/               SearchResponse, SearchResult, SoldListing,
|                         EbayMarketplace, MarketStatistics
|-- Services/             MarketplaceService.cs   — hardcoded list of 5 eBay marketplaces
|-- Statistics/           EMPTY (planned home for stats calculation)
|-- ViewModels/           SearchViewModel.cs      — search command, observable state
|-- Views/                EMPTY (planned home for extra views)
|-- MainPage.xaml(.cs)    Search UI + results list
|-- AppShell.xaml         Shell, single Home route
|-- MauiProgram.cs        DI registrations, config (appsettings + user secrets)
```

## What works today

- Search box + marketplace picker (UK, US, Germany, France, Australia)
- Sold listings display in a `CollectionView` (image, title, price w/ currency,
  condition), scrollable
- MVVM wiring: `SearchViewModel`, `[ObservableProperty]`, `[RelayCommand]`,
  `ObservableCollection<SoldListing>`, `IsSearching` activity indicator
- DI in `MauiProgram.cs`; API key config in appsettings + user secrets
  (UserSecretsId `fc27e301-8773-4b54-86ab-e80309a772ff`)
- **Note:** searching currently throws `NotImplementedException` until the eBay
  provider (`API/EbayApiProvider.cs`) is implemented

## Build / run

```
dotnet build -f net10.0-windows10.0.19041.0 -c Debug
```

Build currently succeeds: 0 errors, 11 warnings (see Known issues).

## What the user wants (MVP goal)

1. Open the app
2. Search for any item
3. Retrieve recent sold eBay listings
4. View the actual sales
5. **See calculated market statistics** — e.g. for 100 listings:
   - Median: £145
   - Average: £158
   - Lowest: £60
   - Highest: £420

## Gaps / next steps

- **EbayApiProvider is a stub** — implementing the official eBay developer API
  is the top task (OAuth token, Browse API search, marketplace IDs). **Highest priority now.**
- **Error handling not implemented** — `Search()` has no `catch`; API failures
  fail silently.
- **Market statistics not implemented** — `MarketStatistics` model exists but is
  unused; `Statistics/` folder is empty. This is the main missing MVP feature.
- Sold date (`SoldListing.SoldDate`) fetched but not displayed in UI
- `Seller` field never populated
- No caching of API results yet (planned under API layer)
- Additional provider support planned (e.g. SoldComps) behind
  `ISoldListingProvider`

## Roadmap

Built as learning exercises — **user types the code, assistant guides**.

1. **Implement eBay API provider** — fill in `EbayApiProvider`: OAuth client
   credentials flow for an access token, call the Browse API
   `item_summary/search` endpoint with the sold-items filter, map the JSON
   response onto `SoldListing`/`SearchResult`. Adjust the `ISoldListingProvider`
   signature if needed (marketplace IDs, per-item currency).
2. **Error handling & UX** — `catch` in `Search()`; show a message on failure
   (e.g. label bound to an `ErrorMessage` property); distinguish network error vs
   API error. Retry/backoff optional.
3. **Market statistics** — implement calculation in `Statistics/` (average,
   median, lowest, highest over the sold listings); `MarketStatistics` model;
   unit-testable pure function taking `List<SoldListing>`; display stats on the
   main page.
4. **Display sold date** — surface `SoldListing.SoldDate` in the results list.
5. **Tidy warnings** — use generated `SearchTerm`/`SelectedMarketplace`/
   `IsSearching` properties instead of backing fields; partial properties
   (MVVMTK0045); `Frame` → `Border`.
6. **Caching** — cache API responses to avoid re-hitting eBay for repeat
   searches.
7. **More providers** — second `ISoldListingProvider` implementation
   (e.g. SoldComps) so providers are interchangeable.
8. **Future expansion** — saved searches, price alerts, community reporting,
   fake/incorrect listing flags, category-specific metadata, user accounts.
   Focus areas: football shirts, retro games, collectibles.

## Known issues (build warnings)

- `SearchViewModel.cs:51` — CS8602 nullable dereference of `selectedMarketplace`
- MVVMTK0034 — backing fields referenced directly; use generated properties
  (`SearchTerm`, `SelectedMarketplace`, `IsSearching`)
- MVVMTK0045 — `[ObservableProperty]` fields should be partial properties for AOT
- `MainPage.xaml` — `Frame` is obsolete in .NET 9+; use `Border`

## Future expansion (wishlist, not started)

Saved searches, price alerts, community reporting, fake/incorrect listing flags,
category-specific metadata, user accounts. Focus areas: football shirts, retro
games, collectibles. Architecture should keep services/models reusable so a MAUI
(or future) UI shares the same core.
