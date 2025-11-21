using MiniBank.App.Exceptions;
using MiniBank.App.Helpers;
using MiniBank.App.Models;

namespace MiniBank.App.Services;
public static class AuthService
{
    public static bool IsAuthenticated { get; private set; } = false;
    public static string CurrentUser { get; private set; } = "";

    public static async Task RegisterAsync()
    {
        int type = await InputHelper.GetAccountTypeAsync();
        string username = await InputHelper.GetUsernameAsync(type), password = await InputHelper.GetPasswordAsync(), currency = await InputHelper.GetCurrencyAsync(), locale = await InputHelper.GetLocaleAsync();

        BankAccount account = type switch
        {
            1 => new CheckingAccount { Owner = username, Password = password, Currency = currency, Locale = locale },
            2 => new SavingsAccount { Owner = username, Password = password, Currency = currency, Locale = locale },
            3 => new LoanAccount { Owner = username, Password = password, Currency = currency, Locale = locale },
            4 => new FixedDepositAccount() { Owner = username, Password = password, Currency = currency, Locale = locale },
            _ => throw new BadRequestException("Invalid account type")
        };
        DataManager.Accounts.Add(account);
        await Console.Out.WriteLineAsync($"Registered {account.GetType().Name} for {username} with balance {CurrencyFormatter.Format(0, account.Currency, account.Locale)}");
        await DataManager.SaveAsync();
    }

    public static async Task LoginAsync()
    {
        string username = await InputHelper.GetUsernameAsync(isRegister: false);
        if (!DataManager.Accounts.AsParallel().Any(account => account.Owner == username)) throw new NotFoundException("User not found");
        
        string password = await InputHelper.GetPasswordAsync(isRegister: false);
        var account = DataManager.Accounts.AsParallel().FirstOrDefault(account => account.Owner == username && account.Password == password) ?? throw new NotFoundException("Invalid credentials");
        
        (IsAuthenticated, CurrentUser) = (true, username);
        await Console.Out.WriteLineAsync($"Logged in as {username} ({account.GetType().Name})");
    }

    public static async Task LogoutAsync()
    {
        (IsAuthenticated, CurrentUser) = (false, "");
        await Console.Out.WriteLineAsync("Logged out");
    }
}
