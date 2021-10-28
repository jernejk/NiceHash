using NiceHash.Core.Services;
using Spectre.Console;
using Spectre.Console.Cli;

namespace NiceHash.Cmd.Commands;

internal class StartRigCommand : AsyncCommand
{
    private readonly IRigsManagementService _rigsManagementService;

    public StartRigCommand(IRigsManagementService rigsManagementService)
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
                    if (rig.MinerStatus != "MINING")
                    {
                        AnsiConsole.MarkupLine($"Starting rig {rig.Name} ({rig.RigId}); Status: [{miningStatusColor}]{rig.MinerStatus}[/]; Power: [#999]{rig.Devices.Where(x => x.PowerUsage > 0).Sum(x => x.PowerUsage)}[/] W");

                        var status = await _rigsManagementService.StartRig(rig.RigId);
                        if (status.Success)
                        {
                            AnsiConsole.MarkupLine($"Rig started!");
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"Rig {rig.Name} ({rig.RigId}) already running; Status: [{miningStatusColor}]{rig.MinerStatus}[/]; Power: [#999]{rig.Devices.Where(x => x.PowerUsage > 0).Sum(x => x.PowerUsage)}[/] W");
                    }
                }

                ctx.Refresh();
            });

        return 0;
    }
}
