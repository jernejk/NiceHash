using NiceHash.Core.Config;
using NiceHash.Core.Services;
using NiceHash.ElgatoStreamDeck.Models;

namespace NiceHash.ElgatoStreamDeck.Services;

/// <summary>
/// Elgato Stream Deck config can change anytime, so we need to make sure that NiceHash API is using the latests config.
/// </summary>
internal class InMemoryConfigProvider : IConfigProvider
{
    public NiceHashConfig Config { get; set; } = new();

    public NiceHashConfig GetConfig() => Config;

    public void Update(NiceSettingsModel settingsModel)
    {
        Config.OrganizationId = settingsModel.OrganizationId;
        Config.ApiSecret = settingsModel.ApiSecret;
        Config.ApiKey = settingsModel.ApiKey;

        Config.MainCurrency = settingsModel.MainCurrency;
        Config.FreeCurrencyApiKey = settingsModel.FreeCurrencyApiKey;

        if (Uri.TryCreate(settingsModel.BaseUrl, UriKind.Absolute, out Uri niceHashApiUrl))
        {
            Config.BaseUri = niceHashApiUrl;
        }
        else
        {
            Config.BaseUri = new Uri("https://api2.nicehash.com");
        }
    }
}
