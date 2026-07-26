using System.Data;

namespace XTrendApp.Web.Repositories.Source;

public interface ISourceRepository
{
    Task<long?> GetIdByNameAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string name);
}