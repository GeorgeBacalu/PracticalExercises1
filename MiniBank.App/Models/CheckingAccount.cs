using MiniBank.App.Exceptions;
using MiniBank.App.Models.Interfaces;

namespace MiniBank.App.Models;
public class CheckingAccount : BankAccount, IOverdraftPolicy
{
    public decimal OverdraftLimit => Constants.Constants.OverdraftLimit;

    public override void Deposit(decimal amount)
    {
        if (amount <= 0) throw new BadRequestException("Deposit amount must be positive");

        Balance += amount;
        Transactions.Add(new() { Type = TransactionType.Deposit, Amount = amount, AccountId = Id });
    }

    public override bool Withdraw(decimal amount, out string? error)
    {
        if (amount <= 0) { error = "Withdraw amount must be positive"; return false; }
        if (Balance - amount < OverdraftLimit) { error = "Insufficient funds"; return false; }

        Balance -= amount;
        error = null;
        Transactions.Add(new() { Type = TransactionType.Withdraw, Amount = amount, AccountId = Id });
        return true;
    }
}
