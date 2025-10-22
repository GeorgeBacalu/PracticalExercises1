using MiniBankConsole.Exceptions;
using MiniBankConsole.Models.Interfaces;
using static MiniBankConsole.Constants.Constants;

namespace MiniBankConsole.Models;
public class FixedDepositAccount: BankAccount, IInterestBearing
{
    private readonly DateTime maturityDate = DateTime.UtcNow.AddMonths(termMonths);

    public override void Deposit(decimal amount) => throw new BadRequestException("Can't deposit into a fixed deposit account after creation");

    public override bool Withdraw(decimal amount, out string? error)
    {
        if (amount <= 0) { error = "Withdraw amount must be positive"; return false; }
        if (DateTime.UtcNow < maturityDate)
        {
            var totalDeduction = amount + amount * penaltyRate;
            if (totalDeduction > Balance) { error = "Insufficient funds (including penalty)"; return false; }

            Balance -= totalDeduction;
            error = null;
            Transactions.Add(new() { Type = TransactionType.Withdraw, Amount = amount, AccountId = Id });
            Transactions.Add(new() { Type = TransactionType.Withdraw, Amount = amount * penaltyRate, AccountId = Id });
            return true;
        }

        if (amount > Balance) { error = "Insufficient funds"; return false; }
        Balance -= amount;
        Transactions.Add(new() { Type = TransactionType.Withdraw, Amount = amount, AccountId = Id });
        error = null;
        return true;
    }

    public void ApplyMonthlyInterest()
    {
        Transactions.Add(new() { Type = TransactionType.Interest, Amount = Balance * interestRate, AccountId = Id });
        Balance += Balance * interestRate;
    }

    public override string ToString() => $"{base.ToString()}, Maturity Date: {maturityDate:yyyy-MM-dd}";
}
