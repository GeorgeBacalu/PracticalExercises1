using MiniBankConsole.Dtos.Common;
using MiniBankConsole.Models;
using System.Globalization;
using System.Text.Json;

namespace MiniBankConsole.Helpers;
public static class DataManager
{
    public static readonly List<BankAccount> Accounts = [];
    public static ExchangeRatesDto ExchangeRates = new();
    public static readonly List<CurrencyDto> Currencies = [.. CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(culture => new RegionInfo(culture.Name)).Select(region => new CurrencyDto(region.ISOCurrencySymbol, region.CurrencyEnglishName)).DistinctBy(culture => culture.Code)];
    public static readonly List<LocaleDto> Locales = [.. CultureInfo.GetCultures(CultureTypes.AllCultures).Select(culture => new LocaleDto(culture.DisplayName, culture.Name))];

    private static readonly string AccountJsonPath = Path.GetFullPath("../../../accounts.json");
    private static readonly string ExchangeRatesJsonPath = Path.GetFullPath("../../../exchange-rates.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static void Load()
    {
        Accounts.Clear();
        if (File.Exists(AccountJsonPath)) Accounts.AddRange(JsonSerializer.Deserialize<List<BankAccount>>(File.ReadAllText(AccountJsonPath)) ?? []);
        if (File.Exists(ExchangeRatesJsonPath)) ExchangeRates = JsonSerializer.Deserialize<ExchangeRatesDto>(File.ReadAllText(ExchangeRatesJsonPath)) ?? new();
    }

    public static void Save() => File.WriteAllText(AccountJsonPath, JsonSerializer.Serialize(Accounts, JsonOptions));
}
