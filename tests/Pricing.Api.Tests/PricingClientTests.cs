using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pricing.Client;

namespace Pricing.Api.Tests;

/// <summary>Runs the client package against the real API in memory.</summary>
public class PricingClientTests : IClassFixture<PricingApiFactory>
{
    private readonly PricingApiFactory _factory;
    private readonly IPricingClient _client;

    public PricingClientTests(PricingApiFactory factory)
    {
        _factory = factory;
        _factory.EnsureDatabase();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Clients:Pricing:BaseAddress"] = "http://localhost/" })
            .Build();
        var services = new ServiceCollection();
        services.AddPricingClient(configuration).ConfigurePrimaryHttpMessageHandler(() => factory.Server.CreateHandler());
        _client = services.BuildServiceProvider().GetRequiredService<IPricingClient>();
    }

    [Fact]
    public async Task Calculates_and_reads_back_a_quote()
    {
        var quote = await _client.CalculateQuoteAsync(new QuoteRequest("EQ-STD", "forklift", 36_000m, 36));

        quote.RatePercent.Should().Be(5.9m);
        quote.ResidualPercent.Should().Be(25m);
        quote.MonthlyPayment.Should().BeGreaterThan(750m);
        (await _client.GetQuoteAsync(quote.QuoteId)).Should().BeEquivalentTo(quote);
    }

    [Fact]
    public async Task Unknown_quote_is_null()
    {
        (await _client.GetQuoteAsync(Guid.NewGuid())).Should().BeNull();
    }

    [Fact]
    public async Task Term_outside_the_rate_card_is_rejected()
    {
        var act = () => _client.CalculateQuoteAsync(new QuoteRequest("EQ-FLEX", "forklift", 10_000m, 60));

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task Rate_card_minimal_api_returns_the_seeded_card()
    {
        var http = _factory.CreateClient();

        var response = await http.GetAsync("/api/rate-cards/EQ-FLEX");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadFromJsonAsync<Dictionary<string, object>>())!["minTermMonths"].ToString().Should().Be("6");
        (await http.GetAsync("/api/residual-values?assetType=excavator&termMonths=48")).StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
