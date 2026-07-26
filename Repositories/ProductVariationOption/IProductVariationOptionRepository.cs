using System.Data;
using XTrendApp.Web.Models.Entities;

public interface IProductVariationOptionRepository
{
    Task<ProductVariationOptionEntity?> GetByVariationIdAndOptionNameAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productVariationId,
        string optionName);

    Task<long> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductVariationOptionEntity option);

    
}