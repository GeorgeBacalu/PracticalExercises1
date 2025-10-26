using MiniBankConsole.Exceptions;
using MiniBankConsole.Models;

namespace MiniBankConsole.Helpers;
public static class InputHelper
{
    public static int GetAccountType(string? role = "account")
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

    public static string GetUsername(int type = 0, bool isRegister = true)
    {
        string username;
        while (true)
        {
            Console.Write("Enter username: ");
            username = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(username)) Console.WriteLine("Username is required");
            else if (isRegister && DataManager.Accounts.Any(account => account.Owner == username && account.GetType().Name == GetTypeName(type))) Console.WriteLine("User already has an account of this type");
            else break;
        }
        return username;
    }

    public static string GetPassword(bool isRegister = true)
    {
        string password;
        while (true)
        {
            Console.Write("Enter password: ");
            password = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(password)) Console.WriteLine("Password is required");
            else if (isRegister && password.Length < 6) Console.WriteLine("Password must be at least 6 characters long");
            else break;
        }
        return password;
    }

    public static string GetCurrency()
    {
        string currency;
        while (true)
        {
            Console.Write("Enter preferred currency: ");
            currency = Console.ReadLine()?.Trim().ToUpper() ?? "";
            if (string.IsNullOrWhiteSpace(currency)) Console.WriteLine("Currency is required");
            else if (!DataManager.Currencies.Any(c => c.Code == currency)) Console.WriteLine("Invalid currency");
            else break;
        }
        return currency;
    }

    public static string GetLocale()
    {
        string locale;
        while (true)
        {
            Console.Write("Enter preferred locale: ");
            locale = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(locale)) Console.WriteLine("Locale is required");
            else if (!DataManager.Locales.Any(l => l.Code == locale)) Console.WriteLine("Invalid locale");
            else break;
        }
        return locale;
    }

    public static decimal GetOpeningDeposit()
    {
        decimal balance;
        while (true)
        {
            Console.Write("Opening deposit: ");
            if (!decimal.TryParse(Console.ReadLine(), out balance)) Console.WriteLine("Opening deposit must be numeric");
            else if (balance < 0) Console.WriteLine("Opening deposit must be positive");
            else break;
        }
        return balance;
    }

    public static decimal GetAmount(string action)
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

    public static string GetReceiver()
    {
        while (true)
        {
            Console.Write($"Enter receiver's name: ");
            var receiver = Console.ReadLine()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(receiver)) Console.WriteLine($"Receiver name is required");
            else return receiver;
        }
    }

    public static string GetTypeName(int type) => type switch { 1 => nameof(CheckingAccount), 2 => nameof(SavingsAccount), 3 => nameof(LoanAccount), 4 => nameof(FixedDepositAccount), _ => throw new BadRequestException("Invalid account type") };
}
