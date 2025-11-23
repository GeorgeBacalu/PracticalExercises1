using MiniBank.App.Dtos;
using MiniBank.App.Models;
using System.Globalization;
using System.Text.Json;

namespace MiniBank.App.Helpers;
public static class DataManager
{
    public static readonly List<BankAccount> Accounts = [];
    public static ExchangeRatesDto ExchangeRates = new();
    public static readonly List<CurrencyDto> Currencies = [.. CultureInfo.GetCultures(CultureTypes.SpecificCultures).AsParallel().Select(culture => new RegionInfo(culture.Name)).Select(region => new CurrencyDto(region.ISOCurrencySymbol, region.CurrencyEnglishName)).DistinctBy(culture => culture.Code)];
    public static readonly List<LocaleDto> Locales = [.. CultureInfo.GetCultures(CultureTypes.AllCultures).AsParallel().Select(culture => new LocaleDto(culture.Name, culture.DisplayName))];

    private static readonly string _jsonAccountsPath = Path.GetFullPath("../../../Data/accounts.json");
    private static readonly string _jsonExchangeRatesPath = Path.GetFullPath("../../../Data/exchange-rates.json");
    private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

    public static async Task LoadAsync()
    {
        Accounts.Clear();
        if (File.Exists(_jsonAccountsPath)) Accounts.AddRange(JsonSerializer.Deserialize<List<BankAccount>>(await File.ReadAllTextAsync(_jsonAccountsPath)) ?? []);
        if (File.Exists(_jsonExchangeRatesPath)) ExchangeRates = JsonSerializer.Deserialize<ExchangeRatesDto>(await File.ReadAllTextAsync(_jsonExchangeRatesPath)) ?? new();
    }

    public static async Task SaveAsync() => await File.WriteAllTextAsync(_jsonAccountsPath, JsonSerializer.Serialize(Accounts, _jsonOptions));
}
