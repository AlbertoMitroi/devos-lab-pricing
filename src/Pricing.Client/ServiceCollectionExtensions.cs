using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Pricing.Client;

public static class ServiceCollectionExtensions
{
    /// <summary>Registers <see cref="IPricingClient"/>; the base address comes from <c>Clients:Pricing:BaseAddress</c>.</summary>
    public static IHttpClientBuilder AddPricingClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PricingClientOptions>(configuration.GetSection(PricingClientOptions.SectionName));
        return services.AddHttpClient<IPricingClient, PricingClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<PricingClientOptions>>().Value;
            client.BaseAddress = options.BaseAddress
                ?? throw new InvalidOperationException($"{PricingClientOptions.SectionName}:BaseAddress is not configured.");
        });
    }
}
