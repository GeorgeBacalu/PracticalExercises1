using MiniBankConsole.Models;
using System.Text.Json;

namespace MiniBankConsole.Helpers;
public static class DataManager
{
    public static List<BankAccount> Accounts { get; } = [];

    private static readonly string JsonPath = Path.GetFullPath("../../../accounts.json");
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static void Load()
    {
        Accounts.Clear();
        if (!File.Exists(JsonPath)) return;
        Accounts.AddRange(JsonSerializer.Deserialize<List<BankAccount>>(File.ReadAllText(JsonPath)) ?? []);
    }

    public static void Save() => File.WriteAllText(JsonPath, JsonSerializer.Serialize(Accounts, JsonOptions));
}
