using System.Data;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ScanExecution;

public interface IScanExecutionRepository
{
    Task<long> StartAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long scanJobId,
        string jobType);

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
        long executionId);

    Task<ScanExecutionEntity?> GetByIdAsync(
        IDbConnection connection,
        long executionId);
}