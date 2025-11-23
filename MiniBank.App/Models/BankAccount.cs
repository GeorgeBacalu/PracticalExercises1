using MiniBank.App.Helpers;
using MiniBank.App.Models.Interfaces;
using System.Text.Json.Serialization;

namespace MiniBank.App.Models;
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CheckingAccount), "checking")]
[JsonDerivedType(typeof(SavingsAccount), "savings")]
[JsonDerivedType(typeof(LoanAccount), "loan")]
[JsonDerivedType(typeof(FixedDepositAccount), "fixeddeposit")]
public abstract class BankAccount : ITransactable, IStatement
{
    public Guid Id { get; init; }
    public required string Owner { get; init; }
    public required string Password { get; init; }
    public decimal Balance { get; set; }
    public string Currency { get; init; } = "EUR";
    public string Locale { get; init; } = "en-US";

    public List<Transaction> Transactions { get; } = [];

    public abstract void Deposit(decimal amount);
    public abstract bool Withdraw(decimal amount, out string? error);

    public virtual void PrintStatement()
    {
        Console.WriteLine($"{GetType().Name} statement for {Owner}");
        foreach (var transaction in Transactions.OrderByDescending(transaction => transaction.Date))
            Console.WriteLine($"{transaction.Date}: {transaction.Type} - {CurrencyFormatter.Format(transaction.Amount, Currency, Locale)}");
    }

    public override string ToString() => $"{GetType().Name} - Owner: {Owner}, Balance: {CurrencyFormatter.Format(Balance, Currency, Locale)}";
}
