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
    private readonly ICurrencyExchangeService _currencyExchangeService;
    private readonly IWalletService _walletService;

    private string _currentStatus = "waiting";

    private int updates = 0;

    public StreamDeckNiceHashService()
    {
        var sp = new ServiceCollection()
            .AddLogging()
            .AddNiceHash(_configProvider)
            .BuildServiceProvider();

        _rigsManagementService = sp.GetService<IRigsManagementService>();
        _currencyExchangeService = sp.GetService<ICurrencyExchangeService>();
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
        ++updates;

        try
        {
            int options = 6;

            var wallet = await _walletService.GetWalletCurrencies();
            if (wallet?.Currencies?.Any() == true)
            {
                string result;
                MiningRig miner = await GetMinerStatus();
                CurrencyInfo highestCurrency = wallet.Currencies.MaxBy(x => x.Available);
                if (updates % options == 0)
                {
                    result = $"{(decimal)Math.Round(highestCurrency.Available, 5)}\n{highestCurrency.Currency}";
                }
                else if (updates % options == 1)
                {
                    double balanceInAud = highestCurrency.Available * await GetExchangeRate(highestCurrency.Currency);
                    result = $"{Math.Round(balanceInAud, 2)}\n{_currencyExchangeService.GetMainCurrency()}";
                }
                else if (updates % options == 2)
                {
                    IEnumerable<WorkerInfo> workers = await GetWorkers();
                    double profitInAud = workers.Sum(x => x.Profitability) * await GetExchangeRate(highestCurrency.Currency);
                    result = $"{Math.Round(profitInAud, 2)}\n{_currencyExchangeService.GetMainCurrency()}/day";
                }
                else if (updates % options == 3)
                {
                    double temps = miner.Devices.Max(x => x.Temperature);
                    double gpuTemps = temps % 65536;
                    double vramTemps = temps / 65536.0;
                    result = $"{Math.Round(gpuTemps, 1)}/{Math.Round(vramTemps, 1)}\n°C";
                }
                else if (updates % options == 4)
                {
                    var speeds = miner.Devices
                        .Where(x => x.Speeds?.Any() == true)
                        .SelectMany(x => x.Speeds)
                        .ToList();

                    double speed = speeds.Sum(x => x.Speed);
                    string displayUnit = speeds
                        .MaxBy(x => x.Speed)
                        ?.DisplaySuffix
                        ?? "MH";
                    result = $"{Math.Round(speed, 2)}\n{displayUnit}/s";
                }
                else
                {
                    double powerUsage = miner.Devices.Sum(x => x.PowerUsage);
                    result = $"{Math.Round(powerUsage, 1)}\nW";
                }

                
                if (miner != null)
                {
                    _currentStatus = miner.MinerStatus?.ToLowerInvariant() switch
                    {
                        "mining" => "mining",
                        "benchmarking" => "minig",
                        "inactive" => "sleeping",
                        "stopped" => "sleeping",
                        "pending" => "sleeping",
                        _ => "unavailable"
                    };
                }

                return result;
            }
            else
            {
                _currentStatus = "unavailable";
                return "No wallet balance found";
            }
        }
        catch
        {
            _currentStatus = "unavailable";
            return "Error";
        }
    }

    public string GetStatus() => _currentStatus;

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

    public async Task<IEnumerable<WorkerInfo>> GetWorkers()
    {
        try
        {
            var workers = await _rigsManagementService.GetActiveWorkers();
            return workers?.Workers;
        }
        catch
        {
            return null;
        }
    }

    public async Task<double> GetExchangeRate(string cryptoCurrency)
    {
        Dictionary<string, double> currency = await _walletService.GetCurrencies();
        if (currency?.TryGetValue($"{cryptoCurrency}USDC", out double usdExchangeRate) == true || currency?.TryGetValue($"{cryptoCurrency}USDT", out usdExchangeRate) == true)
        {
            double exchangeRate = await _currencyExchangeService.GetExchangeRateInMainCurrency();
            return usdExchangeRate *= exchangeRate;
        }
        else
        {
            return 1;
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
