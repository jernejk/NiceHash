using NiceHash.Core.Models;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace NiceHash.ElgatoStreamDeck.Actions;

[ActionUuid(Uuid = "NiceHash.StartRig")]
public class NiceHashStartRigAction : BaseNiceHashAction
{
    public override async Task OnLongPress(StreamDeckEventPayload args)
        => await UpdateDisplay(args);

    public override async Task OnTap(StreamDeckEventPayload args)
    {
        MiningRig miner = await NiceHashService.StartRig();
        await ShowStatus(args, miner);

        await Manager.ShowOkAsync(args.context);
    }

    public override async Task UpdateDisplay(StreamDeckEventPayload args)
    {
        MiningRig miner = await NiceHashService.GetMinerStatus();
        await ShowStatus(args, miner);
    }

    private async Task ShowStatus(StreamDeckEventPayload args, MiningRig miner)
    {
        if (miner == null)
        {
            await Manager.ShowAlertAsync(args.context);
            return;
        }

        await Manager.SetTitleAsync(args.context, $"{miner.Name}\n{miner.MinerStatus}");
    }
}
