using System.Data;
using Dapper;
using XTrendApp.Web.Models.ScanJob;

namespace XTrendApp.Web.Repositories.ScanJob;

public class ScanJobExecutionRepository : IScanJobExecutionRepository
{
    public async Task<long> StartAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string jobType,
        long? sourceId,
        string? keyword)
    {
        const string sql = """
            INSERT INTO ScanJob
            (
                JobType,
                SourceId,
                Keyword,
                Status,
                StartedAt,
                CreatedAt
            )
            OUTPUT INSERTED.Id
            VALUES
            (
                @JobType,
                @SourceId,
                @Keyword,
                'RUNNING',
                SYSUTCDATETIME(),
                SYSUTCDATETIME()
            );
            """;

        return await connection.ExecuteScalarAsync<long>(
            sql,
            new
            {
                JobType = jobType,
                SourceId = sourceId,
                Keyword = keyword
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
            UPDATE ScanJob
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
        long executionId,
        string? errorMessage)
    {
        const string sql = """
            UPDATE ScanJob
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


    public async Task<ScanJobExecutionEntity?> GetByIdAsync(
        IDbConnection connection,
        long executionId)
    {
        const string sql = """
            SELECT
                Id,
                JobType,
                SourceId,
                Keyword,
                Status,
                StartedAt,
                FinishedAt,
                TotalProducts,
                InsertedProducts,
                UpdatedProducts,
                FailedProducts,
                CreatedAt
            FROM ScanJob
            WHERE Id = @Id;
            """;

        return await connection.QueryFirstOrDefaultAsync<ScanJobExecutionEntity>(
            sql,
            new
            {
                Id = executionId
            });
    }
}