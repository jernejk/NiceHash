namespace NiceHash.Core.Utils;

internal class UrlUtils
{
    public static (string path, string? query) GetPathFragments(string path)
    {
        string[] fragments = path.Split('?');
        return fragments.Length > 1
            ? (fragments[0], fragments[1])
            : (fragments[0], null);
    }
}
