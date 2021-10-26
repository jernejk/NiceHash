using System.Text.Json.Serialization;

namespace NiceHash.Core.Models;

public class RigsResponse
{
    public MinerStatuses MinerStatuses { get; set; }
    public int TotalRigs { get; set; }
    public double TotalProfitability { get; set; }
    public string GroupPowerMode { get; set; }
    public int TotalDevices { get; set; }
    public DevicesStatuses DevicesStatuses { get; set; }
    public double UnpaidAmount { get; set; }
    public string Path { get; set; }
    public string BtcAddress { get; set; }
    public string NextPayoutTimestamp { get; set; }
    public string LastPayoutTimestamp { get; set; }

    public List<MiningRig> MiningRigs { get; set; } = new List<MiningRig>();
}

public class MinerStatuses
{
    [JsonPropertyName("MINING")]
    public int Mining { get; set; }
}

public class DevicesStatuses
{
    [JsonPropertyName("MINING")]
    public int Mining { get; set; }

    [JsonPropertyName("DISABLED")]
    public int Disabled { get; set; }
}
