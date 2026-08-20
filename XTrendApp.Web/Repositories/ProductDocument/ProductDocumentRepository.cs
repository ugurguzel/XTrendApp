using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ProductDocument;

public class ProductDocumentRepository : IProductDocumentRepository
{
    private const string DeleteSql = """
        DELETE
        FROM ProductDocument
        WHERE ProductId=@ProductId;
        """;

    private const string InsertSql = """
        INSERT INTO ProductDocument
        (
            ProductId,
            DocumentType,
            Title,
            DocumentUrl
        )
        VALUES
        (
            @ProductId,
            @DocumentType,
            @Title,
            @DocumentUrl
        );
        """;

    public async Task DeleteByProductIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productId)
    {
        await connection.ExecuteAsync(
            DeleteSql,
            new { ProductId = productId },
            transaction);
    }

    public async Task InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductDocumentEntity document)
    {
        await connection.ExecuteAsync(
            InsertSql,
            document,
            transaction);
    }
}