namespace NiceHash.Core.Models;

public class CurrencyResponse
{
    public List<CurrencyInfo> Currencies { get; set; } = new();
}

public class CurrencyInfo
{
    public string Currency { get; set; } = null!;
    public double Available { get; set; }
}
