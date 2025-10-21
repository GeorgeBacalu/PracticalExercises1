using MiniBankConsole.Exceptions;
using MiniBankConsole.Models;
using MiniBankConsole.Models.Interfaces;

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
        string owner, type = GetAccountType();
        decimal balance;

        while (true)
        {
            Console.Write("Owner: ");
            owner = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(owner))
                Console.WriteLine("Owner name is required");
            else if (Accounts.Exists(account => account.Owner.ToLower() == owner.ToLower() && account.GetType().Name.Replace("Account", "").ToLower() == type))
                Console.WriteLine("Owner name must be unique");
            else break;
        }
        while (true)
        {
            Console.Write("Opening deposit: ");
            var balanceString = Console.ReadLine();
            if (!decimal.TryParse(balanceString, out balance))
                Console.WriteLine("Opening deposit must be numeric");
            else if (balance < 0)
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
        account.Transactions.Add(new() { Type = TransactionType.Deposit, Amount = balance, AccountId = account.Id });
        
        Console.WriteLine($"\nCreated #{Accounts.Count} {account.GetType().Name} for {owner} with balance {balance:C}");
    }

    public static void Deposit()
    {
        string owner = GetOwner(), type = GetAccountType();
        decimal amount = GetAmount("deposit");
        
        var account = GetBankAccount(owner, type);
        account.Deposit(amount);
    }

    public static void Withdraw()
    {
        string owner = GetOwner(), type = GetAccountType();
        decimal amount = GetAmount("withdraw");
        
        var account = GetBankAccount(owner, type);
        var withdrawSuccess = account.Withdraw(amount, out var error);
        if (!withdrawSuccess) throw new BadRequestException(error ?? "Withdraw failed");
    }

    public static void ViewStatement()
    {
        string owner = GetOwner(), type = GetAccountType();
        
        var account = GetBankAccount(owner, type);
        account.PrintStatement();
    }

    public static void RunMonthEndProcessing()
    {
        foreach (var account in Accounts)
            if (account is IInterestBearing interestBearing)
                interestBearing.ApplyMonthlyInterest();
        Console.WriteLine("Month-end applied (interest/fees)");
    }

    private static string GetOwner()
    {
        string owner;

        while (true)
        {
            Console.Write("Enter owner's name: ");
            owner = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(owner))
                Console.WriteLine("Owner name is required");
            else break;
        }
        return owner;
    }

    private static string GetAccountType()
    {
        string type;

        while (true)
        {
            Console.Write("Enter account type (checking/savings/loan): ");
            type = Console.ReadLine()?.Trim().ToLower() ?? "";
            if (type != "checking" && type != "savings" && type != "loan")
                Console.WriteLine("Invalid or missing account type");
            else break;
        }
        return type;
    }

    private static decimal GetAmount(string action)
    {
        decimal amount;

        while (true)
        {
            Console.Write($"Enter amount to {action}: ");
            var amountString = Console.ReadLine();
            if (!decimal.TryParse(amountString, out amount))
                Console.WriteLine("Amount must be numeric");
            else if (amount <= 0)
                Console.WriteLine("Amount must be positive");
            else break;
        }
        return amount;
    }

    private static BankAccount GetBankAccount(string owner, string type)
    {
        var account = Accounts.FirstOrDefault(account => account.Owner == owner && account.GetType().Name.Replace("Account", "").ToLower() == type)
            ?? throw new MissingResourceException("Account not found");
        if (account.Id == Guid.Empty) throw new MissingResourceException("Account ID not found");
        return account;
    }
}
