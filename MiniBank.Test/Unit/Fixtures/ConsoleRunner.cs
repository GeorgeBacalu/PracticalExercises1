using System.Text;

namespace MiniBank.Test.Unit.Fixtures;
public static class ConsoleRunner
{
    public static async Task<(T result, string output)> RunAsync<T>(string input, Func<Task<T>> run)
    {
        var oldIn = Console.In;
        var oldOut = Console.Out;
        using var reader = new StringReader(input);
        var stringBuilder = new StringBuilder();
        using var writer = new StringWriter(stringBuilder);
        Console.SetIn(reader);
        Console.SetOut(writer);
        try { return (await run(), stringBuilder.ToString()); }
        finally { Console.SetIn(oldIn); Console.SetOut(oldOut); }
    }

    public static async Task<string> RunAsync(string input, Func<Task> run) => (await RunAsync(input, async () => { await run(); return 0; })).output;
}
