using NiceHash.Core.Config;

namespace NiceHash.Core.Services;

public interface IConfigProvider
{
    NiceHashConfig GetConfig();
}
