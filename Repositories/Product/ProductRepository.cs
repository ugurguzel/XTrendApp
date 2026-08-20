using Dapper;
using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore;
using System.Data;
using XTrendApp.Web.Data;
using XTrendApp.Web.Models.Entities;
using XTrendApp.Web.Models.Product;

namespace XTrendApp.Web.Repositories.Product;

public class ProductRepository : IProductRepository
{

    private readonly DapperContext _context;


    public ProductRepository(DapperContext context)
    {
        _context = context;
    }


    #region SQL

    private const string SelectBySourceProductIdSql = """
SELECT TOP (1) *
FROM Product
WHERE SourceId = @SourceId
AND SourceProductId = @SourceProductId;
""";

    private const string InsertSql = """
INSERT INTO Product
(
    SourceId,
    BrandId,
    CollectionId,
    CategoryId,
    SourceProductId,
    Name,
    Description,
    ProductUrl,
    IsActive
)
VALUES
(
    @SourceId,
    @BrandId,
    @CollectionId,
    @CategoryId,
    @SourceProductId,
    @Name,
    @Description,
    @ProductUrl,
    @IsActive
);

SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
""";

    private const string UpdateSql = """
UPDATE Product
SET

BrandId=@BrandId,

CollectionId=@CollectionId,

CategoryId=@CategoryId,

Name=@Name,

Description=@Description,
ProductUrl=@ProductUrl,


IsActive=@IsActive,

UpdatedAt=SYSDATETIME()

WHERE Id=@Id;
""";

    private const string SelectBySourceIdSql = """
SELECT *
FROM Product
WHERE SourceId = @SourceId;
""";

    private const string SetActiveSql = """
UPDATE Product
SET
    IsActive = @IsActive,
    UpdatedAt = SYSDATETIME()
WHERE Id = @ProductId
  AND IsActive <> @IsActive;
""";

    private const string GetListSql = """
SELECT
    p.Id,
    p.Name,

    b.Name AS Brand,

    s.Name AS Source,

    img.ImageUrl,

    variationCount.VariationCount,

    snap.Rating,

    snap.ReviewCount,

    snap.CapturedAt AS LastCapturedAt

FROM Product p

INNER JOIN Brand b
    ON b.Id = p.BrandId

INNER JOIN Source s
    ON s.Id = p.SourceId


-- ACTIVE VARIATION COUNT
OUTER APPLY
(
    SELECT
        COUNT(*) AS VariationCount

    FROM ProductVariation pv

    WHERE pv.ProductId = p.Id
      AND pv.IsActive = 1

) variationCount


-- LATEST PRODUCT SNAPSHOT
OUTER APPLY
(
    SELECT TOP 1

        ps.Rating,

        ps.ReviewCount,

        ps.CapturedAt

    FROM ProductVariation pvLatest

    INNER JOIN ProductSnapshot ps
        ON ps.ProductVariationId = pvLatest.Id

    WHERE pvLatest.ProductId = p.Id

    ORDER BY
        ps.CapturedAt DESC,
        ps.Id DESC

) snap


-- PRIMARY PRODUCT IMAGE
OUTER APPLY
(
    SELECT TOP 1

        pi.ImageUrl

    FROM ProductImage pi

    INNER JOIN ProductVariation pvImage
        ON pvImage.Id = pi.ProductVariationId

    WHERE pvImage.ProductId = p.Id
      AND pvImage.IsActive = 1
      AND pi.IsPrimary = 1

    ORDER BY
        pi.SortOrder,
        pi.Id

) img


ORDER BY
    p.Id DESC;
""";

    private const string GetDetailSql = """
SELECT
    p.Id,
    p.Name,
    b.Name AS Brand,
    s.Name AS Source,
    p.ProductUrl,

    snap.Rating,
    snap.ReviewCount,
    variationCount.VariationCount,
    snap.CapturedAt AS LastCapturedAt

FROM Product p

INNER JOIN Brand b
    ON b.Id = p.BrandId

INNER JOIN Source s
    ON s.Id = p.SourceId

OUTER APPLY
(
    SELECT
        COUNT(*) AS VariationCount

    FROM ProductVariation pv

    WHERE pv.ProductId = p.Id
      AND pv.IsActive = 1

) variationCount

OUTER APPLY
(
    SELECT TOP 1
        ps.Rating,
        ps.ReviewCount,
        ps.CapturedAt

    FROM ProductVariation pvLatest

    INNER JOIN ProductSnapshot ps
        ON ps.ProductVariationId = pvLatest.Id

    WHERE pvLatest.ProductId = p.Id

    ORDER BY
        ps.CapturedAt DESC,
        ps.Id DESC

) snap

WHERE p.Id = @ProductId;
""";

