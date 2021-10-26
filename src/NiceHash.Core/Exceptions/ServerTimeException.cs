namespace NiceHash.Core.Exceptions;

public class ServerTimeException : Exception
{
    public ServerTimeException()
        : base("Unable to get server time") { }

    public ServerTimeException(Exception exception)
        : base("Unable to get server time", exception) { }
}
