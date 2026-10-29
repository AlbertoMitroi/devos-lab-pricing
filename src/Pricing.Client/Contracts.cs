namespace Pricing.Client;

public sealed record QuoteRequest(string ProductCode, string AssetType, decimal AssetValue, int TermMonths);

public sealed record PricedQuote(
    Guid QuoteId,
    string ProductCode,
    string AssetType,
    decimal AssetValue,
    int TermMonths,
    decimal RatePercent,
    decimal ResidualPercent,
    decimal MonthlyPayment,
    DateTimeOffset CreatedAt);
