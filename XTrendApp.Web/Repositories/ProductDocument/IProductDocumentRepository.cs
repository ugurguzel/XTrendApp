using System.Data;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ProductDocument;

public interface IProductDocumentRepository
{
    Task DeleteByProductIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productId);

    Task InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductDocumentEntity document);
}