using Microsoft.EntityFrameworkCore;
using Pricing.Api.Data;

namespace Pricing.Api.Endpoints;

public sealed record RateCardResponse(string ProductCode, decimal BaseRatePercent, int MinTermMonths, int MaxTermMonths);

public sealed record ResidualValueResponse(string AssetType, int TermMonths, decimal Percent);

public static class RateCardEndpoints
{
    public static IEndpointRouteBuilder MapRateCardEndpoints(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api");

        api.MapGet("/rate-cards/{productCode}", async (string productCode, PricingDbContext db, CancellationToken ct) =>
            await db.RateCards.AsNoTracking().SingleOrDefaultAsync(c => c.ProductCode == productCode, ct) is { } card
                ? Results.Ok(new RateCardResponse(card.ProductCode, card.BaseRatePercent, card.MinTermMonths, card.MaxTermMonths))
                : Results.NotFound());

        api.MapGet("/residual-values", async (string assetType, int termMonths, PricingDbContext db, CancellationToken ct) =>
            await db.ResidualValues.AsNoTracking().SingleOrDefaultAsync(r => r.AssetType == assetType && r.TermMonths == termMonths, ct) is { } value
                ? Results.Ok(new ResidualValueResponse(value.AssetType, value.TermMonths, value.Percent))
                : Results.NotFound());

        return app;
    }
}
