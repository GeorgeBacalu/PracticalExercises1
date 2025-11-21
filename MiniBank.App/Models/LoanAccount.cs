using MiniBank.App.Exceptions;
using MiniBank.App.Models.Interfaces;

namespace MiniBank.App.Models;
public class LoanAccount : BankAccount, IInterestBearing
{
    public override void Deposit(decimal amount)
    {
        if (amount <= 0) throw new BadRequestException("Deposit amount must be positive");
        if (Balance + amount > 0) throw new BadRequestException("Transfer exceeds loan payoff amount");

        Balance += amount;
        Transactions.Add(new() { Type = TransactionType.Deposit, Amount = amount, AccountId = Id });
    }

    public override bool Withdraw(decimal amount, out string? error)
    {
        if (amount <= 0) { error = "Withdraw amount must be positive"; return false; }

        Balance -= amount;
        error = null;
        Transactions.Add(new() { Type = TransactionType.Withdraw, Amount = amount, AccountId = Id });
        return true;
    }

    public void ApplyMonthlyInterest()
    {
        Transactions.Add(new() { Type = TransactionType.Interest, Amount = Math.Abs(Balance * Constants.Constants.InterestRate), AccountId = Id });
        Balance += Balance * Constants.Constants.InterestRate;
    }
}
