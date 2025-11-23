using MiniBank.App.Exceptions;
using MiniBank.App.Helpers;
using MiniBank.App.Models;
using MiniBank.App.Services;
using MiniBank.Test.Mocks;
using MiniBank.Test.Unit.Fixtures;

namespace MiniBank.Test.Unit.Services;
[Collection("Console")] public class AuthServiceTest
{
    public AuthServiceTest()
    {
        DataManager.Accounts.Clear();
        AuthService.LogoutAsync().GetAwaiter().GetResult();
    }

    [Fact] public async Task RegisterAsync_Should_CreateAccount_AndWriteConfirmation()
    {
        var result = await ConsoleRunner.RunAsync(string.Join('\n', ["1", "User1", "123456", "USD", "en-US"]), AuthService.RegisterAsync);

        var account = DataManager.Accounts.Single(account => account.Owner == "User1");
        Assert.IsType<CheckingAccount>(account);
        Assert.Equal("User1", account.Owner);
        Assert.Equal("123456", account.Password);
        Assert.Equal("USD", account.Currency);
        Assert.Equal("en-US", account.Locale);
        Assert.Contains("Registered CheckingAccount for User1", result);
        Assert.False(AuthService.IsAuthenticated);
        Assert.Equal("", AuthService.CurrentUser);
    }

    [Fact] public async Task LoginAsync_Should_SetSession_AndWriteBanner_OnSuccess()
    {
        DataManager.Accounts.Add(AccountMock.NewCheckingAccount());

        var result = await ConsoleRunner.RunAsync(string.Join('\n', ["User1", "123456"]), AuthService.LoginAsync);

        Assert.True(AuthService.IsAuthenticated);
        Assert.Equal("User1", AuthService.CurrentUser);
        Assert.Contains("Logged in as User1 (CheckingAccount)", result);
    }

    [Fact] public async Task LoginAsync_Should_Fail_WhenUserNotFound()
    {
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => ConsoleRunner.RunAsync(string.Join('\n', ["unknownuser", "123456"]), AuthService.LoginAsync));
        Assert.Equal("User not found", exception.Message);
        Assert.False(AuthService.IsAuthenticated);
        Assert.Equal("", AuthService.CurrentUser);
    }

    [Fact] public async Task LoginAsync_Should_Fail_WhenInvalidCredentials()
    {
        DataManager.Accounts.Add(AccountMock.NewCheckingAccount());
        
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => ConsoleRunner.RunAsync(string.Join('\n', ["User1", "wrongpassword"]), AuthService.LoginAsync));
        Assert.Equal("Invalid credentials", exception.Message);
        Assert.False(AuthService.IsAuthenticated);
        Assert.Equal("", AuthService.CurrentUser);
    }

    [Fact] public async Task LogoutAsync_Should_ClearSession_AndWriteMessage()
    {
        DataManager.Accounts.Add(AccountMock.NewCheckingAccount());

        await ConsoleRunner.RunAsync(string.Join('\n', ["User1", "123456"]), AuthService.LoginAsync);
        var result = await ConsoleRunner.RunAsync("", AuthService.LogoutAsync);

        Assert.False(AuthService.IsAuthenticated);
        Assert.Equal("", AuthService.CurrentUser);
        Assert.Contains("Logged out", result);
    }
}
