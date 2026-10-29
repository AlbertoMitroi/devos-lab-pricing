using Microsoft.EntityFrameworkCore;
using Pricing.Api.Data;
using Pricing.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddDbContext<PricingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PricingDb")));
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Configuration.GetValue<bool>("Database:EnsureCreated"))
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetRequiredService<PricingDbContext>().Database.EnsureCreated();
}

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapRateCardEndpoints();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
