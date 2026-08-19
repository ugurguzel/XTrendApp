namespace XTrendApp.Web.Models.ScanJob;

public class ScanExecutionResult
{
    public int TotalProducts { get; set; }

    public int InsertedProducts { get; set; }

    public int UpdatedProducts { get; set; }

    public int FailedProducts { get; set; }

    public int InsertedVariations { get; set; }

    public int UpdatedVariations { get; set; }

    public int SnapshotCount { get; set; }
}