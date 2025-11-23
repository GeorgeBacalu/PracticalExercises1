using MiniBank.App.Constants;
using MiniBank.App.Exceptions;
using MiniBank.App.Models;
using MiniBank.Test.Mocks;

namespace MiniBank.Test.Unit.Models;
public class SavingsAccountTest
{
    private readonly SavingsAccount _account;
    private readonly decimal _initialBalance;

    public SavingsAccountTest() => _initialBalance = (_account = AccountMock.SavingsAccountMock()).Balance;

    [Fact] public void Deposit_Should_IncreaseBalance_WhenAmountIsPositive()
    {
        _account.Deposit(AccountMock.DepositAmount);

        Assert.Equal(_initialBalance + AccountMock.DepositAmount, _account.Balance);
        Assert.Single(_account.Transactions);
        Assert.Equal(TransactionType.Deposit, _account.Transactions[0].Type);
        Assert.Equal(AccountMock.DepositAmount, _account.Transactions[0].Amount);
    }

    [Fact] public void Deposit_Should_Fail_WhenAmountIsNegative()
    {
        var exception = Assert.Throws<BadRequestException>(() => _account.Deposit(AccountMock.NegativeDepositAmount));
        Assert.Equal("Deposit amount must be positive", exception.Message);
        Assert.Equal(_initialBalance, _account.Balance);
        Assert.Empty(_account.Transactions);
    }

    [Fact] public void Withdraw_Should_DecreaseBalance_WhenWithinLimit()
    {
        bool result = _account.Withdraw(AccountMock.WithdrawAmountWithinLimit, out var error);

        Assert.True(result);
        Assert.Null(error);
        Assert.Equal(_initialBalance - AccountMock.WithdrawAmountWithinLimit, _account.Balance);
        Assert.Single(_account.Transactions);
        Assert.Equal(TransactionType.Withdraw, _account.Transactions[0].Type);
        Assert.Equal(AccountMock.WithdrawAmountWithinLimit, _account.Transactions[0].Amount);
    }

    [Fact] public void Withdraw_Should_Fail_WhenOverLimit()
    {
        bool result = _account.Withdraw(AccountMock.WithdrawAmountOverLimitNonChecking, out var error);

        Assert.False(result);
        Assert.Equal("Insufficient funds", error);
        Assert.Equal(_initialBalance, _account.Balance);
        Assert.Empty(_account.Transactions);
    }

    [Fact] public void Withdraw_Should_Fail_WhenAmountIsNegative()
    {
        bool result = _account.Withdraw(AccountMock.NegativeWithdrawAmount, out var error);

        Assert.False(result);
        Assert.Equal("Withdraw amount must be positive", error);
        Assert.Equal(_initialBalance, _account.Balance);
        Assert.Empty(_account.Transactions);
    }

    [Fact] public void ApplyMonthlyInterest_Should_IncreaseBalance()
    {
        _account.ApplyMonthlyInterest();

        Assert.Equal(_initialBalance + _initialBalance * Constants.InterestRate, _account.Balance);
        Assert.Single(_account.Transactions);
        Assert.Equal(TransactionType.Interest, _account.Transactions[0].Type);
        Assert.Equal(_initialBalance * Constants.InterestRate, _account.Transactions[0].Amount);
    }
}
