using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Models;

namespace MiniBankConsole.Services;
public static class AuthService
{
    public static bool IsAuthenticated { get; private set; } = false;
    public static string CurrentUsername { get; private set; } = "";

    public static void Register()
    {
        int type = GetAccountType();
        string username, password;

        while (true)
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(username)) Console.WriteLine("Username is required");
            else if (DataManager.Accounts.Any(account => account.Owner == username && account.GetType().Name == GetTypeName(type))) Console.WriteLine("User already has an account of this type");
            else break;
        }
        while (true)
        {
            Console.Write("Enter password: ");
            password = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(password)) Console.WriteLine("Password is required");
            else if (password.Length < 6) Console.WriteLine("Password must be at least 6 characters long");
            else break;
        }

        BankAccount account = type switch
        {
            1 => new CheckingAccount { Owner = username, Password = password },
            2 => new SavingsAccount { Owner = username, Password = password },
            3 => new LoanAccount { Owner = username, Password = password },
            4 => new FixedDepositAccount() { Owner = username, Password = password },
            _ => throw new BadRequestException("Invalid account type")
        };
        DataManager.Accounts.Add(account);
        Console.WriteLine($"\nRegistered {account.GetType().Name} for {username} with balance {0:C}");
        DataManager.Save();
    }

    public static void Login()
    {
        string username, password;

        while (true)
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(username)) Console.WriteLine("Username is required");
            else break;
        }
        while (true)
        {
            Console.Write("Enter password: ");
            password = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(password)) Console.WriteLine("Password is required");
            else break;
        }

        var account = GetAccount(username, password);
        (IsAuthenticated, CurrentUsername) = (true, username);
        Console.WriteLine($"\nLogged in as {username} ({account.GetType().Name})");
    }

    public static void Logout()
    {
        (IsAuthenticated, CurrentUsername) = (false, "");
        Console.WriteLine("Logged out");
    }

    private static int GetAccountType(string? role = "account")
    {
        int type;
        while (true)
        {
            Console.Write($"Enter {role} type (1 - checking, 2 - savings, 3 - loan, 4 - fixed deposit): ");
            if (!int.TryParse(Console.ReadLine(), out type)) Console.WriteLine("Account type must be numeric");
            else if (type < 1 || type > 4) Console.WriteLine("Enter a number between 1 and 4");
            else break;
        }
        return type;
    }

    private static BankAccount GetAccount(string username, string password) => DataManager.Accounts.FirstOrDefault(account => account.Owner == username && account.Password == password) ?? throw new MissingResourceException("Account not found");

    private static string GetTypeName(int type) => type switch { 1 => nameof(CheckingAccount), 2 => nameof(SavingsAccount), 3 => nameof(LoanAccount), 4 => nameof(FixedDepositAccount), _ => throw new BadRequestException("Invalid account type") };
}
