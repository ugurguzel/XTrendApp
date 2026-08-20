using System.Data;

namespace XTrendApp.Web.Repositories.Collection;

public interface ICollectionRepository
{
    Task<long?> GetOrCreateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long brandId,
        string? name);
}