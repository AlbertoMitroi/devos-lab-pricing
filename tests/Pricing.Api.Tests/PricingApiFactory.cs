using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Pricing.Api.Data;

namespace Pricing.Api.Tests;

public sealed class PricingApiFactory : WebApplicationFactory<Program>
{
    private readonly string _database = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<PricingDbContext>>();
            services.AddDbContext<PricingDbContext>(o => o.UseInMemoryDatabase(_database));
        });
    }

    /// <summary>Creates the in-memory database with its seed data.</summary>
    public void EnsureDatabase()
    {
        using var scope = Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<PricingDbContext>().Database.EnsureCreated();
    }
}
