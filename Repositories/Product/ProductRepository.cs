using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;
using XTrendApp.Web.Models.Entities;
using XTrendApp.Web.Models.Product;
using XTrendApp.Web.Data;

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

}