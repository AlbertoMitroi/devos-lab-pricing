using System.Net;
using System.Net.Http.Json;

namespace Pricing.Client;

internal sealed class PricingClient(HttpClient httpClient) : IPricingClient
{
    public async Task<PricedQuote> CalculateQuoteAsync(QuoteRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync("api/quotes", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<PricedQuote>(cancellationToken))!;
    }

    public async Task<PricedQuote?> GetQuoteAsync(Guid quoteId, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/quotes/{quoteId}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PricedQuote>(cancellationToken);
    }
}
