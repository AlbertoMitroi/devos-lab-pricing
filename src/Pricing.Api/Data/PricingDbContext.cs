using Microsoft.EntityFrameworkCore;

namespace Pricing.Api.Data;

public sealed class PricingDbContext(DbContextOptions<PricingDbContext> options) : DbContext(options)
{
    public DbSet<RateCardEntity> RateCards => Set<RateCardEntity>();

    public DbSet<ResidualValueEntity> ResidualValues => Set<ResidualValueEntity>();

    public DbSet<QuoteEntity> Quotes => Set<QuoteEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RateCardEntity>(e =>
        {
            e.ToTable("rate_cards");
            e.HasKey(x => x.ProductCode);
            e.HasData(
                new RateCardEntity { ProductCode = "EQ-STD", BaseRatePercent = 5.9m, MinTermMonths = 12, MaxTermMonths = 60 },
                new RateCardEntity { ProductCode = "EQ-FLEX", BaseRatePercent = 6.4m, MinTermMonths = 6, MaxTermMonths = 48 });
        });

        modelBuilder.Entity<ResidualValueEntity>(e =>
        {
            e.ToTable("residual_values");
            e.HasKey(x => new { x.AssetType, x.TermMonths });
            e.HasData(
                new ResidualValueEntity { AssetType = "forklift", TermMonths = 36, Percent = 25m },
                new ResidualValueEntity { AssetType = "forklift", TermMonths = 48, Percent = 18m },
                new ResidualValueEntity { AssetType = "excavator", TermMonths = 36, Percent = 35m },
                new ResidualValueEntity { AssetType = "excavator", TermMonths = 48, Percent = 28m });
        });

        modelBuilder.Entity<QuoteEntity>(e =>
        {
            e.ToTable("quotes");
            e.HasKey(x => x.Id);
            e.Property(x => x.ProductCode).HasMaxLength(32);
            e.Property(x => x.AssetType).HasMaxLength(64);
        });
    }
}

public sealed class RateCardEntity
{
    public string ProductCode { get; set; } = "";

    public decimal BaseRatePercent { get; set; }

    public int MinTermMonths { get; set; }

    public int MaxTermMonths { get; set; }
}

public sealed class ResidualValueEntity
{
    public string AssetType { get; set; } = "";

    public int TermMonths { get; set; }

    public decimal Percent { get; set; }
}

public sealed class QuoteEntity
{
    public Guid Id { get; set; }

    public string ProductCode { get; set; } = "";

    public string AssetType { get; set; } = "";

    public decimal AssetValue { get; set; }

    public int TermMonths { get; set; }

    public decimal RatePercent { get; set; }

    public decimal ResidualPercent { get; set; }

    public decimal MonthlyPayment { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
