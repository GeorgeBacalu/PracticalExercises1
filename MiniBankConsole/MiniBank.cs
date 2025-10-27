using MiniBankConsole.Exceptions;
using MiniBankConsole.Helpers;
using MiniBankConsole.Services;

namespace MiniBankConsole;
public static class MiniBank
{
    public static async Task RunAsync()
    {
        await DataManager.LoadAsync();
        while (true)
        {
            try
            {
                if (!AuthService.IsAuthenticated)
                {
                    await Console.Out.WriteAsync("""
                    === MINIBANK (not signed in) ===
                    1. Register
                    2. Login
                    3. Exit
                    
                    Choose your option: 
                    """);
                    
                    if (!int.TryParse(await Console.In.ReadLineAsync(), out var option)) throw new BadRequestException("Option must be numeric");
                    await Console.Out.WriteLineAsync();
                    switch (option)
                    {
                        case 1: await AuthService.RegisterAsync(); await DataManager.SaveAsync(); break;
                        case 2: await AuthService.LoginAsync(); break;
                        case 3: await Console.Out.WriteLineAsync("Thanks for using the app"); return;
                        default: throw new BadRequestException("Invalid option");
                    }
                }
                else
                {
                    await Console.Out.WriteAsync($"""
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
                    
                    if (!int.TryParse(await Console.In.ReadLineAsync(), out var option)) throw new BadRequestException("Option must be numeric");
                    await Console.Out.WriteLineAsync();
                    switch (option)
                    {
                        case 1: await AccountRegistry.GetAccountsAsync(); break;
                        case 2: await AccountRegistry.CreateAccountAsync(); await DataManager.SaveAsync(); break;
                        case 3: await AccountRegistry.DepositAsync(); await DataManager.SaveAsync(); break;
                        case 4: await AccountRegistry.WithdrawAsync(); await DataManager.SaveAsync(); break;
                        case 5: await AccountRegistry.ViewStatementAsync(); break;
                        case 6: await AccountRegistry.RunMonthEndProcessingAsync(); await DataManager.SaveAsync(); break;
                        case 7: await AccountRegistry.TransferAsync(); await DataManager.SaveAsync(); break;
                        case 8: await AuthService.LogoutAsync(); break;
                        case 9: await Console.Out.WriteLineAsync("Thanks for using the app"); return;
                        default: throw new BadRequestException("Invalid option");
                    }
                }
            }
            catch (Exception exception) { await Console.Out.WriteLineAsync($"Error: {exception.Message}"); }
            finally
            {
                await Console.Out.WriteAsync("\nPress any key to continue");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }
}
