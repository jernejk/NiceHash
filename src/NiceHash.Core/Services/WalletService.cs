using NiceHash.Core.Models;

namespace NiceHash.Core.Services;

public interface IWalletService
{
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
}
