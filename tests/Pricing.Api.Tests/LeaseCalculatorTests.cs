using FluentAssertions;
using Pricing.Api.Pricing;

namespace Pricing.Api.Tests;

public class LeaseCalculatorTests
{
    [Fact]
    public void Zero_rate_spreads_the_depreciation_evenly()
    {
        LeaseCalculator.MonthlyPayment(36_000m, 0m, 36, 25m).Should().Be(750m);
    }

    [Fact]
    public void Payment_with_interest_is_higher_than_without()
    {
        var withInterest = LeaseCalculator.MonthlyPayment(36_000m, 5.9m, 36, 25m);

        withInterest.Should().BeGreaterThan(750m).And.BeLessThan(1_000m);
    }
}
