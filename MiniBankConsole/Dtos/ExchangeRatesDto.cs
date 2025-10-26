using System.Text.Json.Serialization;

namespace MiniBankConsole.Dtos;
public class ExchangeRatesDto
{
    [JsonPropertyName("base")] public string BaseCurrency { get; init; } = "USD";
    [JsonPropertyName("rates")] public Dictionary<string, decimal> Rates { get; set; } = [];
}
