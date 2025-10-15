using MiniBankConsole.Models.Interfaces;
using static MiniBankConsole.Constants.Constants;

namespace MiniBankConsole.Models;
public class LoanAccount : BankAccount, IInterestBearing
{
    public override bool Withdraw(decimal amount, out string? error)
    {
        if (amount <= 0)
        {
            error = "Withdraw amount must be positive";
            return false;
        }

        Balance -= amount;
        error = null;
        Transactions.Add(new() { Type = TransactionType.Withdrawal, Amount = amount, AccountId = Id });
        return true;
    }

    public void ApplyMonthlyInterest()
    {
        Balance += Balance * interestRate;
        Transactions.Add(new() { Type = TransactionType.Interest, Amount = Balance * interestRate, AccountId = Id });
    }
}
