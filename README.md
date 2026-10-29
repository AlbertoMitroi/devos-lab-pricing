# lab-pricing

Pricing API of the equipment-leasing lab and its client package `Pricing.Client`, published to GitHub Packages from this repository (`https://nuget.pkg.github.com/AlbertoMitroi/index.json`).

| Endpoint | Kind | Used by |
|---|---|---|
| `POST api/quotes` | controller `QuotesController.Calculate` | `IPricingClient.CalculateQuoteAsync` |
| `GET api/quotes/{id}` | controller `QuotesController.Get` | `IPricingClient.GetQuoteAsync` |
| `POST api/quotes/{id}/recalculate` | controller `QuotesController.Recalculate` | `IPricingClient.RecalculateQuoteAsync` (since 1.1.0) |
| `GET api/rate-cards/{productCode}` | minimal API | `Lab.Core.Clients.Pricing` (lab-core) |
| `GET api/residual-values` | minimal API | `Lab.Core.Clients.Pricing` (lab-core) |

Storage: EF Core on PostgreSQL (`ConnectionStrings:PricingDb`, tables `rate_cards`, `residual_values`, `quotes`).

## Build

Docker only:

```bash
docker build --target test .
docker build --target packages --build-arg VERSION=1.1.0 --output type=local,dest=out .
docker build --target runtime -t lab-pricing .
```

A tag `v*` publishes `Pricing.Client` from GitHub Actions.
