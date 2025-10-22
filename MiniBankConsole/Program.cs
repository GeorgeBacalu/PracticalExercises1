using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Services;

DataManager.Load();
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
        Console.WriteLine("7. Transfer");
        Console.WriteLine("8. Exit");
        Console.Write("\nChoose your option: ");

        if (!int.TryParse(Console.ReadLine(), out var option))
            throw new BadRequestException("Option must be numeric");
        Console.WriteLine();
        switch (option)
        {
            case 1: AccountRegistry.GetAccounts(); break;
            case 2: AccountRegistry.CreateAccount(); DataManager.Save(); break;
            case 3: AccountRegistry.Deposit(); DataManager.Save(); break;
            case 4: AccountRegistry.Withdraw(); DataManager.Save(); break;
            case 5: AccountRegistry.ViewStatement(); break;
            case 6: AccountRegistry.RunMonthEndProcessing(); DataManager.Save(); break;
            case 7: AccountRegistry.Transfer(); DataManager.Save(); break;
            case 8: Console.WriteLine("Thanks for using the app"); return;
            default: throw new BadRequestException("Invalid option");
        }
    }
    catch (Exception exception) { Console.WriteLine($"Error: {exception.Message}"); }
    finally
    {
        Console.WriteLine("\nPress enter to select another option");
        Console.ReadKey();
        Console.Clear();
    }
}
