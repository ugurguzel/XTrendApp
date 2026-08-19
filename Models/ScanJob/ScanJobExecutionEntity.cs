namespace XTrendApp.Web.Models.ScanJob;

public class ScanJobExecutionEntity
{
    public long Id { get; set; }

    public string JobType { get; set; } = string.Empty;

    public long? SourceId { get; set; }

    public string? Keyword { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; }

    public DateTime? FinishedAt { get; set; }

    public int TotalProducts { get; set; }

    public int InsertedProducts { get; set; }

    public int UpdatedProducts { get; set; }

    public int FailedProducts { get; set; }

    public DateTime CreatedAt { get; set; }
}