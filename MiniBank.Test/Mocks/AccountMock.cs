using MiniBank.App.Models;

namespace MiniBank.Test.Mocks;
public static class AccountMock
{
    public static List<BankAccount> Accounts() => [CheckingAccountMock(), SavingsAccountMock(), LoanAccountMock(), FixedDepositAccountMock()];

    public static CheckingAccount CheckingAccountMock() => new() { Id = Guid.Parse("998ec7bb-abb4-4631-8b65-3e72327c8c9f"), Owner = "User1", Password = "user1", Balance = 1000m };
    public static SavingsAccount SavingsAccountMock() => new() { Id = Guid.Parse("1cdb89e3-4186-449e-8fdf-6ae61f3cef7a"), Owner = "User2", Password = "user2", Balance = 1500m };
    public static LoanAccount LoanAccountMock() => new() { Id = Guid.Parse("daaf94a6-15da-43fb-9422-20a019878f38"), Owner = "User3", Password = "user3", Balance = -1000m };
    public static FixedDepositAccount FixedDepositAccountMock() => new() { Id = Guid.Parse("2cf2208b-ea38-4404-8870-21ae5c1d2a03"), Owner = "User4", Password = "user4", Balance = 2000m };

    public const decimal DepositAmount = 1000m;
    public const decimal DepositAmountOverPayoffLimit = 1001m;
    public const decimal NegativeDepositAmount = -1000m;
    public const decimal WithdrawAmount = 500m;
    public const decimal WithdrawAmountWithinLimit = 1000m;
    public const decimal WithdrawAmountOverLimitChecking = 1201m;
    public const decimal WithdrawAmountOverLimitNonChecking = 2001m;
    public const decimal NegativeWithdrawAmount = -1000m;
}
