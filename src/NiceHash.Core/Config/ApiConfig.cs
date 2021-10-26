namespace NiceHash.Core.Config;

public class ApiConfig
{
    public ApiConfig() { }

    public ApiConfig(Uri baseUri, string organizationId, string apiKey, string apiSecret)
    {
        BaseUri = baseUri;
        OrganizationId = organizationId;
        ApiKey = apiKey;
        ApiSecret = apiSecret;
    }

    public Uri BaseUri { get; set; }
    public string OrganizationId { get; set; }
    public string ApiKey { get; set; }
    public string ApiSecret { get; set; }
}
