namespace MiniBankConsole.Models.Interfaces;
public interface IOverdraftPolicy
{
    decimal OverdraftLimit { get; }
}
