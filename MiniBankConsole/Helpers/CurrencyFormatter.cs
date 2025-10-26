using System.Globalization;

namespace MiniBankConsole.Helpers;
public static class CurrencyFormatter
{
    public static string Format(decimal amount, string currency, string locale)
    {
        var culture = new CultureInfo(locale);
        var region = new RegionInfo(culture.Name);
        culture.NumberFormat.CurrencySymbol = region.ISOCurrencySymbol == currency ? region.CurrencySymbol : currency;
        return string.Format(culture, "{0:C}", amount);
    }
}
