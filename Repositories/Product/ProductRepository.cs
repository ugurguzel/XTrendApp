using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.Product;

public class ProductRepository : IProductRepository
{
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

IsActive=@IsActive,

UpdatedAt=SYSDATETIME()

WHERE Id=@Id;
""";

    //private const string UpdateLastScanSql = """
    //    UPDATE Product
    //    SET
    //        LastScanAt = @LastScanAt
    //    WHERE
    //        Id = @ProductId;
    //    """;

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

}