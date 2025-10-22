using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Services;

DataManager.Load();
while (true)
{
    try
    {
        if (!AuthService.IsAuthenticated)
        {
            Console.WriteLine("=== MINIBANK (not signed in) ===");
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");
            Console.Write("\nChoose your option: ");

            if (!int.TryParse(Console.ReadLine(), out var authOption))
                throw new BadRequestException("Option must be numeric");
            Console.WriteLine();
            switch (authOption)
            {
                case 1: AuthService.Register(); break;
                case 2: AuthService.Login(); break;
                case 3: Console.WriteLine("Thanks for using the app"); return;
                default: throw new BadRequestException("Invalid option");
            }
        }
        else
        {
            Console.WriteLine($"=== MINIBANK (signed in: {AuthService.CurrentUsername}) ===");
            Console.WriteLine("1. List accounts");
            Console.WriteLine("2. Create account");
            Console.WriteLine("3. Deposit");
            Console.WriteLine("4. Withdraw");
            Console.WriteLine("5. View statement");
            Console.WriteLine("6. Run month-end");
            Console.WriteLine("7. Transfer");
            Console.WriteLine("8. Logout");
            Console.WriteLine("9. Exit");
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
                case 8: AuthService.Logout(); break;
                case 9: Console.WriteLine("Thanks for using the app"); return;
                default: throw new BadRequestException("Invalid option");
            }
        }
    }
    catch (Exception exception) { Console.WriteLine($"Error: {exception.Message}"); }
    finally
    {
        Console.WriteLine("\nPress enter to select another option");
        Console.ReadLine();
        Console.Clear();
    }
}
