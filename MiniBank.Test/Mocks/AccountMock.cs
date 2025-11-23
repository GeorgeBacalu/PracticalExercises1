using MiniBank.App.Models;

namespace MiniBank.Test.Mocks;
public static class AccountMock
{
    public static List<BankAccount> Accounts() => [CheckingAccountMock(), SavingsAccountMock(), LoanAccountMock(), FixedDepositAccountMock()];

    public static CheckingAccount CheckingAccountMock() => new() { Id = Guid.Parse("998ec7bb-abb4-4631-8b65-3e72327c8c9f"), Owner = "User1", Password = "user1", Balance = 1000m };
    public static SavingsAccount SavingsAccountMock() => new() { Id = Guid.Parse("1cdb89e3-4186-449e-8fdf-6ae61f3cef7a"), Owner = "User2", Password = "user2", Balance = 1500m };
    public static LoanAccount LoanAccountMock() => new() { Id = Guid.Parse("daaf94a6-15da-43fb-9422-20a019878f38"), Owner = "User3", Password = "user3", Balance = -1000m };
    public static FixedDepositAccount FixedDepositAccountMock() => new() { Id = Guid.Parse("2cf2208b-ea38-4404-8870-21ae5c1d2a03"), Owner = "User4", Password = "user4", Balance = 2000m };

    public static CheckingAccount NewCheckingAccount() => new() { Owner = "User1", Password = "123456", Currency = "USD", Locale = "en-US", Balance = 100m };
    public static SavingsAccount NewSavingsAccount() => new() { Owner = "User1", Password = "123456", Currency = "USD", Locale = "en-US", Balance = 100m };
    public static SavingsAccount NewSavingsAccount2() => new() { Owner = "User1", Password = "123456", Currency = "EUR", Locale = "fr-FR", Balance = 100m };
    public static LoanAccount NewLoanAccount() => new() { Owner = "User1", Password = "123456", Currency = "USD", Locale = "en-US", Balance = -50m };
    public static CheckingAccount NewFixedDepositAccount() => new() { Owner = "User1", Password = "123456", Currency = "USD", Locale = "en-US", Balance = 100m };

    public const decimal Amount = 1000m;
    public const decimal DepositAmount = 1000m;
    public const decimal DepositAmountOverPayoffLimit = 1001m;
    public const decimal NegativeDepositAmount = -1000m;
    public const decimal WithdrawAmount = 500m;
    public const decimal WithdrawAmountWithinLimit = 1000m;
    public const decimal WithdrawAmountOverLimitChecking = 1201m;
    public const decimal WithdrawAmountOverLimitNonChecking = 2001m;
    public const decimal NegativeWithdrawAmount = -1000m;

    public static IEnumerable<object[]> LocalizedCurrencies => [[100m, "USD", "en-US", "$100.00"], [100m, "EUR", "fr-FR", "100,00€"], [100m, "JPY", "ja-JP", "￥100"]];
    public static IEnumerable<object[]> AccountTypeNames => [[1, nameof(CheckingAccount)], [2, nameof(SavingsAccount)], [3, nameof(LoanAccount)], [4, nameof(FixedDepositAccount)]];
}
