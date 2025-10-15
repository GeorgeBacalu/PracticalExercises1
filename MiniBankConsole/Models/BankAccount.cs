using MiniBankConsole.Models.Interfaces;

namespace MiniBankConsole.Models;
public abstract class BankAccount : ITransactable, IStatement
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string Owner { get; init; }
    public decimal Balance { get; protected set; }

    public List<Transaction> Transactions { get; set; } = [];

    public abstract void Deposit(decimal amount);
    public abstract bool Withdraw(decimal amount, out string? error);
    public abstract void PrintStatement();
}
