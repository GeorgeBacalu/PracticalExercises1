using MiniBankConsole.Exceptions;
using MiniBankConsole.Services;

string? owner;

while (true)
{
    try
    {
        Console.WriteLine("=== MINIBANK ===");
        Console.WriteLine("1. List accounts");
        Console.WriteLine("2. Create account");
        Console.WriteLine("3. Deposit");
        Console.WriteLine("4. Withdraw");
        Console.WriteLine("5. View statement");
        Console.WriteLine("6. Run month-end");
        Console.WriteLine("7. Exit");
        Console.Write("Choose your option: ");

        int option = Convert.ToInt32(Console.ReadLine());
        switch (option)
        {
            case 1:
                AccountRegistry.GetAccounts();
                break;
            case 2:
                AccountRegistry.CreateAccount();
                break;
            case 3:
                Console.Write("Enter owner name: ");
                owner = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(owner))
                    Console.WriteLine("Owner name is required");
                Console.Write("Enter amount to deposit: ");
                var depositAmountString = Console.ReadLine();
                if (!decimal.TryParse(depositAmountString, out var depositAmount))
                {
                    Console.WriteLine("Amount must be numeric");
                    break;
                }
                if (depositAmount <= 0)
                {
                    Console.WriteLine("Amount must be positive");
                    break;
                }
                AccountRegistry.Deposit(owner, depositAmount);
                break;
            case 4:
                Console.Write("Enter owner name: ");
                owner = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(owner))
                    Console.WriteLine("Owner name is required");
                Console.Write("Enter amount to withdraw: ");
                var withdrawAmountString = Console.ReadLine();
                if (!decimal.TryParse(withdrawAmountString, out var withdrawAmount))
                {
                    Console.WriteLine("Amount must be numeric");
                    break;
                }
                if (withdrawAmount <= 0)
                {
                    Console.WriteLine("Amount must be positive");
                    break;
                }
                AccountRegistry.Withdraw(owner, withdrawAmount);
                break;
            case 5:
                Console.Write("Enter owner name: ");
                owner = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(owner))
                    Console.WriteLine("Owner name is required");
                AccountRegistry.ViewStatement(owner);
                break;
            case 6:
                AccountRegistry.RunMonthEndProcessing();
                break;
            case 7:
                Console.WriteLine("Thanks for using the app");
                return;
            default:
                throw new BadRequestException("Invalid option");
        }
        Console.WriteLine("Press enter to select another option");
        Console.ReadKey();
        Console.Clear();
    }
    catch (Exception exception)
    {
        Console.WriteLine($"Error: {exception.Message}");
    }
}
