using MiniBank.App.Constants;
using MiniBank.App.Exceptions;
using MiniBank.App.Models;
using MiniBank.Test.Mocks;

namespace MiniBank.Test.Unit.Models;
public class LoanAccountTest
{
    private readonly LoanAccount _account;
    private readonly decimal _initialBalance;

    public LoanAccountTest() => _initialBalance = (_account = AccountMock.LoanAccountMock()).Balance;

    [Fact] public void Deposit_Should_DecreaseDebt_WhenAmountIsPositive()
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

    [Fact] public void Deposit_Should_Fail_WhenOverPayoffLimit()
    {
        var exception = Assert.Throws<BadRequestException>(() => _account.Deposit(AccountMock.DepositAmountOverPayoffLimit));
        Assert.Equal("Transfer exceeds loan payoff amount", exception.Message);
        Assert.Equal(_initialBalance, _account.Balance);
        Assert.Empty(_account.Transactions);
    }

    [Fact] public void Withdraw_Should_IncreaseDebt_WhenAmountIsPositive()
    {
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

    [Fact] public void ApplyMonthlyInterest_Should_DecreaseDebt()
    {
        _account.ApplyMonthlyInterest();

        Assert.Equal(_initialBalance + _initialBalance * Constants.InterestRate, _account.Balance);
        Assert.Single(_account.Transactions);
        Assert.Equal(TransactionType.Interest, _account.Transactions[0].Type);
        Assert.Equal(Math.Abs(_initialBalance * Constants.InterestRate), _account.Transactions[0].Amount);
    }
}
