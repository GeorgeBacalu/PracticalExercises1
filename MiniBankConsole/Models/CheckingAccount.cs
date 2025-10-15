using MiniBankConsole.Models.Interfaces;
using static MiniBankConsole.Constants.Constants;

namespace MiniBankConsole.Models;
public class CheckingAccount : BankAccount, IOverdraftPolicy
{
    public decimal OverdraftLimit => overdraftLimit;

    public override void Deposit(decimal amount) => throw new NotImplementedException();

    public override bool Withdraw(decimal amount, out string? error) => throw new NotImplementedException();

    public override void PrintStatement() => throw new NotImplementedException();
}
