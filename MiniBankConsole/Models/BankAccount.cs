using MiniBankConsole.Exceptions;
using MiniBankConsole.Models.Interfaces;

namespace MiniBankConsole.Models;
public abstract class BankAccount : ITransactable, IStatement
{
    public Guid Id { get; } = Guid.NewGuid();
    public required string Owner { get; init; }
    public decimal Balance { get; set; }

    public List<Transaction> Transactions { get; } = [];

    public virtual void Deposit(decimal amount)
    {
        if (amount <= 0)
            throw new BadRequestException("Deposit amount must be positive");

        Balance += amount;
        Transactions.Add(new() { Type = TransactionType.Deposit, Amount = amount, AccountId = Id });
    }

    public abstract bool Withdraw(decimal amount, out string? error);
    public virtual void PrintStatement()
    {
        Console.WriteLine($"{GetType().Name} statement for {Owner}");
        foreach (var transaction in Transactions.OrderByDescending(transaction => transaction.Date))
            Console.WriteLine($"{transaction.Date}: {transaction.Type} - {transaction.Amount:C}");
    }

    public override string ToString() => $"{GetType().Name} - Owner: {Owner}, Balance: {Balance:C}";
}
