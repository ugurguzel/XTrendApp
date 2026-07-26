using System.Data;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.Attribute;

public interface IProductAttributeRepository
{
    Task DeleteByProductIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productId);

    Task InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductAttributeEntity attribute);
}