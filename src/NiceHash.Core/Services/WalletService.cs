using NiceHash.Core.Models;

namespace NiceHash.Core.Services;

public interface IWalletService
{
    Task<Dictionary<string, double>?> GetCurrencies(CancellationToken ct = default);
    Task<CurrencyResponse?> GetWalletCurrencies(CancellationToken ct = default);
}

internal class WalletService : IWalletService
{
    private readonly INiceHashService _niceHashService;

    public WalletService(INiceHashService niceHashService)
    {
        _niceHashService = niceHashService;
    }

    public async Task<CurrencyResponse?> GetWalletCurrencies(CancellationToken ct = default)
    {
        return await _niceHashService.Get<CurrencyResponse>("/main/api/v2/accounting/accounts2", ct);
    }

    public async Task<Dictionary<string, double>?> GetCurrencies(CancellationToken ct = default)
        => await _niceHashService.GetAnnonymous<Dictionary<string, double>>("/exchange/api/v2/info/prices", ct);
}
