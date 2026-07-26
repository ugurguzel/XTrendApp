using System.Data;

namespace XTrendApp.Web.Repositories.Brand;

public interface IBrandRepository
{
    Task<long> GetOrCreateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        string name);
}