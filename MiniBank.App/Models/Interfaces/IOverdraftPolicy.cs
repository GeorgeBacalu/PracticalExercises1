namespace MiniBank.App.Models.Interfaces;
public interface IOverdraftPolicy
{
    decimal OverdraftLimit { get; }
}
