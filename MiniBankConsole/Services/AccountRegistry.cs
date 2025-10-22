using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Models;
using MiniBankConsole.Models.Interfaces;

namespace MiniBankConsole.Services;
public static class AccountRegistry
{
    public static void GetAccounts()
    {
        var accounts = AuthService.IsAuthenticated ? [.. DataManager.Accounts.Where(account => account.Owner == AuthService.CurrentUsername)] : DataManager.Accounts;
        if (!accounts.Any()) { Console.WriteLine("No accounts found"); return; }

        Console.WriteLine("Bank accounts: ");
        accounts.ForEach(Console.WriteLine);
    }

    public static void CreateAccount()
    {
        int type = GetAccountType();
        string username = AuthService.IsAuthenticated ? AuthService.CurrentUsername : GetOwner();
        if (DataManager.Accounts.Any(account => account.Owner == username && account.GetType().Name == GetTypeName(type))) { Console.WriteLine("User already has an account of this type"); return; }

        decimal balance;
        while (true)
        {
            Console.Write("Opening deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out balance)) Console.WriteLine("Opening deposit must be numeric");
            else if (balance < 0) Console.WriteLine("Opening deposit must be positive");
            else break;
        }

        var password = (AuthService.IsAuthenticated ? DataManager.Accounts.FirstOrDefault(account => account.Owner == username) : null)?.Password ?? username;
        BankAccount account = type switch
        {
            1 => new CheckingAccount { Owner = username, Password = password, Balance = balance },
            2 => new SavingsAccount { Owner = username, Password = password, Balance = balance },
            3 => new LoanAccount { Owner = username, Password = password, Balance = -balance },
            4 => new FixedDepositAccount() { Owner = username, Password = password, Balance = balance },
            _ => throw new BadRequestException("Invalid account type")
        };
        DataManager.Accounts.Add(account);
        account.Transactions.Add(new() { Type = type == 3 ? TransactionType.Withdraw : TransactionType.Deposit, Amount = balance, AccountId = account.Id });
        Console.WriteLine($"\nCreated #{DataManager.Accounts.Count} {account.GetType().Name} for {username} with balance {balance:C}");
    }

    public static void Deposit()
    {
        string owner = GetOwner();
        int type = GetAccountType();
        decimal amount = GetAmount("deposit");

        GetAccount(owner, type).Deposit(amount);
    }

    public static void Withdraw()
    {
        string owner = GetOwner();
        int type = GetAccountType();
        decimal amount = GetAmount("withdraw");

        if (!GetAccount(owner, type).Withdraw(amount, out var error)) throw new BadRequestException(error ?? "Withdraw failed");
    }

    public static void ViewStatement()
    {
        string owner = GetOwner();
        int type = GetAccountType();

        GetAccount(owner, type).PrintStatement();
    }

    public static void RunMonthEndProcessing()
    {
        foreach (var account in DataManager.Accounts.OfType<IInterestBearing>()) account.ApplyMonthlyInterest();
        Console.WriteLine("Month-end applied (interest/fees)");
    }

    public static void Transfer()
    {
        string sender = AuthService.CurrentUsername, receiver = GetRole("receiver");
        int senderType = GetAccountType("sender's account"), receiverType = GetAccountType("receiver's account");
        if (sender == receiver && senderType == receiverType) throw new BadRequestException("Sender and receiver must be different");
        
        decimal amount = GetAmount("transfer");
        var senderAccount = GetAccount(sender, senderType);
        var receiverAccount = GetAccount(receiver, receiverType);

        if (!senderAccount.Withdraw(amount, out var error)) throw new BadRequestException(error ?? "Transfer failed");
        receiverAccount.Deposit(amount);

        Console.WriteLine($"\nTransferred {amount:C} from {sender} ({GetTypeName(senderType)}) to {receiver} ({GetTypeName(receiverType)})");
    }

    private static string GetOwner()
    {
        if (AuthService.IsAuthenticated) return AuthService.CurrentUsername;

        string owner;
        while (true)
        {
            Console.Write("Enter owner's name: ");
            owner = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(owner)) Console.WriteLine("Owner name is required");
            else break;
        }
        return owner;
    }

    private static int GetAccountType(string? role = "account")
    {
        int type;
        while (true)
        {
            Console.Write($"Enter {role} type (1 - checking, 2 - savings, 3 - loan, 4 - fixed deposit): ");
            if (!int.TryParse(Console.ReadLine(), out type)) Console.WriteLine("Account type must be numeric");
            else if (type is < 1 or > 4) Console.WriteLine("Enter a number between 1 and 4");
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
            if (!decimal.TryParse(Console.ReadLine(), out amount)) Console.WriteLine("Amount must be numeric");
            else if (amount <= 0) Console.WriteLine("Amount must be positive");
            else break;
        }
        return amount;
    }

    private static string GetRole(string role)
    {
        while (true)
        {
            Console.Write($"Enter {role}'s name: ");
            var name = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(name)) Console.WriteLine($"{role} name is required");
            else return name;
        }
    }

    private static BankAccount GetAccount(string owner, int type) => DataManager.Accounts.FirstOrDefault(account => account.Owner == owner && account.GetType().Name == GetTypeName(type)) ?? throw new MissingResourceException("Account not found");

    private static string GetTypeName(int type) => type switch { 1 => nameof(CheckingAccount), 2 => nameof(SavingsAccount), 3 => nameof(LoanAccount), 4 => nameof(FixedDepositAccount), _ => throw new BadRequestException("Invalid account type") };
}
