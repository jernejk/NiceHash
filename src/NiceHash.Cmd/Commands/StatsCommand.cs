using NiceHash.Core.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NiceHash.Cmd.Commands;

internal class StatsCommand : AsyncCommand
{
    private readonly IWalletService _walletService;
    private readonly IRigsManagementService _rigsManagementService;

    public StatsCommand(IWalletService walletService, IRigsManagementService rigsManagementService)
    {
        _walletService = walletService;
        _rigsManagementService = rigsManagementService;
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
                AnsiConsole.MarkupLine($"Balance: [#999]{wallet.Currencies[0].Available}[/] {wallet.Currencies[0].Currency}");

                ctx.Status = "Getting worker stats...";
                ctx.Refresh();

                var workers = await _rigsManagementService.GetActiveWorkers();

                foreach (var worker in workers.Workers)
                {
                    AnsiConsole.MarkupLine($"Worker: {worker.RigName}; Speed: [#999]{worker.SpeedAccepted}[/]; Profitability: [#999]{worker.Profitability}[/]");
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
