using System.Text.Json.Serialization;

namespace NiceHash.Core.Models;

public class FreeCurrencyExchangeRateResponse
{
    public FreeCurrencyExchangeRateQuery Query { get; set; }
    [JsonPropertyName("data")]
    public Dictionary<string, double> ExchangeRates { get; set; }
}

public class FreeCurrencyExchangeRateQuery
{
    public string ApiKey { get; set; }
    [JsonPropertyName("timestamp")]
    public int Timestamp { get; set; }
    [JsonPropertyName("base_currency")]
    public string BaseCurrency { get; set; }

    public DateTime Updated => DateTime.UnixEpoch.AddSeconds(Timestamp);
}
