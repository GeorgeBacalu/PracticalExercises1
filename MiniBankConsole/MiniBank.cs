using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Services;

namespace MiniBankConsole;
public static class MiniBank
{
    public static void Run()
    {
        DataManager.Load();
        while (true)
        {
            try
            {
                if (!AuthService.IsAuthenticated)
                {
                    Console.Write("""
                    === MINIBANK (not signed in) ===
                    1. Register
                    2. Login
                    3. Exit
                    
                    Choose your option: 
                    """);
                    
                    if (!int.TryParse(Console.ReadLine(), out var option)) throw new BadRequestException("Option must be numeric");
                    Console.WriteLine();
                    switch (option)
                    {
                        case 1: AuthService.Register(); DataManager.Save(); break;
                        case 2: AuthService.Login(); break;
                        case 3: Console.WriteLine("Thanks for using the app"); return;
                        default: throw new BadRequestException("Invalid option");
                    }
                }
                else
                {
                    Console.Write($"""
                    === MINIBANK (signed in: {AuthService.CurrentUser}) ===
                    1. List accounts
                    2. Create account
                    3. Deposit
                    4. Withdraw
                    5. View statement
                    6. Run month-end
                    7. Transfer
                    8. Logout
                    9. Exit
                    
                    Choose your option: 
                    """);
                    
                    if (!int.TryParse(Console.ReadLine(), out var option)) throw new BadRequestException("Option must be numeric");
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
                Console.WriteLine("\nPress any key to continue");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
