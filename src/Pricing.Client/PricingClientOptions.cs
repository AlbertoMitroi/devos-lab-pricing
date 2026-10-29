namespace Pricing.Client;

public sealed class PricingClientOptions
{
    public const string SectionName = "Clients:Pricing";

    public Uri? BaseAddress { get; set; }
}
