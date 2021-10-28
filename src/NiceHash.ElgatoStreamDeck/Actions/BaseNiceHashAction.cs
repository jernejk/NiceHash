using NiceHash.ElgatoStreamDeck.Models;
using NiceHash.ElgatoStreamDeck.Services;
using StreamDeckLib;
using StreamDeckLib.Messages;

namespace NiceHash.ElgatoStreamDeck.Actions;

public abstract class BaseNiceHashAction : BaseStreamDeckActionWithSettingsModel<NiceSettingsModel>
{
    private readonly StreamDeckNiceHashService _niceHashService = new();
    private DateTime _pressDownDateTime;

    public StreamDeckNiceHashService NiceHashService { get { return _niceHashService; } }

    public override async Task OnDidReceiveSettings(StreamDeckEventPayload args)
    {
        await base.OnDidReceiveSettings(args);

        _niceHashService.UpdateSettings(SettingsModel);
    }

    public override Task OnKeyDown(StreamDeckEventPayload args)
    {
        _pressDownDateTime = DateTime.Now;

        return base.OnKeyDown(args);
    }

    public override async Task OnKeyUp(StreamDeckEventPayload args)
    {
        try
        {
            var now = DateTime.Now;
            var delta = now.Subtract(_pressDownDateTime);

            if (delta.TotalSeconds > 1)
            {
                await OnLongPress(args);
            }
            else
            {
                await OnTap(args);
            }
        }
        catch
        {
            await Manager.ShowAlertAsync(args.context);
        }
    }

    public override async Task OnWillAppear(StreamDeckEventPayload args)
    {
        await base.OnWillAppear(args);

        _niceHashService.UpdateSettings(SettingsModel);
        if (_niceHashService.IsSettingsValid())
        {
            await UpdateDisplay(args);
        }
    }

    public abstract Task UpdateDisplay(StreamDeckEventPayload args);
    public abstract Task OnTap(StreamDeckEventPayload args);
    public abstract Task OnLongPress(StreamDeckEventPayload args);

    public virtual Task MissingSettings(StreamDeckEventPayload args)
    {
        return Task.CompletedTask;
    }
}
