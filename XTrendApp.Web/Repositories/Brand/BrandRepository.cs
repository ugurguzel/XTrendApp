using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.Brand;

public class BrandRepository : IBrandRepository
{
    public async Task<long> GetOrCreateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Brand name cannot be empty.", nameof(name));

        name = name.Trim();

        const string selectSql = """
            SELECT TOP (1) Id
            FROM Brand
            WHERE Name = @Name;
            """;

        var brandId = await connection.QueryFirstOrDefaultAsync<long?>(
            selectSql,
            new { Name = name },
            transaction);

        if (brandId.HasValue)
            return brandId.Value;

        const string insertSql = """
            INSERT INTO Brand
            (
                Name,
                IsActive
            )
            VALUES
            (
                @Name,
                1
            );

            SELECT CAST(SCOPE_IDENTITY() AS bigint);
            """;

        return await connection.ExecuteScalarAsync<long>(
            insertSql,
            new { Name = name },
            transaction);
    }
}