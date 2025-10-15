using MiniBankConsole.Exceptions;
using MiniBankConsole.Services;

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
                AccountRegistry.Deposit();
                break;
            case 4:
                AccountRegistry.Withdraw();
                break;
            case 5:
                AccountRegistry.ViewStatement();
                break;
            case 6:
                AccountRegistry.RunMonthEnd();
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
