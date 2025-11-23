using MiniBank.App.Dtos;
using MiniBank.App.Exceptions;
using MiniBank.App.Helpers;
using MiniBank.App.Models;
using MiniBank.App.Services;
using MiniBank.Test.Mocks;
using MiniBank.Test.Unit.Fixtures;

namespace MiniBank.Test.Unit.Services;
[Collection("Console")] public class AccountRegistryTest
{
    public AccountRegistryTest()
    {
        DataManager.Accounts.Clear();
        AuthService.LogoutAsync().GetAwaiter().GetResult();
        DataManager.ExchangeRates = new ExchangeRatesDto { Rates = new() { ["USD"] = 1.0m, ["EUR"] = 0.9m, ["GBP"] = 0.8m } };
    }

    private static async Task LoginAsAsync(string user, string password)
    {
        if (!DataManager.Accounts.Any(account => account.Owner == user))
            DataManager.Accounts.Add(new CheckingAccount { Owner = user, Password = password, Currency = "USD", Locale = "en-US" });
        await ConsoleRunner.RunAsync(string.Join('\n', [user, password]), AuthService.LoginAsync);
    }

    [Fact] public async Task GetAccountsAsync_Should_PrintNoAccount_WhenNoneForCurrentUser()
    {
        await LoginAsAsync("User1", "123456");
        DataManager.Accounts.Clear();

        var output = await ConsoleRunner.RunAsync("", AccountRegistry.GetAccountsAsync);

        Assert.Contains("No accounts found", output);
    }

    [Fact] public async Task GetAccountsAsync_Should_ListOnlyCurrentUserAccounts()
    {
        DataManager.Accounts.AddRange(new CheckingAccount { Owner = "User1", Password = "123456" }, new SavingsAccount { Owner = "User2", Password = "234567" });
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync("", AccountRegistry.GetAccountsAsync);

        Assert.Contains("Bank accounts:", output);
        Assert.Contains("User1", output);
        Assert.DoesNotContain("User2", output);
    }

    [Fact] public async Task CreateAccountAsync_Should_CreateChecking_WithInitialDeposit_AndTransaction()
    {
        await LoginAsAsync("User1", "123456");
        var account = DataManager.Accounts.OfType<CheckingAccount>().Single(account => account.Owner == "User1");
        DataManager.Accounts.Remove(account);
        DataManager.Accounts.Add(new SavingsAccount { Owner = "User1", Password = "123456" });

        var output = await ConsoleRunner.RunAsync(string.Join('\n', ["1", "100", "USD", "en-US"]), AccountRegistry.CreateAccountAsync);

        account = DataManager.Accounts.OfType<CheckingAccount>().Single(account => account.Owner == "User1");
        Assert.Equal(100m, account.Balance);
        Assert.Single(account.Transactions);
        Assert.Equal(TransactionType.Deposit, account.Transactions[0].Type);
        Assert.Equal(100m, account.Transactions[0].Amount);
        Assert.Contains("Created CheckingAccount for User1 with balance $100.00", output);
    }

    [Fact] public async Task CreateAccountAsync_Should_CreateSavings_WithInitialDeposit_AndTransaction()
    {
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync(string.Join('\n', ["2", "100", "USD", "en-US"]), AccountRegistry.CreateAccountAsync);

        var account = DataManager.Accounts.OfType<SavingsAccount>().Single(account => account.Owner == "User1");
        Assert.Equal(100m, account.Balance);
        Assert.Single(account.Transactions);
        Assert.Equal(TransactionType.Deposit, account.Transactions[0].Type);
        Assert.Equal(100m, account.Transactions[0].Amount);
        Assert.Contains("Created SavingsAccount for User1 with balance $100.00", output);
    }

    [Fact] public async Task CreateAccountAsync_Should_CreateLoan_WithNegativeInitialBalance_AndWithdrawTransaction()
    {
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync(string.Join('\n', ["3", "100", "USD", "en-US"]), AccountRegistry.CreateAccountAsync);

        var account = DataManager.Accounts.OfType<LoanAccount>().Single(account => account.Owner == "User1");
        Assert.Equal(-100m, account.Balance);
        Assert.Single(account.Transactions);
        Assert.Equal(TransactionType.Withdraw, account.Transactions[0].Type);
        Assert.Equal(100m, account.Transactions[0].Amount);
        Assert.Contains("Created LoanAccount for User1 with balance ($100.00)", output);
    }

    [Fact] public async Task CreateAccountAsync_Should_CreateFixedDeposit_WithInitialDeposit_AndTransaction()
    {
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync(string.Join('\n', ["4", "100", "USD", "en-US"]), AccountRegistry.CreateAccountAsync);

        var account = DataManager.Accounts.OfType<FixedDepositAccount>().Single(account => account.Owner == "User1");
        Assert.Equal(100m, account.Balance);
        Assert.Single(account.Transactions);
        Assert.Equal(TransactionType.Deposit, account.Transactions[0].Type);
        Assert.Equal(100m, account.Transactions[0].Amount);
        Assert.Contains("Created FixedDepositAccount for User1 with balance $100.00", output);
    }

    [Fact] public async Task CreateAccountAsync_Should_Fail_WhenDuplicateTypeForUser()
    {
        DataManager.Accounts.Add(new CheckingAccount { Owner = "User1", Password = "123456" });
        await LoginAsAsync("User1", "123456");

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => ConsoleRunner.RunAsync(string.Join('\n', ["1", "50", "USD", "en-US"]), AccountRegistry.CreateAccountAsync));
        Assert.Contains("User already has an account of this type", exception.Message);
    }

    [Fact] public async Task DepositAsync_Should_DepositAmount_AndWriteMessage()
    {
        DataManager.Accounts.Add(AccountMock.NewCheckingAccount());
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync(string.Join('\n', ["1", "50"]), AccountRegistry.DepositAsync);

        var account = DataManager.Accounts.OfType<CheckingAccount>().Single(account => account.Owner == "User1");
        Assert.Equal(150m, account.Balance);
        Assert.Contains("Deposited $50.00 to User1 (CheckingAccount)", output);
    }

    [Fact] public async Task DepositAsync_Should_Throw_NotFound_WhenAccountMissing()
    {
        DataManager.Accounts.Add(AccountMock.NewCheckingAccount());
        await LoginAsAsync("User1", "123456");

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => ConsoleRunner.RunAsync(string.Join('\n', ["2", "50"]), AccountRegistry.DepositAsync));
        Assert.Contains("Account not found", exception.Message);
    }

    [Fact] public async Task WithdrawAsync_Should_WithdrawAmount_WhenAllowed()
    {
        DataManager.Accounts.Add(AccountMock.NewCheckingAccount());
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync(string.Join('\n', ["1", "50"]), AccountRegistry.WithdrawAsync);

        var account = DataManager.Accounts.OfType<CheckingAccount>().Single(account => account.Owner == "User1");
        Assert.Equal(50m, account.Balance);
        Assert.Contains("Withdrew $50.00 from User1 (CheckingAccount)", output);
    }

    [Fact] public async Task WithdrawAsync_Should_Fail_WhenInsufficientFunds()
    {
        DataManager.Accounts.Add(AccountMock.NewCheckingAccount());
        await LoginAsAsync("User1", "123456");

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => ConsoleRunner.RunAsync(string.Join('\n', ["1", "350"]), AccountRegistry.WithdrawAsync));
        Assert.Contains("Insufficient funds", exception.Message);
    }

    [Fact] public async Task WithdrawAsync_Should_Throw_NotFound_WhenAccountMissing()
    {
        DataManager.Accounts.Add(AccountMock.NewCheckingAccount());
        await LoginAsAsync("User1", "123456");

        var exception = await Assert.ThrowsAsync<NotFoundException>(() => ConsoleRunner.RunAsync(string.Join('\n', ["2", "50"]), AccountRegistry.WithdrawAsync));
        Assert.Contains("Account not found", exception.Message);
    }

    [Fact] public async Task ViewStatementAsync_Should_PrintStatement()
    {
        var account = AccountMock.NewCheckingAccount();
        DataManager.Accounts.Add(account);
        account.Deposit(100m);
        account.Withdraw(100m, out _);
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync("1", AccountRegistry.ViewStatementAsync);
        Assert.Contains("CheckingAccount statement for User1", output);
        Assert.Contains("Deposit - $100.00", output);
        Assert.Contains("Withdraw - $100.00", output);
    }

    [Fact] public async Task RunMonthEndProcessingAsync_Should_ApplyInterestToAllInterestBearingAccounts()
    {
        DataManager.Accounts.AddRange(AccountMock.NewCheckingAccount(), AccountMock.NewSavingsAccount(), AccountMock.NewLoanAccount());
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync("", AccountRegistry.RunMonthEndProcessingAsync);

        var checkingAccount = DataManager.Accounts.OfType<CheckingAccount>().Single(account => account.Owner == "User1");
        var savingsAccount = DataManager.Accounts.OfType<SavingsAccount>().Single(account => account.Owner == "User1");
        var loanAccount = DataManager.Accounts.OfType<LoanAccount>().Single(account => account.Owner == "User1");
        Assert.True(savingsAccount.Balance > 100m);
        Assert.True(loanAccount.Balance < -50m);
        Assert.Equal(100m, checkingAccount.Balance);
        Assert.Contains("Month-end applied (interest/fees)", output);
    }

    [Fact] public async Task TransferAsync_SameCurrency_Should_MoveFunds_AndWriteMessage()
    {
        DataManager.Accounts.AddRange(AccountMock.NewCheckingAccount(), AccountMock.NewSavingsAccount());
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync(string.Join('\n', ["User1", "1", "2", "50"]), AccountRegistry.TransferAsync);

        var checkingAccount = DataManager.Accounts.OfType<CheckingAccount>().Single(account => account.Owner == "User1");
        var savingsAccount = DataManager.Accounts.OfType<SavingsAccount>().Single(account => account.Owner == "User1");
        Assert.Equal(50m, checkingAccount.Balance);
        Assert.Equal(150m, savingsAccount.Balance);
        Assert.Contains("Transferred $50.00 from User1 (CheckingAccount) to User1 (SavingsAccount)", output);
    }

    [Fact] public async Task TransferAsync_CrossCurrency_Should_ConvertUsingRates()
    {
        DataManager.Accounts.AddRange(AccountMock.NewCheckingAccount(), AccountMock.NewSavingsAccount2());
        await LoginAsAsync("User1", "123456");

        var output = await ConsoleRunner.RunAsync(string.Join('\n', ["User1", "1", "2", "50"]), AccountRegistry.TransferAsync);

        var checkingAccount = DataManager.Accounts.OfType<CheckingAccount>().Single(account => account.Owner == "User1");
        var savingsAccount = DataManager.Accounts.OfType<SavingsAccount>().Single(account => account.Owner == "User1");
        Assert.Equal(50m, checkingAccount.Balance);
        Assert.Equal(100m + 50m * DataManager.ExchangeRates.Rates["EUR"], savingsAccount.Balance);
        Assert.Contains("Transferred $50.00 from User1 (CheckingAccount) to User1 (SavingsAccount) credited as 45,00 €", output);
    }

    [Fact] public async Task TransferAsync_Should_Fail_WhenSameSenderAndReceiverSameType()
    {
        DataManager.Accounts.Add(AccountMock.NewCheckingAccount());
        await LoginAsAsync("User1", "123456");

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => ConsoleRunner.RunAsync(string.Join('\n', ["User1", "1", "1", "50"]), AccountRegistry.TransferAsync));
        Assert.Equal("Sender and receiver must be different", exception.Message);
    }

    [Fact] public async Task TransferAsync_Should_Fail_WhenInsufficientFundsOnSender()
    {
        DataManager.Accounts.AddRange(AccountMock.NewCheckingAccount(), AccountMock.NewSavingsAccount2());
        await LoginAsAsync("User1", "123456");

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => ConsoleRunner.RunAsync(string.Join('\n', ["User1", "1", "2", "350"]), AccountRegistry.TransferAsync));
        Assert.Equal("Insufficient funds", exception.Message);
    }

    [Fact] public async Task TransferAsync_Should_Fail_WhenExceedsLoanPayoff()
    {
        DataManager.Accounts.AddRange(AccountMock.NewCheckingAccount(), AccountMock.NewLoanAccount());
        await LoginAsAsync("User1", "123456");

        var exception = await Assert.ThrowsAsync<BadRequestException>(() => ConsoleRunner.RunAsync(string.Join('\n', ["User1", "1", "3", "100"]), AccountRegistry.TransferAsync));
        Assert.Contains("Transfer exceeds loan payoff amount", exception.Message);
    }
}
