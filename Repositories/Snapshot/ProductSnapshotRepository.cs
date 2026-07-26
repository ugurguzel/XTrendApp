using System.Data;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.Snapshot;

public interface IProductSnapshotRepository
{
    Task<long> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductSnapshotEntity snapshot);
}