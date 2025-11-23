using MiniBank.App.Helpers;
using MiniBank.App.Models;

namespace MiniBank.Test.Unit.Helpers;
public class DataManagerTest
{
    [Fact] public async Task SaveAndLoadAsync_ShouldPersistAccounts()
    {
        DataManager.Accounts.Clear();
        DataManager.Accounts.Add(new CheckingAccount { Owner = "newuser", Password = "123456" });

        await DataManager.SaveAsync();
        DataManager.Accounts.Clear();

        await DataManager.LoadAsync();
        Assert.Contains(DataManager.Accounts, account => account.Owner == "newuser");
    }
}
