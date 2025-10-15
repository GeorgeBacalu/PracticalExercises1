using MiniBankConsole.Models.Interfaces;
using static MiniBankConsole.Constants.Constants;

namespace MiniBankConsole.Models;
public class CheckingAccount : BankAccount, IOverdraftPolicy
{
    public decimal OverdraftLimit => overdraftLimit;

    public override bool Withdraw(decimal amount, out string? error)
    {
        if (amount <= 0)
        {
            error = "Withdraw amount must be positive";
            return false;
        }
        if (Balance - amount < OverdraftLimit)
        {
            error = "Insufficient funds";
            return false;
        }

        Balance -= amount;
        error = null;
        Transactions.Add(new() { Type = TransactionType.Withdrawal, Amount = amount, AccountId = Id });
        return true;
    }
}
