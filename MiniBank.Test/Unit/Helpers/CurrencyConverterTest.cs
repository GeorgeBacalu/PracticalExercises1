using MiniBank.App.Dtos;
using MiniBank.App.Exceptions;
using MiniBank.App.Helpers;
using MiniBank.Test.Mocks;

namespace MiniBank.Test.Unit.Helpers;
public class CurrencyConverterTest
{
    public CurrencyConverterTest() => DataManager.ExchangeRates = new ExchangeRatesDto { Rates = new() { ["USD"] = 1.0m, ["EUR"] = 0.9m, ["GBP"] = 0.8m } };

    [Fact] public void Convert_SameCurrency_Should_ReturnSameAmount() => Assert.Equal(AccountMock.Amount, CurrencyConverter.Convert(AccountMock.Amount, "USD", "USD"));

    [Fact] public void Convert_USDtoEUR_Should_ApplyRate() => Assert.Equal(AccountMock.Amount * 0.9m, CurrencyConverter.Convert(AccountMock.Amount, "USD", "EUR"));

    [Fact] public void Convert_Should_Fail_WhenMissingRate() => Assert.Throws<BadRequestException>(() => CurrencyConverter.Convert(100, "JPY", "USD"));
}
