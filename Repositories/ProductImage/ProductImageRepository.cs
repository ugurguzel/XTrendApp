using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ProductImage;

public class ProductImageRepository : IProductImageRepository
{
    private const string DeleteSql = """
        DELETE
        FROM ProductImage
        WHERE ProductId=@ProductId;
        """;

    private const string InsertSql = """
INSERT INTO ProductImage
(
    ProductId,
    ImageUrl,
    ImageType,
    DisplayOrder,
    IsPrimary
)
VALUES
(
    @ProductId,
    @ImageUrl,
    @ImageType,
    @DisplayOrder,
    @IsPrimary
);
""";

    private const string GetSql = """
SELECT TOP 1 *
FROM ProductImage
WHERE ProductId = @ProductId
AND ImageUrl = @ImageUrl;
""";

    private const string UpdateSql = """
UPDATE ProductImage
SET
    ImageType = @ImageType,
    DisplayOrder = @DisplayOrder,
    IsPrimary = @IsPrimary
WHERE Id = @Id;
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
        ProductImageEntity image)
    {
        await connection.ExecuteAsync(
            InsertSql,
            image,
            transaction);
    }

    public async Task<ProductImageEntity?> GetByProductIdAndImageUrlAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productId,
    string imageUrl)
    {
        return await connection.QueryFirstOrDefaultAsync<ProductImageEntity>(
            GetSql,
            new
            {
                ProductId = productId,
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