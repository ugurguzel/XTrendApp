using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ProductImage;

public class ProductImageRepository : IProductImageRepository
{
    private const string DeleteSql = """
        DELETE
        FROM ProductImage
        WHERE ProductVariationId=@ProductVariationId;
        """;

    private const string InsertSql = """
INSERT INTO ProductImage
(
    ProductVariationId,
    ImageUrl,
    SortOrder,
    IsPrimary
)
VALUES
(
    @ProductVariationId,
    @ImageUrl,
    @SortOrder,
    @IsPrimary
);
""";

    private const string GetSql = """
SELECT TOP 1 *
FROM ProductImage
WHERE ProductVariationId = @ProductVariationId;
""";

    private const string UpdateSql = """
UPDATE ProductImage
SET
    SortOrder = @SortOrder,
    IsPrimary = @IsPrimary
WHERE Id = @Id;
""";

    public async Task DeleteByProductVariationIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productVariationId)
    {
        await connection.ExecuteAsync(
            DeleteSql,
            new { ProductVariationId = productVariationId },
            transaction);
    }

    public async Task InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductImageEntity image)
    {
        await connection.ExecuteAsync(
            InsertSql,
            image,
            transaction);
    }

    public async Task<ProductImageEntity?> GetByProductVariationIdAndImageUrlAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productVariationId,
    string imageUrl)
    {
        return await connection.QueryFirstOrDefaultAsync<ProductImageEntity>(
            GetSql,
            new
            {
                ProductVariationId = productVariationId,
                ImageUrl = imageUrl
            },
            transaction);
    }

    public async Task UpdateAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    ProductImageEntity image)
    {
        await connection.ExecuteAsync(
            UpdateSql,
            image,
            transaction);
    }
}