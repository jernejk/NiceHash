using NiceHash.Core.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NiceHash.Cmd.Commands;

internal class StatsCommand : AsyncCommand
{
    private readonly IWalletService _walletService;
    private readonly IRigsManagementService _rigsManagementService;
    private readonly ICurrencyExchangeService _currencyExchangeService;

    public StatsCommand(IWalletService walletService, IRigsManagementService rigsManagementService, ICurrencyExchangeService currencyExchangeService)
    {
        _walletService = walletService;
        _rigsManagementService = rigsManagementService;
        _currencyExchangeService = currencyExchangeService;
    }

    public override async Task<int> ExecuteAsync(CommandContext context)
    {
        await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Default)
            .SpinnerStyle(Style.Parse("green bold"))
            .StartAsync("Getting Balance...", async ctx =>
            {
                var wallet = await _walletService.GetWalletCurrencies();
                if (wallet == null || wallet.Currencies?.Any() != true)
                {
                    AnsiConsole.MarkupLine($"No wallet found");
                    return;
                }

                AnsiConsole.MarkupLine($"Balance: [#999]{wallet.Currencies[0].Available}[/] {wallet.Currencies[0].Currency}");

                double usdToMainCurrencyExchangeRate = await _currencyExchangeService.GetExchangeRateInMainCurrency();

                ctx.Status = "Getting current currency exchange...";
                var currency = await _walletService.GetCurrencies();
                if (currency?.TryGetValue(wallet.Currencies[0].Currency + "USDC", out double exchangeRate) == true)
                {
                    exchangeRate *= usdToMainCurrencyExchangeRate;

                    AnsiConsole.MarkupLine($"Balance: [#999]{Math.Round(wallet.Currencies[0].Available * exchangeRate, 2)}[/] AUD");
                    AnsiConsole.WriteLine();
                }
                else
                {
                    AnsiConsole.MarkupLine("Exchange not found");
                    AnsiConsole.WriteLine();
                    exchangeRate = 0;
                }

                ctx.Status = "Getting worker stats...";
                ctx.Refresh();

                var workers = await _rigsManagementService.GetActiveWorkers();

                foreach (var worker in workers.Workers)
                {
                    AnsiConsole.MarkupLine($"Worker: {worker.RigName}; Speed: [#999]{worker.SpeedAccepted}[/] MH/s; Profitability: [#999]{worker.Profitability}[/]");
                    if (exchangeRate > 0)
                    {
                        AnsiConsole.MarkupLine($"Profitability: [#999]{Math.Round(worker.Profitability * exchangeRate, 2)} AUD[/]");
                        AnsiConsole.WriteLine();
                    }
                }

                ctx.Status = "Getting rigs stats...";
                ctx.Refresh();

                var rigs = await _rigsManagementService.GetRigs();
                foreach (var rig in rigs.MiningRigs)
                {
                    string miningStatusColor = rig.MinerStatus == "MINING" ? "green" : "red";
                    AnsiConsole.MarkupLine($"Rig: {rig.Name} ({rig.RigId}); Status: [{miningStatusColor}]{rig.MinerStatus}[/]; Power: [#999]{rig.Devices.Where(x => x.PowerUsage > 0).Sum(x => x.PowerUsage)}[/] W");
                }

                ctx.Refresh();
            });
        
        return 0;
    }
}
