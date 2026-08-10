using System.Data;
using XTrendApp.Web.Models.Entities;
using XTrendApp.Web.Models.Product;

namespace XTrendApp.Web.Repositories.Product;

public interface IProductRepository
{
    Task<ProductEntity?> GetBySourceProductIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long sourceId,
        string sourceProductId);

    Task<long> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductEntity product);

    Task UpdateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductEntity product);

    Task<List<ProductEntity>> GetBySourceIdAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long sourceId);

    Task SetActiveAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productId,
        bool isActive);

    Task<IEnumerable<ProductListViewModel>> GetListAsync();

}