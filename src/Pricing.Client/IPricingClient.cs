namespace Pricing.Client;

public interface IPricingClient
{
    Task<PricedQuote> CalculateQuoteAsync(QuoteRequest request, CancellationToken cancellationToken = default);

    Task<PricedQuote?> GetQuoteAsync(Guid quoteId, CancellationToken cancellationToken = default);

    /// <summary>Prices an existing quote again with another term. Since 1.1.0.</summary>
    Task<PricedQuote> RecalculateQuoteAsync(Guid quoteId, int termMonths, CancellationToken cancellationToken = default);
}
