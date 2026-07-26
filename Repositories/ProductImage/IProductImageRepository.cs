using System.Data;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ProductImage;

public interface IProductImageRepository
{
    Task DeleteByProductIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productId);

    Task InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductImageEntity image);

    Task<ProductImageEntity?> GetByProductIdAndImageUrlAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productId,
    string imageUrl);

    Task UpdateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductImageEntity image);
}