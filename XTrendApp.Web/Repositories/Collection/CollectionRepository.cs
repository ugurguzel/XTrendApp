using System.Data;
using Dapper;

namespace XTrendApp.Web.Repositories.Collection;

public class CollectionRepository : ICollectionRepository
{
    public async Task<long?> GetOrCreateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long brandId,
        string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        name = name.Trim();

        const string selectSql = """
            SELECT TOP (1) Id
            FROM Collection
            WHERE BrandId=@BrandId
              AND Name=@Name;
            """;

        var id = await connection.QueryFirstOrDefaultAsync<long?>(
            selectSql,
            new
            {
                BrandId = brandId,
                Name = name
            },
            transaction);

        if (id.HasValue)
            return id;

        const string insertSql = """
            INSERT INTO Collection
            (
                BrandId,
                Name,
                IsActive
            )
            VALUES
            (
                @BrandId,
                @Name,
                1
            );

            SELECT CAST(SCOPE_IDENTITY() AS bigint);
            """;

        return await connection.ExecuteScalarAsync<long>(
            insertSql,
            new
            {
                BrandId = brandId,
                Name = name
            },
            transaction);
    }
}