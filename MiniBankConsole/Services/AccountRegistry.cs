using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Models;
using MiniBankConsole.Models.Interfaces;

namespace MiniBankConsole.Services;
public static class AccountRegistry
{
    public static void GetAccounts()
    {
        if (!DataManager.Accounts.Any()) { Console.WriteLine("No accounts found"); return; }

        Console.WriteLine("Bank accounts: ");
        DataManager.Accounts.ForEach(Console.WriteLine);
    }

    public static void CreateAccount()
    {
        int type = GetAccountType();
        string owner;
        decimal balance;

        while (true)
        {
            Console.Write("Owner: ");
            owner = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(owner)) Console.WriteLine("Owner name is required");
            else if (DataManager.Accounts.Any(account => account.Owner == owner && account.GetType().Name == GetTypeName(type))) Console.WriteLine("Owner already has an account of this type");
            else break;
        }
        while (true)
        {
            Console.Write("Opening deposit: ");
            var balanceString = Console.ReadLine();
            if (!decimal.TryParse(balanceString, out balance)) Console.WriteLine("Opening deposit must be numeric");
            else if (balance < 0) Console.WriteLine("Opening deposit must be positive");
            else break;
        }

        BankAccount account = type switch
        {
            1 => new CheckingAccount { Owner = owner, Balance = balance },
            2 => new SavingsAccount { Owner = owner, Balance = balance },
            3 => new LoanAccount { Owner = owner, Balance = -balance },
            4 => new FixedDepositAccount() { Owner = owner, Balance = balance },
            _ => throw new BadRequestException("Invalid account type")
        };
        DataManager.Accounts.Add(account);
        account.Transactions.Add(new() { Type = type == 3 ? TransactionType.Withdraw : TransactionType.Deposit, Amount = balance, AccountId = account.Id });
        Console.WriteLine($"\nCreated #{DataManager.Accounts.Count} {account.GetType().Name} for {owner} with balance {balance:C}");
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
        string sender = GetRole("sender"), receiver = GetRole("receiver");
        int senderType = GetAccountType("sender's account"), receiverType = GetAccountType("receiver's account");
        if (sender == receiver && senderType == receiverType) throw new BadRequestException("Sender and receiver must be different");
        decimal amount = GetAmount("transfer");

        if (!GetAccount(sender, senderType).Withdraw(amount, out var error)) throw new BadRequestException(error ?? "Transfer failed");
        GetAccount(receiver, receiverType).Deposit(amount);

        Console.WriteLine($"\nTransferred {amount:C} from {sender} ({GetTypeName(senderType)}) to {receiver} ({GetTypeName(receiverType)})");
    }

    private static string GetOwner()
    {
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
            var typeString = Console.ReadLine();
            if (!int.TryParse(typeString, out type)) Console.WriteLine("Account type must be numeric");
            else if (type < 1 || type > 4) Console.WriteLine("Enter a number between 1 and 4");
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
            if (!decimal.TryParse(amountString, out amount)) Console.WriteLine("Amount must be numeric");
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
