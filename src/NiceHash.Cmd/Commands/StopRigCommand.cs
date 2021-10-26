using NiceHash.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NiceHash.Cmd.Commands;

internal class StopRigCommand : AsyncCommand
{
    private readonly IRigsManagementService _rigsManagementService;

    public StopRigCommand(IRigsManagementService rigsManagementService)
    {
        _rigsManagementService = rigsManagementService;
    }

    public async override Task<int> ExecuteAsync(CommandContext context)
    {
        await AnsiConsole.Status()
            .AutoRefresh(true)
            .Spinner(Spinner.Known.Default)
            .SpinnerStyle(Style.Parse("green bold"))
            .StartAsync("Starting rigs...", async ctx =>
            {
                var rigs = await _rigsManagementService.GetRigs();
                foreach (var rig in rigs.MiningRigs)
                {
                    string miningStatusColor = rig.MinerStatus == "MINING" ? "green" : "red";
                    AnsiConsole.MarkupLine($"Stopping rig {rig.Name} ({rig.RigId}); Status: [{miningStatusColor}]{rig.MinerStatus}[/]; Power: [#999]{rig.Devices.Where(x => x.PowerUsage > 0).Sum(x => x.PowerUsage)}[/] W");

                    var status = await _rigsManagementService.StopRig(rig.RigId);
                    if (status.Success)
                    {
                        AnsiConsole.MarkupLine($"Rig stopped!");
                    }
                }

                ctx.Refresh();
            });

        return 0;
    }
}
