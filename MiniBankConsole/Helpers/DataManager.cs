using MiniBankConsole.Dtos;
using MiniBankConsole.Models;
using System.Globalization;
using System.Text.Json;

namespace MiniBankConsole.Helpers;
public static class DataManager
{
    public static readonly List<BankAccount> Accounts = [];
    public static ExchangeRatesDto ExchangeRates = new();
    public static readonly List<CurrencyDto> Currencies = [.. CultureInfo.GetCultures(CultureTypes.SpecificCultures).AsParallel().Select(culture => new RegionInfo(culture.Name)).Select(region => new CurrencyDto(region.ISOCurrencySymbol, region.CurrencyEnglishName)).DistinctBy(culture => culture.Code)];
    public static readonly List<LocaleDto> Locales = [.. CultureInfo.GetCultures(CultureTypes.AllCultures).AsParallel().Select(culture => new LocaleDto(culture.Name, culture.DisplayName))];

    private static readonly string AccountJsonPath = Path.GetFullPath("../../../accounts.json");
    private static readonly string ExchangeRatesJsonPath = Path.GetFullPath("../../../exchange-rates.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static async Task LoadAsync()
    {
        Accounts.Clear();
        if (File.Exists(AccountJsonPath)) Accounts.AddRange(JsonSerializer.Deserialize<List<BankAccount>>(await File.ReadAllTextAsync(AccountJsonPath)) ?? []);
        if (File.Exists(ExchangeRatesJsonPath)) ExchangeRates = JsonSerializer.Deserialize<ExchangeRatesDto>(await File.ReadAllTextAsync(ExchangeRatesJsonPath)) ?? new();
    }

    public static async Task SaveAsync() => await File.WriteAllTextAsync(AccountJsonPath, JsonSerializer.Serialize(Accounts, JsonOptions));
}
