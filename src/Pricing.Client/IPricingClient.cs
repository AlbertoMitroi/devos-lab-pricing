namespace Pricing.Client;

public interface IPricingClient
{
    Task<PricedQuote> CalculateQuoteAsync(QuoteRequest request, CancellationToken cancellationToken = default);

    Task<PricedQuote?> GetQuoteAsync(Guid quoteId, CancellationToken cancellationToken = default);
}
