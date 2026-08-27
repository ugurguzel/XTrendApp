namespace XTrendApp.Web.Models.Entities;

public class ScanExecutionEntity : BaseEntity
{
    public long ScanJobId { get; set; }

    public string JobType { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public int ProductLimit { get; set; }

    public int TotalProducts { get; set; }

    public int InsertedProducts { get; set; }

    public int UpdatedProducts { get; set; }

    public int FailedProducts { get; set; }
}