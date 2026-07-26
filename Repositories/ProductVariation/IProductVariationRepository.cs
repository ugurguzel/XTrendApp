using System.Data;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ProductVariation;

public interface IProductVariationRepository
{
    Task<ProductVariationEntity?> GetBySourceVariationIdAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productId,
    string sourceVariationId);

    Task<long> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductVariationEntity variation);

    Task UpdateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductVariationEntity variation);
}