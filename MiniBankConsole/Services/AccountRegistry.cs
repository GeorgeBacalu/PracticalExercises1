using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Models;
using MiniBankConsole.Models.Interfaces;

namespace MiniBankConsole.Services;
public static class AccountRegistry
{
    public static void GetAccounts()
    {
        var accounts = AuthService.IsAuthenticated ? [.. DataManager.Accounts.Where(account => account.Owner == AuthService.CurrentUser)] : DataManager.Accounts;
        if (!accounts.Any()) { Console.WriteLine("No accounts found"); return; }

        Console.WriteLine("Bank accounts: ");
        accounts.ForEach(Console.WriteLine);
    }

    public static void CreateAccount()
    {
        int type = InputHelper.GetAccountType();
        if (DataManager.Accounts.Any(account => account.Owner == AuthService.CurrentUser && account.GetType().Name == InputHelper.GetTypeName(type))) throw new BadRequestException("User already has an account of this type");
        decimal balance = InputHelper.GetOpeningDeposit();
        string currency = InputHelper.GetCurrency(), locale = InputHelper.GetLocale();
        string password = AuthService.IsAuthenticated ? DataManager.Accounts.First(account => account.Owner == AuthService.CurrentUser).Password : AuthService.CurrentUser;

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
        Console.WriteLine($"\nCreated #{DataManager.Accounts.Count} {account.GetType().Name} for {AuthService.CurrentUser} with balance {CurrencyFormatter.Format(balance, account.Currency, account.Locale)}");
    }

    public static void Deposit()
    {
        int type = InputHelper.GetAccountType();
        decimal amount = InputHelper.GetAmount("deposit");

        var account = GetAccount(AuthService.CurrentUser, type);
        account.Deposit(amount);
        Console.WriteLine($"\nDeposited {CurrencyFormatter.Format(amount, account.Currency, account.Locale)} to {AuthService.CurrentUser} ({account.GetType().Name})");
    }

    public static void Withdraw()
    {
        int type = InputHelper.GetAccountType();
        decimal amount = InputHelper.GetAmount("withdraw");

        var account = GetAccount(AuthService.CurrentUser, type);
        if (!account.Withdraw(amount, out var error)) throw new BadRequestException(error ?? "Withdraw failed");
        Console.WriteLine($"\nWithdrew {CurrencyFormatter.Format(amount, account.Currency, account.Locale)} from {AuthService.CurrentUser} ({account.GetType().Name})");
    }

    public static void ViewStatement() => GetAccount(AuthService.CurrentUser, InputHelper.GetAccountType()).PrintStatement();

    public static void RunMonthEndProcessing()
    {
        foreach (var account in DataManager.Accounts.OfType<IInterestBearing>()) account.ApplyMonthlyInterest();
        Console.WriteLine("Month-end applied (interest/fees)");
    }

    public static void Transfer()
    {
        string sender = AuthService.CurrentUser, receiver = InputHelper.GetReceiver();
        int senderType = InputHelper.GetAccountType("sender's account"), receiverType = InputHelper.GetAccountType("receiver's account");
        if (sender == receiver && senderType == receiverType) throw new BadRequestException("Sender and receiver must be different");

        decimal amount = InputHelper.GetAmount("transfer");
        var senderAccount = GetAccount(sender, senderType);
        var receiverAccount = GetAccount(receiver, receiverType);

        var creditedAmount = CurrencyConverter.Convert(amount, senderAccount.Currency, receiverAccount.Currency);
        if (receiverAccount is LoanAccount && receiverAccount.Balance + creditedAmount > 0) throw new BadRequestException("Transfer exceeds loan payoff amount");
        if (!senderAccount.Withdraw(amount, out var error)) throw new BadRequestException(error ?? "Transfer failed");
        receiverAccount.Deposit(creditedAmount);
        Console.WriteLine($"\nTransferred {CurrencyFormatter.Format(amount, senderAccount.Currency, senderAccount.Locale)} from {sender} ({InputHelper.GetTypeName(senderType)}) to {receiver} ({InputHelper.GetTypeName(receiverType)}) credited as {CurrencyFormatter.Format(creditedAmount, receiverAccount.Currency, receiverAccount.Locale)}");
    }

    private static BankAccount GetAccount(string owner, int type) => DataManager.Accounts.FirstOrDefault(account => account.Owner == owner && account.GetType().Name == InputHelper.GetTypeName(type)) ?? throw new MissingResourceException("Account not found");
}
