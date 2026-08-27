using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ScanExecution;

public class ScanExecutionRepository : IScanExecutionRepository
{
    public async Task<long> StartAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long scanJobId,
    string jobType,
    int productLimit)
    {
        const string sql = """
        INSERT INTO ScanExecution
        (
            ScanJobId,
            JobType,
            ProductLimit,
            Status,
            StartedAt
        )
        OUTPUT INSERTED.Id
        VALUES
        (
            @ScanJobId,
            @JobType,
            @ProductLimit,
            'RUNNING',
            SYSUTCDATETIME()
        );
        """;

        return await connection.ExecuteScalarAsync<long>(
            sql,
            new
            {
                ScanJobId = scanJobId,
                JobType = jobType,
                ProductLimit = productLimit
            },
            transaction);
    }


    public async Task CompleteAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long executionId,
        int totalProducts,
        int insertedProducts,
        int updatedProducts,
        int failedProducts)
    {
        const string sql = """
            UPDATE ScanExecution
            SET
                Status = 'COMPLETED',
                FinishedAt = SYSUTCDATETIME(),
                TotalProducts = @TotalProducts,
                InsertedProducts = @InsertedProducts,
                UpdatedProducts = @UpdatedProducts,
                FailedProducts = @FailedProducts
            WHERE Id = @Id;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                Id = executionId,
                TotalProducts = totalProducts,
                InsertedProducts = insertedProducts,
                UpdatedProducts = updatedProducts,
                FailedProducts = failedProducts
            },
            transaction);
    }


    public async Task FailAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long executionId)
    {
        const string sql = """
            UPDATE ScanExecution
            SET
                Status = 'FAILED',
                FinishedAt = SYSUTCDATETIME()
            WHERE Id = @Id;
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                Id = executionId
            },
            transaction);
    }


    public async Task<ScanExecutionEntity?> GetByIdAsync(
        IDbConnection connection,
        long executionId)
    {
        const string sql = """
            SELECT
                Id,
                ScanJobId,
                JobType,
                ProductLimit,
                Status,
                StartedAt,
                FinishedAt,
                TotalProducts,
                InsertedProducts,
                UpdatedProducts,
                FailedProducts,
                CreatedAt
            FROM ScanExecution
            WHERE Id = @Id;
            """;

        return await connection.QueryFirstOrDefaultAsync<ScanExecutionEntity>(
            sql,
            new
            {
                Id = executionId
            });
    }
}