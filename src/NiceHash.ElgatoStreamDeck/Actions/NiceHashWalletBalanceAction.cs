using NiceHash.ElgatoStreamDeck.Actions;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace NiceHash.ElgatoStreamDeck;

[ActionUuid(Uuid = "NiceHash.WalletBalance")]
public class NiceHashWalletBalanceAction : BaseNiceHashAction
{
    public override async Task OnLongPress(StreamDeckEventPayload args)
    {
        await NiceHashService.ToggleRigStatus();
        await UpdateWalletBalance(args);
    }

    public override async Task UpdateDisplay(StreamDeckEventPayload args)
        => await UpdateWalletBalance(args);

    public override async Task OnTap(StreamDeckEventPayload args)
        => await UpdateWalletBalance(args);

    private async Task UpdateWalletBalance(StreamDeckEventPayload args)
    {
        string balance = await NiceHashService.GetCurrentWalletBalance();
        await Manager.SetTitleAsync(args.context, balance);
    }
}
