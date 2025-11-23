using MiniBank.App.Constants;
using MiniBank.App.Exceptions;
using MiniBank.App.Models;
using MiniBank.Test.Mocks;

namespace MiniBank.Test.Unit.Models;
public class FixedDepositAccountTest
{
    private readonly FixedDepositAccount _account;
    private readonly decimal _initialBalance;

    public FixedDepositAccountTest() => _initialBalance = (_account = AccountMock.FixedDepositAccountMock()).Balance;

    [Fact] public void Deposit_Should_ThrowException_Always()
    {
        var exception = Assert.Throws<BadRequestException>(() => _account.Deposit(default));
        Assert.Equal("Can't deposit into a fixed deposit account after creation", exception.Message);
        Assert.Equal(_initialBalance, _account.Balance);
        Assert.Empty(_account.Transactions);
    }

    [Fact] public void Withdraw_Should_ApplyPenalty_OnEarlyWithdrawal()
    {
        bool result = _account.Withdraw(AccountMock.WithdrawAmount, out var error);

        Assert.True(result);
        Assert.Null(error);
        Assert.Equal(_initialBalance - (AccountMock.WithdrawAmount + AccountMock.WithdrawAmount * Constants.PenaltyRate), _account.Balance);
        Assert.Equal(2, _account.Transactions.Count);
        Assert.Equal(TransactionType.Withdraw, _account.Transactions[0].Type);
        Assert.Equal(AccountMock.WithdrawAmount, _account.Transactions[0].Amount);
        Assert.Equal(TransactionType.Fee, _account.Transactions[1].Type);
        Assert.Equal(AccountMock.WithdrawAmount * Constants.PenaltyRate, _account.Transactions[1].Amount);
    }

    [Fact] public void Withdraw_Should_ExcludePenalty_AfterMaturity()
    {
        _account.UtcNow = () => Constants.MaturityDate.AddDays(1);

        bool result = _account.Withdraw(AccountMock.WithdrawAmount, out var error);

        Assert.True(result);
        Assert.Null(error);
        Assert.Equal(_initialBalance - AccountMock.WithdrawAmount, _account.Balance);
        Assert.Single(_account.Transactions);
        Assert.Equal(TransactionType.Withdraw, _account.Transactions[0].Type);
        Assert.Equal(AccountMock.WithdrawAmount, _account.Transactions[0].Amount);
    }

    [Fact] public void Withdraw_Should_Fail_WhenAmountIsNegative()
    {
        bool result = _account.Withdraw(AccountMock.NegativeWithdrawAmount, out var error);

        Assert.False(result);
        Assert.Equal("Withdraw amount must be positive", error);
        Assert.Equal(_initialBalance, _account.Balance);
        Assert.Empty(_account.Transactions);
    }

    [Fact] public void Withdraw_Should_Fail_WhenInsufficientFunds_OnEarlyWithdrawal()
    {
        bool result = _account.Withdraw(AccountMock.WithdrawAmountOverLimitNonChecking, out var error);

        Assert.False(result);
        Assert.Equal("Insufficient funds (including penalty)", error);
        Assert.Equal(_initialBalance, _account.Balance);
        Assert.Empty(_account.Transactions);
    }

    [Fact] public void Withdraw_Should_Fail_WhenInsufficientFunds_AfterMaturity()
    {
        _account.UtcNow = () => Constants.MaturityDate.AddDays(1);

        bool result = _account.Withdraw(AccountMock.WithdrawAmountOverLimitNonChecking, out var error);

        Assert.False(result);
        Assert.Equal("Insufficient funds", error);
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