    private const string GetAttributesSql = """
SELECT
    AttributeGroup AS [Group],
    AttributeName AS [Name],
    AttributeValue AS [Value]
FROM ProductAttribute
WHERE ProductId = @ProductId
ORDER BY
    DisplayOrder,
    Id;
""";

    private const string GetVariationsSql = """
SELECT
    pv.Id,

    pv.SourceVariationId AS ASIN,

    pv.Name,

    MAX(CASE
        WHEN LOWER(pvo.OptionName) = 'color'
        THEN pvo.OptionValue
    END) AS Color,

    MAX(CASE
        WHEN LOWER(pvo.OptionName) = 'size'
        THEN pvo.OptionValue
    END) AS Size,

    snap.Price,

    snap.CurrencyCode,

    pv.IsActive,

    pv.ProductUrl

FROM ProductVariation pv

LEFT JOIN ProductVariationOption pvo
    ON pvo.ProductVariationId = pv.Id

OUTER APPLY
(
    SELECT TOP 1
        ps.Price,
        ps.CurrencyCode

    FROM ProductSnapshot ps

    WHERE ps.ProductVariationId = pv.Id

    ORDER BY
        ps.CapturedAt DESC,
        ps.Id DESC

) snap

WHERE pv.ProductId = @ProductId

GROUP BY
    pv.Id,
    pv.SourceVariationId,
    pv.Name,
    snap.Price,
    snap.CurrencyCode,
    pv.IsActive,
    pv.ProductUrl,
    pv.DisplayOrder

ORDER BY
    pv.DisplayOrder,
    pv.Id;
""";


    private const string GetReviewHistorySql = """
SELECT
    se.StartedAt AS CapturedAt,
    MAX(ps.ReviewCount) AS ReviewCount

FROM ProductVariation pv

INNER JOIN ProductSnapshot ps
    ON ps.ProductVariationId = pv.Id

INNER JOIN ScanExecution se
    ON se.Id = ps.ScanExecutionId

WHERE pv.ProductId = @ProductId
  AND ps.ReviewCount IS NOT NULL
  AND ps.ScanExecutionId IS NOT NULL

GROUP BY
    se.Id,
    se.StartedAt

ORDER BY
    se.StartedAt;
""";
    #endregion

    public async Task<ProductEntity?> GetBySourceProductIdAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long sourceId,
    string sourceProductId)
    {
        return await connection.QueryFirstOrDefaultAsync<ProductEntity>(
            SelectBySourceProductIdSql,
            new
            {
                SourceId = sourceId,
                SourceProductId = sourceProductId
            },
            transaction);
    }

    public async Task<long> InsertAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    ProductEntity product)
    {
        return await connection.ExecuteScalarAsync<long>(
            InsertSql,
            product,
            transaction);
    }

    public async Task UpdateAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    ProductEntity product)
    {
        await connection.ExecuteAsync(
            UpdateSql,
            product,
            transaction);
    }

    public async Task<List<ProductEntity>> GetBySourceIdAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long sourceId)
    {
        var result = await connection.QueryAsync<ProductEntity>(
            SelectBySourceIdSql,
            new
            {
                SourceId = sourceId
            },
            transaction);

        return result.ToList();
    }

    public async Task SetActiveAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productId,
        bool isActive)
    {
        await connection.ExecuteAsync(
            SetActiveSql,
            new
            {
                ProductId = productId,
                IsActive = isActive
            },
            transaction);
    }

    public async Task<IEnumerable<ProductListViewModel>> GetListAsync()
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryAsync<ProductListViewModel>(
            GetListSql);
    }

    public async Task<ProductDetailViewModel?> GetDetailAsync(
    long productId)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ProductDetailViewModel>(
            GetDetailSql,
            new
            {
                ProductId = productId
            });
    }

    public async Task<List<ProductAttributeViewModel>> GetAttributesAsync(
    long productId)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<ProductAttributeViewModel>(
            GetAttributesSql,
            new
            {
                ProductId = productId
            });

        return result.ToList();
    }

    public async Task<List<ProductVariationViewModel>> GetVariationsAsync(
    long productId)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<ProductVariationViewModel>(
            GetVariationsSql,
            new
            {
                ProductId = productId
            });

        return result.ToList();
    }

    public async Task<List<ProductReviewHistoryViewModel>> GetReviewHistoryAsync(
    long productId)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<ProductReviewHistoryViewModel>(
            GetReviewHistorySql,
            new
            {
                ProductId = productId
            });

        return result.ToList();
    }

}