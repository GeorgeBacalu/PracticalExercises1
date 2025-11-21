using MiniBank.App.Exceptions;
using MiniBank.App.Models.Interfaces;

namespace MiniBank.App.Models;
public class FixedDepositAccount: BankAccount, IInterestBearing
{
    public override void Deposit(decimal amount) => throw new BadRequestException("Can't deposit into a fixed deposit account after creation");

    public override bool Withdraw(decimal amount, out string? error)
    {
        if (amount <= 0) { error = "Withdraw amount must be positive"; return false; }

        var isEarly = DateTime.UtcNow < Constants.Constants.MaturityDate;
        var penalty = isEarly ? amount * Constants.Constants.PenaltyRate : 0;
        var total = amount + penalty;

        if (total > Balance) { error = isEarly ? "Insufficient funds (including penalty)" : "Insufficient funds"; return false; }

        Balance -= total;
        Transactions.Add(new() { Id = Guid.NewGuid(), Type = TransactionType.Withdraw, Amount = amount, AccountId = Id });
        if (penalty > 0) Transactions.Add(new() { Id = Guid.NewGuid(), Type = TransactionType.Fee, Amount = penalty, AccountId = Id });
        error = null;
        return true;
    }

    public void ApplyMonthlyInterest()
    {
        Transactions.Add(new() { Id = Guid.NewGuid(), Type = TransactionType.Interest, Amount = Balance * Constants.Constants.InterestRate, AccountId = Id });
        Balance += Balance * Constants.Constants.InterestRate;
    }

    public override string ToString() => $"{base.ToString()}, Maturity Date: {Constants.Constants.MaturityDate}";
}
