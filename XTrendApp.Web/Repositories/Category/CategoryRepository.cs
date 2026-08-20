using System.Data;
using Dapper;

namespace XTrendApp.Web.Repositories.Category;

public class CategoryRepository : ICategoryRepository
{
    private const string SelectSql = """
        SELECT TOP (1) Id
        FROM Category
        WHERE Name = @Name;
        """;

    private const string InsertSql = """
        INSERT INTO Category
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

    public async Task<long> GetOrCreateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string name)
    {
        name = name.Trim();

        var id = await connection.QueryFirstOrDefaultAsync<long?>(
            SelectSql,
            new { Name = name },
            transaction);

        if (id.HasValue)
            return id.Value;

        return await connection.ExecuteScalarAsync<long>(
            InsertSql,
            new { Name = name },
            transaction);
    }
}