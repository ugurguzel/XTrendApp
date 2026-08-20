using System.Data;

namespace XTrendApp.Web.Repositories.Category;

public interface ICategoryRepository
{
    Task<long> GetOrCreateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string name);
}