namespace NiceHash.Core.Models;

public class PaginationBaseResponse
{
    public int Size { get; set; }
    public int Page { get; set; }
    public int TotalPageCount { get; set; }
}
