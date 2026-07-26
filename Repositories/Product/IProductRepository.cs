//using System.Data;
//using XTrendApp.Web.Models.Entities;

//namespace XTrendApp.Web.Repositories.Product;

//public interface IProductRepository
//{
//    Task<ProductEntity?> GetByExternalIdAsync(
//        IDbConnection connection,
//        IDbTransaction transaction,
//        long sourceId,
//        string countryCode,
//        string externalProductId);

//    Task<long> InsertAsync(
//        IDbConnection connection,
//        IDbTransaction transaction,
//        ProductEntity product);

//    Task UpdateAsync(
//        IDbConnection connection,
//        IDbTransaction transaction,
//        ProductEntity product);

//}

using System.Data;
using XTrendApp.Web.Models.Entities;

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
}