using System.Data;
using XTrendApp.Web.Models.ScanJob;

namespace XTrendApp.Web.Repositories.ScanJob;

public interface IScanJobExecutionRepository
{
    Task<long> StartAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string jobType,
        long? sourceId,
        string? keyword);

    Task CompleteAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long executionId,
        int totalProducts,
        int insertedProducts,
        int updatedProducts,
        int failedProducts);

    Task FailAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long executionId,
        string? errorMessage);

    Task<ScanJobExecutionEntity?> GetByIdAsync(
        IDbConnection connection,
        long executionId);
}