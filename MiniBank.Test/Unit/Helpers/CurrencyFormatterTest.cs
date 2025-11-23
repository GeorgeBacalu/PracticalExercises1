using MiniBank.App.Helpers;
using MiniBank.Test.Mocks;

namespace MiniBank.Test.Unit.Helpers;
public class CurrencyFormatterTest
{
    [Theory, MemberData(nameof(AccountMock.LocalizedCurrencies), MemberType = typeof(AccountMock))]
    public void Format_Should_DisplayLocalizedCurrency(decimal amount, string currency, string locale, string expectedPrefix) => Assert.Contains(expectedPrefix[..2].Trim(), CurrencyFormatter.Format(amount, currency, locale));

    [Fact] public void Format_Should_UseCurrencyCode_When_LocaleDoesNotMatch() => Assert.Equal("USD1,000.00", CurrencyFormatter.Format(AccountMock.DepositAmount, "USD", "xx-XX"));

    [Fact] public void Format_Should_HandleNegativeValues() => Assert.Equal("($1,000.00)", CurrencyFormatter.Format(AccountMock.NegativeDepositAmount, "USD", "en-US"));

    [Fact] public void Format_Should_Work_WithDifferentNumberGrouping()
    {
        Assert.Contains(",", CurrencyFormatter.Format(AccountMock.DepositAmount, "USD", "en-US"));
        Assert.Contains(" ", CurrencyFormatter.Format(AccountMock.DepositAmount, "EUR", "fr-FR"));
    }

    [Fact] public void Format_Should_RespectLocaleDecimalSeparator()
    {
        Assert.Contains(".", CurrencyFormatter.Format(AccountMock.DepositAmount, "USD", "en-US"));
        Assert.Contains(",", CurrencyFormatter.Format(AccountMock.DepositAmount, "EUR", "fr-FR"));
    }
}
