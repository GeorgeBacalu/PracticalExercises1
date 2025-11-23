using System.Text.Json.Serialization;

namespace MiniBank.App.Dtos;
public class ExchangeRatesDto
{
    [JsonPropertyName("base")] public string BaseCurrency { get; init; } = "EUR";
    [JsonPropertyName("rates")] public Dictionary<string, decimal> Rates { get; set; } = [];
}
