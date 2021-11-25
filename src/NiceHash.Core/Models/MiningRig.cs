namespace NiceHash.Core.Models;

public class MiningRig
{
    public string RigId { get; set; }
    public string Type { get; set; }
    public string Name { get; set; }
    public long StatusTime { get; set; }
    public long JoinTime { get; set; }
    public string MinerStatus { get; set; }
    public string GroupName { get; set; }
    public decimal UnpaidAmount { get; set; }
    public string SoftwareVersions { get; set; }

    public List<MiningDevice> Devices { get; set; } = new List<MiningDevice>();
}

public class MiningDevice
{
    public string Id { get; set; }
    public string Name { get; set; }
    //"deviceType": {
    //  "enumName": "CPU",
    //  "description": "CPU"
    //},
    //"status": {
    //  "enumName": "DISABLED",
    //  "description": "Disabled"
    //},
    public double Temperature { get; set; }
    public double RevolutionsPerMinute { get; set; }
    public double RevolutionsPerMinutePercentage { get; set; }
    public double PowerUsage { get; set; }
    public List<DeviceSpeed> Speeds { get; set; }
    //"intensity": {
    //  "enumName": "LOW",
    //  "description": "Low power mode"
    //},
}

public class DeviceSpeed
{
    public string Algorithm { get; set; }
    public string Title { get; set; }
    public double Speed { get; set; }
    public string DisplaySuffix { get; set; }
}
