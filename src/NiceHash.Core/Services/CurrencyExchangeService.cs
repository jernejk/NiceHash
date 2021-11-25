using Microsoft.Extensions.Caching.Memory;
using NiceHash.Core.Models;
using System.Net.Http.Json;

namespace NiceHash.Core.Services;

public interface ICurrencyExchangeService
{
    Task<double> GetExchangeRate(string currency, CancellationToken ct = default);
    Task<double> GetExchangeRateInMainCurrency(CancellationToken ct = default);
    Task<Dictionary<string, double>?> GetExchangeRates(CancellationToken ct = default);
    string? GetMainCurrency();
}

internal class CurrencyExchangeService : ICurrencyExchangeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfigProvider _configProvider;
    private readonly IMemoryCache _memoryCache;

    public CurrencyExchangeService(IHttpClientFactory httpClientFactory, IConfigProvider configProvider, IMemoryCache memoryCache)
    {
        _httpClientFactory = httpClientFactory;
        _configProvider = configProvider;
        _memoryCache = memoryCache;
    }

    public async Task<Dictionary<string, double>?> GetExchangeRates(CancellationToken ct = default)
    {
        return await _memoryCache.GetOrCreateAsync("ExchangeRate", async e =>
        {
            try
            {
                var config = _configProvider.GetConfig();
                string apiKey = !string.IsNullOrWhiteSpace(config.ApiKey)
                    ? config.ApiKey
                    : string.Empty;

                HttpClient? httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetFromJsonAsync<FreeCurrencyExchangeRateResponse>($"https://freecurrencyapi.net/api/v2/latest?apikey={config.FreeCurrencyApiKey}", ct);

                if (response?.ExchangeRates?.Any() == true)
                {
                    e.SetSlidingExpiration(TimeSpan.FromMinutes(30));
                    return response.ExchangeRates;
                }
            }
            catch
            {
                // TODO: Log...
            }

            // If it fails for some reason, return null for 1 minutes.
            e.SetSlidingExpiration(TimeSpan.FromMinutes(1));
            return null;
        });
    }

    public async Task<double> GetExchangeRateInMainCurrency(CancellationToken ct = default)
    {
        var config = _configProvider.GetConfig();
        return await GetExchangeRate(config.MainCurrency, ct);
    }

    public async Task<double> GetExchangeRate(string currency, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(currency) || "usd".Equals(currency, StringComparison.OrdinalIgnoreCase))
        {
            // We already get values in USD from NiceHash.
            return 1;
        }

        var exchangeRates = await GetExchangeRates(ct);
        currency = currency.ToUpperInvariant();

        // If currency is not matched, return 1.
        return exchangeRates?.TryGetValue(currency, out var rate) == true
            ? rate
            : 1;
    }

    public string? GetMainCurrency()
    {
        var config = _configProvider.GetConfig();
        return config?.MainCurrency?.ToUpperInvariant()
            ?? "USD";
    }
}
