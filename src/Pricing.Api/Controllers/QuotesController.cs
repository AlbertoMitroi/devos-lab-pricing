using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pricing.Api.Data;
using Pricing.Api.Pricing;

namespace Pricing.Api.Controllers;

public sealed record CalculateQuoteRequest(string ProductCode, string AssetType, decimal AssetValue, int TermMonths);

public sealed record RecalculateQuoteRequest(int TermMonths);

public sealed record QuoteResponse(
    Guid QuoteId,
    string ProductCode,
    string AssetType,
    decimal AssetValue,
    int TermMonths,
    decimal RatePercent,
    decimal ResidualPercent,
    decimal MonthlyPayment,
    DateTimeOffset CreatedAt);

[ApiController]
[Route("api/quotes")]
public sealed class QuotesController(PricingDbContext db, TimeProvider clock) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<QuoteResponse>> Calculate(CalculateQuoteRequest request, CancellationToken cancellationToken)
    {
        var quote = await Price(Guid.NewGuid(), request.ProductCode, request.AssetType, request.AssetValue, request.TermMonths, cancellationToken);
        if (quote is null)
        {
            return ValidationProblem($"No rate card or residual value for {request.ProductCode}, {request.AssetType}, {request.TermMonths} months.");
        }

        db.Quotes.Add(quote);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = quote.Id }, ToResponse(quote));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuoteResponse>> Get(Guid id, CancellationToken cancellationToken)
    {
        var quote = await db.Quotes.AsNoTracking().SingleOrDefaultAsync(q => q.Id == id, cancellationToken);
        return quote is null ? NotFound() : ToResponse(quote);
    }

    [HttpPost("{id:guid}/recalculate")]
    public async Task<ActionResult<QuoteResponse>> Recalculate(Guid id, RecalculateQuoteRequest request, CancellationToken cancellationToken)
    {
        var existing = await db.Quotes.SingleOrDefaultAsync(q => q.Id == id, cancellationToken);
        if (existing is null)
        {
            return NotFound();
        }

        var quote = await Price(id, existing.ProductCode, existing.AssetType, existing.AssetValue, request.TermMonths, cancellationToken);
        if (quote is null)
        {
            return ValidationProblem($"No residual value for {existing.AssetType}, {request.TermMonths} months.");
        }

        db.Entry(existing).CurrentValues.SetValues(quote);
        await db.SaveChangesAsync(cancellationToken);
        return ToResponse(existing);
    }

    private async Task<QuoteEntity?> Price(Guid id, string productCode, string assetType, decimal assetValue, int termMonths, CancellationToken cancellationToken)
    {
        var card = await db.RateCards.AsNoTracking().SingleOrDefaultAsync(c => c.ProductCode == productCode, cancellationToken);
        var residual = await db.ResidualValues.AsNoTracking()
            .SingleOrDefaultAsync(r => r.AssetType == assetType && r.TermMonths == termMonths, cancellationToken);
        if (card is null || residual is null || termMonths < card.MinTermMonths || termMonths > card.MaxTermMonths)
        {
            return null;
        }

        return new QuoteEntity
        {
            Id = id,
            ProductCode = productCode,
            AssetType = assetType,
            AssetValue = assetValue,
            TermMonths = termMonths,
            RatePercent = card.BaseRatePercent,
            ResidualPercent = residual.Percent,
            MonthlyPayment = LeaseCalculator.MonthlyPayment(assetValue, card.BaseRatePercent, termMonths, residual.Percent),
            CreatedAt = clock.GetUtcNow(),
        };
    }

    private static QuoteResponse ToResponse(QuoteEntity q) =>
        new(q.Id, q.ProductCode, q.AssetType, q.AssetValue, q.TermMonths, q.RatePercent, q.ResidualPercent, q.MonthlyPayment, q.CreatedAt);
}
