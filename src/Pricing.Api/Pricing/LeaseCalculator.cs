namespace Pricing.Api.Pricing;

public static class LeaseCalculator
{
    /// <summary>Monthly payment of an annuity lease with a balloon residual value at the end of the term.</summary>
    public static decimal MonthlyPayment(decimal assetValue, decimal annualRatePercent, int termMonths, decimal residualPercent)
    {
        var residual = assetValue * residualPercent / 100m;
        var r = (double)annualRatePercent / 100d / 12d;
        if (r == 0)
        {
            return Math.Round((assetValue - residual) / termMonths, 2);
        }

        var factor = Math.Pow(1 + r, termMonths);
        var payment = ((double)assetValue - (double)residual / factor) * r / (1 - 1 / factor);
        return Math.Round((decimal)payment, 2);
    }
}
