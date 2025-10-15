using MiniBankConsole.Models.Interfaces;

namespace MiniBankConsole.Models;
public class SavingsAccount : BankAccount, IInterestBearing
{
    public override void Deposit(decimal amount) => throw new NotImplementedException();

    public override bool Withdraw(decimal amount, out string? error) => throw new NotImplementedException();

    public override void PrintStatement() => throw new NotImplementedException();

    public void ApplyMonthlyInterest() => throw new NotImplementedException();
}
