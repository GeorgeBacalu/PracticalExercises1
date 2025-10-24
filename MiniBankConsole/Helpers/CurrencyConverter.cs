using MiniBankConsole.Exceptions;

namespace MiniBankConsole.Helpers;
public static class CurrencyConverter
{
    public static decimal Convert(decimal amount, string fromCurrency, string toCurrency)
    {
        if (fromCurrency == toCurrency) return amount;
        if (!DataManager.ExchangeRates.Rates.TryGetValue(fromCurrency, out var fromRate)) throw new BadRequestException($"Missing exchange rate for {fromCurrency}");
        if (!DataManager.ExchangeRates.Rates.TryGetValue(toCurrency, out var toRate)) throw new BadRequestException($"Missing exchange rate for {toCurrency}");
        return Math.Round(amount * (toRate / fromRate), 2);
    }
}
