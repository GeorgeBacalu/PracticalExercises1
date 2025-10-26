using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Models;

namespace MiniBankConsole.Services;
public static class AuthService
{
    public static bool IsAuthenticated { get; private set; } = false;
    public static string CurrentUser { get; private set; } = "";

    public static void Register()
    {
        int type = InputHelper.GetAccountType();
        string username = InputHelper.GetUsername(type), password = InputHelper.GetPassword(), currency = InputHelper.GetCurrency(), locale = InputHelper.GetLocale();

        BankAccount account = type switch
        {
            1 => new CheckingAccount { Owner = username, Password = password, Currency = currency, Locale = locale },
            2 => new SavingsAccount { Owner = username, Password = password, Currency = currency, Locale = locale },
            3 => new LoanAccount { Owner = username, Password = password, Currency = currency, Locale = locale },
            4 => new FixedDepositAccount() { Owner = username, Password = password, Currency = currency, Locale = locale },
            _ => throw new BadRequestException("Invalid account type")
        };
        DataManager.Accounts.Add(account);
        Console.WriteLine($"\nRegistered {account.GetType().Name} for {username} with balance {CurrencyFormatter.Format(0, account.Currency, account.Locale)}");
    }

    public static void Login()
    {
        string username = InputHelper.GetUsername(isRegister: false);
        if (!DataManager.Accounts.Any(account => account.Owner == username)) throw new MissingResourceException("User not found");
        
        string password = InputHelper.GetPassword(isRegister: false);
        var account = DataManager.Accounts.FirstOrDefault(account => account.Owner == username && account.Password == password) ?? throw new MissingResourceException("Invalid credentials");
        
        (IsAuthenticated, CurrentUser) = (true, username);
        Console.WriteLine($"\nLogged in as {username} ({account.GetType().Name})");
    }

    public static void Logout()
    {
        (IsAuthenticated, CurrentUser) = (false, "");
        Console.WriteLine("Logged out");
    }
}
