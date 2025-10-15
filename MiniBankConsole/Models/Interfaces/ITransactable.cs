namespace MiniBankConsole.Models.Interfaces;
public interface ITransactable
{
    void Deposit(decimal amount);
    bool Withdraw(decimal amount, out string? error);
}
