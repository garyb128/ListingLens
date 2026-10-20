using ListingLens.Models;

namespace ListingLens.Statistics;

public static class ListingClassifier
{
    static readonly string[] BundleTerms =
        [
            "bundle",
            "job lot",
            "lot of",
            "multi buy"
        ];

    static readonly string[] PartialTerms =
    [
        "box only",
        "case only",
        "manual only",
        "instructions only",
        "cover only",
        "empty box",
        "no game",
        "no console"
    ];

    static readonly string[] ForPartsTerms =
    [
        "for parts",
        "spares or repair",
        "spares repair",
        "not working",
        "faulty",
        "broken"
    ];

    static readonly string[] ReproductionTerms =
    [
        "reproduction",
        "repro",
        "replica",
        "bootleg",
        "fake",
        "aftermarket"
    ];

    public static ListingClassification Classify(
        string title,
        string searchTerm)
    {
        var normalizedTitle = Normalize(title);
        var normalizedSearch = Normalize(searchTerm);

        // Most specific classification first; do not exclude a term the user explicitly searched for.
        if (ContainsAny(normalizedTitle, ReproductionTerms) &&
            !ContainsAny(normalizedSearch, ReproductionTerms))
        {
            return ListingClassification.Reproduction;
        }

        if (ContainsAny(normalizedTitle, ForPartsTerms) &&
            !ContainsAny(normalizedSearch, ForPartsTerms))
        {
            return ListingClassification.ForParts;
        }

        if (ContainsAny(normalizedTitle, PartialTerms) &&
            !ContainsAny(normalizedSearch, PartialTerms))
        {
            return ListingClassification.Partial;
        }

        if (ContainsAny(normalizedTitle, BundleTerms) &&
            !ContainsAny(normalizedSearch, BundleTerms))
        {
            return ListingClassification.Bundle;
        }

        return ListingClassification.Standard;
    }

    static bool ContainsAny(
        string text,
        IEnumerable<string> terms)
    {
        return terms.Any(term =>
            ContainsPhrase(text, Normalize(term)));
    }

    // Match whole words only, so "box" does not match "boxed".
    static bool ContainsPhrase(
        string text,
        string phrase)
    {
        return $" {text} ".Contains(
            $" {phrase} ",
            StringComparison.Ordinal);
    }

    // Normalize punctuation and case before word matching.
    static string Normalize(string value)
    {
        var characters = value
            .ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : ' ')
            .ToArray();

        return string.Join(
            ' ',
            new string(characters)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
