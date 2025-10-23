using MiniBankConsole.Dtos.Common;
using MiniBankConsole.Models;
using System.Globalization;
using System.Text.Json;

namespace MiniBankConsole.Helpers;
public static class DataManager
{
    public static readonly List<BankAccount> Accounts = [];
    public static readonly List<CurrencyDto> Currencies = [.. CultureInfo.GetCultures(CultureTypes.SpecificCultures).Select(culture => new RegionInfo(culture.Name)).Select(region => new CurrencyDto(region.ISOCurrencySymbol, region.CurrencyEnglishName)).DistinctBy(culture => culture.Code)];
    public static readonly List<LocaleDto> Locales = [.. CultureInfo.GetCultures(CultureTypes.AllCultures).Select(culture => new LocaleDto(Code: culture.Name, Name: culture.DisplayName))];

    private static readonly string JsonPath = Path.GetFullPath("../../../accounts.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static void Load()
    {
        Accounts.Clear();
        if (File.Exists(JsonPath)) Accounts.AddRange(JsonSerializer.Deserialize<List<BankAccount>>(File.ReadAllText(JsonPath)) ?? []);
    }

    public static void Save() => File.WriteAllText(JsonPath, JsonSerializer.Serialize(Accounts, JsonOptions));
}
