using System.Security.Cryptography;
using System.Text;

namespace NiceHash.Core.Utils;

internal class CryptoUtils
{
    public static string HashBySegments(string key, string apiKey, string time, string nonce, string orgId, string method, string url, string? bodyStr)
    {
        (string path, string? query) = UrlUtils.GetPathFragments(url);
        List<string?> segments = new()
        {
            apiKey,
            time,
            nonce,
            null,
            orgId,
            null,
            method,
            path,
            query,
        };

        if (bodyStr?.Length > 0)
        {
            segments.Add(bodyStr);
        }

        return CalcHMACSHA256Hash(JoinSegments(segments), key);
    }

    private static string CalcHMACSHA256Hash(string plaintext, string salt)
    {
        HMACSHA256 hasher = new(Encoding.Default.GetBytes(salt));
        byte[] baHashedText = hasher.ComputeHash(Encoding.Default.GetBytes(plaintext));
        return string.Join("", baHashedText.Select(b => b.ToString("x2")));
    }

    private static string JoinSegments(List<string?> segments)
    {
        StringBuilder sb = new();
        for (int i = 0; i < segments.Count; i++)
        {
            if (i > 0)
            {
                sb.Append("\x00");
            }

            string? segment = segments[i];
            if (segment != null)
            {
                sb.Append(segments[i]);
            }
        }

        return sb.ToString();
    }
}
