using System.Data;
using Dapper;

namespace XTrendApp.Web.Repositories.Source;

public class SourceRepository : ISourceRepository
{
    private const string Sql = """
        SELECT TOP (1) Id
        FROM Source
        WHERE Name = @Name;
        """;

    public async Task<long?> GetIdByNameAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string name)
    {
        return await connection.QueryFirstOrDefaultAsync<long?>(
            Sql,
            new { Name = name },
            transaction);
    }
}