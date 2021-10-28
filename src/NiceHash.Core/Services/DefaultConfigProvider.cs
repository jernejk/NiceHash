using NiceHash.Core.Config;

namespace NiceHash.Core.Services;

internal class DefaultConfigProvider : IConfigProvider
{
    private readonly NiceHashConfig _config;

    public DefaultConfigProvider(NiceHashConfig config)
    {
        _config = config;
    }

    public NiceHashConfig GetConfig() => _config;
}
