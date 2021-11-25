using NiceHash.ElgatoStreamDeck.Models;
using NiceHash.ElgatoStreamDeck.Services;
using StreamDeckLib.Messages;

namespace NiceHash.ElgatoStreamDeck.Actions;

public abstract class BaseNiceHashAction : BaseElgateStreamDeckAction<NiceSettingsModel>
{
    private readonly StreamDeckNiceHashService _niceHashService = new();

    public StreamDeckNiceHashService NiceHashService { get { return _niceHashService; } }

    public override TimeSpan GetUpdateInterval() => TimeSpan.FromMinutes(1);

    public async override Task OnDidReceiveSettings(StreamDeckEventPayload args)
    {
        _niceHashService.UpdateSettings(SettingsModel);

        await base.OnDidReceiveSettings(args);
    }

    public override bool IsSettingsValid()
    {
        _niceHashService.UpdateSettings(SettingsModel);
        return NiceHashService.IsSettingsValid();
    }

    public override async Task OnError(StreamDeckEventPayload args, Exception ex)
    {
        await Manager.ShowAlertAsync(args.context);
    }
}
