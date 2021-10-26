namespace NiceHash.Core.Models;

public class WorkersResponse : PaginationBaseResponse
{
    public List<WorkerInfo>? Workers { get; set; }
}

public class WorkerInfo
{
    public long StatsTime { get; set; }
    public string? Market { get; set; }
    public AlgorithmInfo? AlgorithmInfo { get; set; }
    public decimal UnpaidAmount { get; set; }
    public double Difficulty { get; set; }
    public long TimeConnected { get; set; }
    public double SpeedAccepted { get; set; }
    public double SpeedRejectedTotal {get;set;}
    public double Profitability { get; set; }
    public string RigName { get; set; } = null!;
}

public class AlgorithmInfo
{
    public string EnumName { get; set; } = null!;
    public string Description { get; set; } = null!;
}
