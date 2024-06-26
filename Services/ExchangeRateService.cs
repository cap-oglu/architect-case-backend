using FinancialManagementMVC.Models;


namespace FinancialManagementMVC.Services;
public interface IExchangeRateService
{
    ExchangeRate GetExchangeRate(string baseCurrency, string targetCurrency);
}

public class MockExchangeRateService : IExchangeRateService
{
    public ExchangeRate GetExchangeRate(string baseCurrency, string targetCurrency)
    {
        // Return a mock exchange rate
        return new ExchangeRate
        {
            BaseCurrency = baseCurrency,
            TargetCurrency = targetCurrency,
            Rate = new Random().NextDouble() * (1.5 - 1.0) + 1.0  // Random rate between 1.0 and 1.5
        };
    }
}
