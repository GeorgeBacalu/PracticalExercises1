using MiniBankConsole.Models.Interfaces;
using System.Text.Json.Serialization;

namespace MiniBankConsole.Models;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CheckingAccount), "checking")]
[JsonDerivedType(typeof(SavingsAccount), "savings")]
[JsonDerivedType(typeof(LoanAccount), "loan")]
[JsonDerivedType(typeof(FixedDepositAccount), "fixed deposit")]
public abstract class BankAccount : ITransactable, IStatement
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string Owner { get; init; }
    public required string Password { get; init; }
    public decimal Balance { get; set; }

    public List<Transaction> Transactions { get; } = [];

    public abstract void Deposit(decimal amount);
    public abstract bool Withdraw(decimal amount, out string? error);

    public virtual void PrintStatement()
    {
        Console.WriteLine($"\n{GetType().Name} statement for {Owner}");
        foreach (var transaction in Transactions.OrderByDescending(transaction => transaction.Date))
            Console.WriteLine($"{transaction.Date}: {transaction.Type} - {transaction.Amount:C}");
    }

    public override string ToString() => $"{GetType().Name} - Owner: {Owner}, Balance: {Balance:C}";
}
