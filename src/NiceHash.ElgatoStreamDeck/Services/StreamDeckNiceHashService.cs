using Microsoft.Extensions.DependencyInjection;
using NiceHash.Core;
using NiceHash.Core.Models;
using NiceHash.Core.Services;
using NiceHash.ElgatoStreamDeck.Models;

namespace NiceHash.ElgatoStreamDeck.Services;

public class StreamDeckNiceHashService
{
    private readonly InMemoryConfigProvider _configProvider = new();
    private readonly IRigsManagementService _rigsManagementService;
    private readonly IWalletService _walletService;

    public StreamDeckNiceHashService()
    {
        var sp = new ServiceCollection()
            .AddLogging()
            .AddNiceHash(_configProvider)
            .BuildServiceProvider();

        _rigsManagementService = sp.GetService<IRigsManagementService>();
        _walletService = sp.GetService<IWalletService>();
    }

    public void UpdateSettings(NiceSettingsModel settingsModel)
        => _configProvider.Update(settingsModel);

    public bool IsSettingsValid()
        => _configProvider.Config != null
            && _configProvider.Config.BaseUri != null
            && !string.IsNullOrWhiteSpace(_configProvider.Config.OrganizationId)
            && !string.IsNullOrWhiteSpace(_configProvider.Config.ApiKey)
            && !string.IsNullOrWhiteSpace(_configProvider.Config.ApiSecret);

    public async Task<string> GetCurrentWalletBalance()
    {
        try
        {
            var wallet = await _walletService.GetWalletCurrencies();

            if (wallet?.Currencies?.Any() == true)
            {
                CurrencyInfo highestCurrency = wallet.Currencies.MaxBy(x => x.Available);
                string result = $"{Math.Round(highestCurrency.Available, 5)}\n{highestCurrency.Currency}";

                MiningRig miner = await GetMinerStatus();
                if (miner != null)
                {
                    result += "\n" + miner.MinerStatus;
                }

                return result;
            }
            else
            {
                return "No wallet balance found";
            }
        }
        catch
        {
            return "Unable to get balance";
        }
    }

    public async Task<MiningRig> GetMinerStatus()
    {
        try
        {
            var rigs = await _rigsManagementService.GetRigs();

            return rigs.MiningRigs
                .FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }

    public async Task<MiningRig> StartRig()
    {
        await ChangeRigStatus("START");

        // Get latest details about the miner.
        return await GetMinerStatus();
    }

    public async Task<MiningRig> StopRig()
    {
        await ChangeRigStatus("STOP");

        // Get latest details about the miner.
        return await GetMinerStatus();
    }

    public async Task ToggleRigStatus()
    {
        await ChangeRigStatus("TOGGLE");
    }

    private async Task ChangeRigStatus(string action)
    {
        var rigs = await _rigsManagementService.GetRigs();
        foreach (var rig in rigs.MiningRigs)
        {
            if (action == "TOGGLE")
            {
                action = rig.MinerStatus == "MINING" ? "STOP" : "START";
            }

            await _rigsManagementService.RunRigAction(rig.RigId, action);
        }
    }
}
