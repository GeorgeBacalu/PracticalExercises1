using MiniBankConsole.Exceptions;
using MiniBankConsole.Models;

namespace MiniBankConsole.Services;
public class AccountRegistry
{
    public static List<BankAccount> Accounts { get; } = [];

    public static void GetAccounts()
    {
        if (!Accounts.Any())
        {
            Console.WriteLine("No accounts found");
            return;
        }

        Console.WriteLine("Bank accounts: ");
        Accounts.ForEach(Console.WriteLine);
    }

    public static void CreateAccount()
    {
        string type, owner;
        decimal balance;

        while (true)
        {
            Console.Write("Type (Checking/Savings/Loan): ");
            type = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (type != "checking" && type != "savings" && type != "loan")
                Console.WriteLine("Invalid or missing account type");
            else break;
        }
        while (true)
        {
            Console.Write("Owner: ");
            owner = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(owner))
                Console.WriteLine("Owner name is required");
            else if (Accounts.Exists(account => account.Owner.ToLower() == owner.ToLower()))
                Console.WriteLine("Owner name must be unique");
            else break;
        }
        while (true)
        {
            Console.Write("Opening deposit: ");
            var balanceString = Console.ReadLine();
            if (!decimal.TryParse(balanceString, out balance))
            {
                Console.WriteLine("Opening deposit must be numeric");
                continue;
            }
            if (balance < 0)
                Console.WriteLine("Opening deposit must be positive");
            else break;
        }

        BankAccount account = type switch
        {
            "checking" => new CheckingAccount { Owner = owner, Balance = balance },
            "savings" => new SavingsAccount { Owner = owner, Balance = balance },
            "loan" => new LoanAccount { Owner = owner, Balance = -balance },
            _ => throw new BadRequestException("Invalid account type")
        };
        Accounts.Add(account);
        Console.WriteLine($"Created #{Accounts.Count} {account.GetType().Name} for {owner} with balance {balance:C}");
    }

    public static void Deposit(string owner, decimal amount)
    {
        var account = Accounts.FirstOrDefault(account => account.Owner == owner);
        if (account == null) throw new MissingResourceException("Account not found");
        account.Deposit(amount);
    }

    public static void Withdraw(string owner, decimal amount)
    {
        var account = Accounts.FirstOrDefault(account => account.Owner == owner);
        if (account == null) throw new MissingResourceException("Account not found");
        var withdrawSuccess = account.Withdraw(amount, out var error);
        if (!withdrawSuccess) throw new BadRequestException(error ?? "Withdraw failed");
    }

    public static void ViewStatement(string owner)
    {
        var account = Accounts.FirstOrDefault(account => account.Owner == owner);
        if (account == null) throw new MissingResourceException("Account not found");
        account.PrintStatement();
    }

    public static void RunMonthEndProcessing()
    {
        foreach (var account in Accounts)
            if (account is SavingsAccount savingsAccount)
                savingsAccount.ApplyMonthlyInterest();
            else if (account is LoanAccount loanAccount)
                loanAccount.ApplyMonthlyInterest();
        Console.WriteLine("Monthly interest applied for all accounts");
    }
}
