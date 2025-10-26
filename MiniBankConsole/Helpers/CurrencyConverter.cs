using MiniBankConsole.Exceptions;

namespace MiniBankConsole.Helpers;
public static class CurrencyConverter
{
    public static decimal Convert(decimal amount, string from, string to)
    {
        if (from == to) return amount;
        if (!DataManager.ExchangeRates.Rates.TryGetValue(from, out var fromRate)) throw new BadRequestException($"Missing exchange rate for {from}");
        if (!DataManager.ExchangeRates.Rates.TryGetValue(to, out var toRate)) throw new BadRequestException($"Missing exchange rate for {to}");
        return Math.Round(amount * (toRate / fromRate), 2);
    }
}
