using System.Data;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ProductImage;

public interface IProductImageRepository
{
    Task DeleteByProductVariationIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productVariationId);

    Task InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductImageEntity image);

    Task<ProductImageEntity?> GetByProductVariationIdAndImageUrlAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productVariationId,
        string imageUrl);

    Task UpdateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductImageEntity image);
}