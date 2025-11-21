namespace MiniBank.App.Constants;
public static class Constants
{
    public const decimal OverdraftLimit = -200m;
    public const decimal InterestRate = 0.01m;
    public const decimal PenaltyRate = 0.02m;
    public const int FixedAccountTermMonths = 12;

    public static readonly DateTime MaturityDate = DateTime.UtcNow.AddMonths(FixedAccountTermMonths);
}
