using MiniBank.App.Exceptions;
using MiniBank.App.Helpers;
using MiniBank.Test.Mocks;
using MiniBank.Test.Unit.Fixtures;

namespace MiniBank.Test.Unit.Helpers;
[Collection("Console")] public class InputHelperTest
{
    public InputHelperTest() => DataManager.Accounts.Clear();

    [Fact] public async Task GetAccountTypeAsync_Should_Repeat_OnNonNumeric_AndOutOfRange()
    {
        var (type, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["abc", "0", "1"]), () => InputHelper.GetAccountTypeAsync());
        
        Assert.Contains("Account type must be numeric", output);
        Assert.Contains("Account type must be between 1 and 4", output);
        Assert.Equal(1, type);
    }

    [Fact] public async Task GetUsernameAsync_Register_Should_RejectDuplicateForType()
    {
        DataManager.Accounts.Add(AccountMock.CheckingAccountMock());

        var (username, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["", "User1", "User0"]), () => InputHelper.GetUsernameAsync(type: 1, isRegister: true));
        
        Assert.Contains("Username is required", output);
        Assert.Contains("User already has an account of this type", output);
        Assert.Equal("User0", username);
    }

    [Fact] public async Task GetUsernameAsync_Login_Should_RejectEmpty_ThenReturnValue()
    {
        var (username, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["", "User1"]), () => InputHelper.GetUsernameAsync(isRegister: false));
        
        Assert.Contains("Username is required", output);
        Assert.Equal("User1", username);
    }

    [Fact] public async Task GetPasswordAsync_Register_Should_ValidateRequiredAndLength()
    {
        var (pasword, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["", "12345", "123456"]), () => InputHelper.GetPasswordAsync(isRegister: true));
        
        Assert.Contains("Password is required", output);
        Assert.Contains("Password must be at least 6 characters long", output);
        Assert.Equal("123456", pasword);
    }

    [Fact] public async Task GetPasswordAsync_Login_Should_AllowShorterPasswordsIfProvided()
    {
        var (password, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["123"]), () => InputHelper.GetPasswordAsync(isRegister: false));
        
        Assert.DoesNotContain("at least 6 characters", output);
        Assert.Equal("123", password);
    }

    [Fact] public async Task GetCurrencyAsync_Should_RejectEmptyAndInvalid_ThenAcceptValid()
    {
        var (currency, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["", "XXX", "USD"]), InputHelper.GetCurrencyAsync);

        Assert.Contains("Currency is required", output);
        Assert.Contains("Invalid currency", output);
        Assert.Equal("USD", currency);
    }

    [Fact] public async Task GetLocaleAsync_Should_RejectEmptyAndInvalid_ThenAcceptValid()
    {
        var (locale, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["", "XXX", "en-US"]), InputHelper.GetLocaleAsync);

        Assert.Contains("Locale is required", output);
        Assert.Contains("Invalid locale", output);
        Assert.Equal("en-US", locale);
    }

    [Fact] public async Task GetOpeningDepositAsync_Should_Repeat_OnNonNumeric_AndNegative()
    {
        var (amount, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["abc", "-10", "50"]), InputHelper.GetOpeningDepositAsync);
        
        Assert.Contains("Opening deposit must be numeric", output);
        Assert.Contains("Opening deposit must be positive", output);
        Assert.Equal(50m, amount);
    }

    [Fact] public async Task GetAmountAsync_Should_Repeat_OnNonNumeric_AndNonPositive()
    {
        var (amount, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["abc", "-10", "50"]), () => InputHelper.GetAmountAsync("transfer"));

        Assert.Contains("Amount must be numeric", output);
        Assert.Contains("Amount must be positive", output);
        Assert.Equal(50m, amount);
    }

    [Fact] public async Task GetReceiverAsync_Should_RejectEmpty_ThenReturnValue()
    {
        var (receiver, output) = await ConsoleRunner.RunAsync(string.Join('\n', ["", "User1"]), InputHelper.GetReceiverAsync);
        Assert.Contains("Receiver name is required", output);
        Assert.Equal("User1", receiver);
    }

    [Theory, MemberData(nameof(AccountMock.AccountTypeNames), MemberType = typeof(AccountMock))]
    public void GetTypeName_Should_ReturnCorrectNames(int type, string name) => Assert.Equal(name, InputHelper.GetTypeName(type));

    [Fact] public void GetTypeName_Should_Fail_OnInvalidType() => Assert.Throws<BadRequestException>(() => InputHelper.GetTypeName(0));
}
