using MiniBank.App.Exceptions;
using MiniBank.App.Models;

namespace MiniBank.App.Helpers;
public static class InputHelper
{
    public static async Task<int> GetAccountTypeAsync(string? role = "account")
    {
        while (true)
        {
            await Console.Out.WriteAsync($"Enter {role} type (1 - checking, 2 - savings, 3 - loan, 4 - fixed deposit): ");
            if (!int.TryParse(await Console.In.ReadLineAsync(), out int type)) { await Console.Out.WriteLineAsync("Account type must be numeric"); continue; }
            else if (type is < 1 or > 4) { await Console.Out.WriteLineAsync("Enter a number between 1 and 4"); continue; }
            return type;
        }
    }

    public static async Task<string> GetUsernameAsync(int type = 0, bool isRegister = true)
    {
        while (true)
        {
            await Console.Out.WriteAsync("Enter username: ");
            string username = (await Console.In.ReadLineAsync())?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(username)) { await Console.Out.WriteLineAsync("Username is required"); continue; }
            else if (isRegister && DataManager.Accounts.AsParallel().Any(account => account.Owner == username && account.GetType().Name == GetTypeName(type))) { await Console.Out.WriteLineAsync("User already has an account of this type"); continue; }
            return username;
        }
    }

    public static async Task<string> GetPasswordAsync(bool isRegister = true)
    {
        while (true)
        {
            await Console.Out.WriteAsync("Enter password: ");
            string password = (await Console.In.ReadLineAsync())?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(password)) { await Console.Out.WriteLineAsync("Password is required"); continue; }
            else if (isRegister && password.Length < 6) { await Console.Out.WriteLineAsync("Password must be at least 6 characters long"); continue; }
            return password;
        }
    }

    public static async Task<string> GetCurrencyAsync()
    {
        while (true)
        {
            await Console.Out.WriteAsync("Enter preferred currency: ");
            string currency = (await Console.In.ReadLineAsync())?.Trim().ToUpper() ?? "";
            if (string.IsNullOrWhiteSpace(currency)) { await Console.Out.WriteLineAsync("Currency is required"); continue; }
            else if (!DataManager.Currencies.AsParallel().Any(c => c.Code == currency)) { await Console.Out.WriteLineAsync("Invalid currency"); continue; }
            return currency;
        }
    }

    public static async Task<string> GetLocaleAsync()
    {
        while (true)
        {
            await Console.Out.WriteAsync("Enter preferred locale: ");
            string locale = (await Console.In.ReadLineAsync())?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(locale)) { await Console.Out.WriteLineAsync("Locale is required"); continue; }
            else if (!DataManager.Locales.AsParallel().Any(l => l.Code == locale)) { await Console.Out.WriteLineAsync("Invalid locale"); continue; }
            return locale;
        }
    }

    public static async Task<decimal> GetOpeningDepositAsync()
    {
        while (true)
        {
            await Console.Out.WriteAsync("Opening deposit: ");
            if (!decimal.TryParse(await Console.In.ReadLineAsync(), out decimal balance)) { await Console.Out.WriteLineAsync("Opening deposit must be numeric"); continue; }
            else if (balance < 0) { await Console.Out.WriteLineAsync("Opening deposit must be positive"); continue; }
            return balance;
        }
    }

    public static async Task<decimal> GetAmountAsync(string action)
    {
        while (true)
        {
            await Console.Out.WriteAsync($"Enter amount to {action}: ");
            if (!decimal.TryParse(await Console.In.ReadLineAsync(), out decimal amount)) { await Console.Out.WriteLineAsync("Amount must be numeric"); continue; }
            else if (amount <= 0) { await Console.Out.WriteLineAsync("Amount must be positive"); continue; }
            return amount;
        }
    }

    public static async Task<string> GetReceiverAsync()
    {
        while (true)
        {
            await Console.Out.WriteAsync($"Enter receiver's name: ");
            var receiver = (await Console.In.ReadLineAsync())?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(receiver)) { await Console.Out.WriteLineAsync("Receiver name is required"); continue; }
            return receiver;
        }
    }

    public static string GetTypeName(int type) => type switch { 1 => nameof(CheckingAccount), 2 => nameof(SavingsAccount), 3 => nameof(LoanAccount), 4 => nameof(FixedDepositAccount), _ => throw new BadRequestException("Invalid account type") };
}
