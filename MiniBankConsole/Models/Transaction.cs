namespace MiniBankConsole.Models;
public class Transaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public Guid AccountId { get; set; }
}

public enum TransactionType { Invalid, Deposit, Withdrawal, Transfer, Interest }
