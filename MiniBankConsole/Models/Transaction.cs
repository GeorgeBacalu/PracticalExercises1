namespace MiniBankConsole.Models;
public class Transaction
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime Date { get; } = DateTime.UtcNow;
    public TransactionType Type { get; init; }
    public decimal Amount { get; init; }
    public Guid AccountId { get; init; }
}

public enum TransactionType { Invalid, Deposit, Withdraw, Interest }
