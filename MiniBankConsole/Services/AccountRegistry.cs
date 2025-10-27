using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Models;
using MiniBankConsole.Models.Interfaces;

namespace MiniBankConsole.Services;
public static class AccountRegistry
{
    public static async Task GetAccountsAsync()
    {
        var accounts = AuthService.IsAuthenticated ? [.. DataManager.Accounts.AsParallel().Where(account => account.Owner == AuthService.CurrentUser)] : DataManager.Accounts;
        if (!accounts.Any()) { await Console.Out.WriteLineAsync("No accounts found"); return; }

        await Console.Out.WriteLineAsync("Bank accounts: ");
        Parallel.ForEach(accounts, Console.WriteLine);
    }

    public static async Task CreateAccountAsync()
    {
        int type = await InputHelper.GetAccountTypeAsync();
        if (DataManager.Accounts.AsParallel().Any(account => account.Owner == AuthService.CurrentUser && account.GetType().Name == InputHelper.GetTypeName(type))) throw new BadRequestException("User already has an account of this type");
        decimal balance = await InputHelper.GetOpeningDepositAsync();
        string currency = await InputHelper.GetCurrencyAsync(), locale = await InputHelper.GetLocaleAsync();
        string password = AuthService.IsAuthenticated ? DataManager.Accounts.AsParallel().First(account => account.Owner == AuthService.CurrentUser).Password : AuthService.CurrentUser;

        BankAccount account = type switch
        {
            1 => new CheckingAccount { Owner = AuthService.CurrentUser, Password = password, Balance = balance, Currency = currency, Locale = locale },
            2 => new SavingsAccount { Owner = AuthService.CurrentUser, Password = password, Balance = balance, Currency = currency, Locale = locale },
            3 => new LoanAccount { Owner = AuthService.CurrentUser, Password = password, Balance = -balance, Currency = currency, Locale = locale },
            4 => new FixedDepositAccount() { Owner = AuthService.CurrentUser, Password = password, Balance = balance, Currency = currency, Locale = locale },
            _ => throw new BadRequestException("Invalid account type")
        };
        DataManager.Accounts.Add(account);
        account.Transactions.Add(new() { Type = type == 3 ? TransactionType.Withdraw : TransactionType.Deposit, Amount = balance, AccountId = account.Id });
        await Console.Out.WriteLineAsync($"\nCreated #{DataManager.Accounts.Count} {account.GetType().Name} for {AuthService.CurrentUser} with balance {CurrencyFormatter.Format(balance, account.Currency, account.Locale)}");
    }

    public static async Task DepositAsync()
    {
        int type = await InputHelper.GetAccountTypeAsync();
        decimal amount = await InputHelper.GetAmountAsync("deposit");

        var account = GetAccount(AuthService.CurrentUser, type);
        account.Deposit(amount);
        await Console.Out.WriteLineAsync($"\nDeposited {CurrencyFormatter.Format(amount, account.Currency, account.Locale)} to {AuthService.CurrentUser} ({account.GetType().Name})");
    }

    public static async Task WithdrawAsync()
    {
        int type = await InputHelper.GetAccountTypeAsync();
        decimal amount = await InputHelper.GetAmountAsync("withdraw");

        var account = GetAccount(AuthService.CurrentUser, type);
        if (!account.Withdraw(amount, out var error)) throw new BadRequestException(error ?? "Withdraw failed");
        await Console.Out.WriteLineAsync($"\nWithdrew {CurrencyFormatter.Format(amount, account.Currency, account.Locale)} from {AuthService.CurrentUser} ({account.GetType().Name})");
    }

    public static async Task ViewStatementAsync() => GetAccount(AuthService.CurrentUser, await InputHelper.GetAccountTypeAsync()).PrintStatement();

    public static async Task RunMonthEndProcessingAsync()
    {
        Parallel.ForEach(DataManager.Accounts.AsParallel().OfType<IInterestBearing>(), account => account.ApplyMonthlyInterest());
        await Console.Out.WriteLineAsync("Month-end applied (interest/fees)");
    }

    public static async Task TransferAsync()
    {
        string sender = AuthService.CurrentUser, receiver = await InputHelper.GetReceiverAsync();
        int senderType = await InputHelper.GetAccountTypeAsync("sender's account"), receiverType = await InputHelper.GetAccountTypeAsync("receiver's account");
        if (sender == receiver && senderType == receiverType) throw new BadRequestException("Sender and receiver must be different");

        decimal amount = await InputHelper.GetAmountAsync("transfer");
        var senderAccount = GetAccount(sender, senderType);
        var receiverAccount = GetAccount(receiver, receiverType);

        var creditedAmount = CurrencyConverter.Convert(amount, senderAccount.Currency, receiverAccount.Currency);
        if (receiverAccount is LoanAccount && receiverAccount.Balance + creditedAmount > 0) throw new BadRequestException("Transfer exceeds loan payoff amount");
        if (!senderAccount.Withdraw(amount, out var error)) throw new BadRequestException(error ?? "Transfer failed");
        receiverAccount.Deposit(creditedAmount);
        await Console.Out.WriteLineAsync($"\nTransferred {CurrencyFormatter.Format(amount, senderAccount.Currency, senderAccount.Locale)} from {sender} ({InputHelper.GetTypeName(senderType)}) to {receiver} ({InputHelper.GetTypeName(receiverType)}) credited as {CurrencyFormatter.Format(creditedAmount, receiverAccount.Currency, receiverAccount.Locale)}");
    }

    private static BankAccount GetAccount(string owner, int type) => DataManager.Accounts.AsParallel().FirstOrDefault(account => account.Owner == owner && account.GetType().Name == InputHelper.GetTypeName(type)) ?? throw new MissingResourceException("Account not found");
}
