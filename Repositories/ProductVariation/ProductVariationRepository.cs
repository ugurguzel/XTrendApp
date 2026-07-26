using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ProductVariation;

public class ProductVariationRepository : IProductVariationRepository
{
    #region SQL

    private const string SelectBySourceVariationIdSql = """
        SELECT TOP (1) *
        FROM ProductVariation
        WHERE ProductId = @ProductId
          AND SourceVariationId = @SourceVariationId;
        """;

    private const string InsertSql = """
        INSERT INTO ProductVariation
        (
            ProductId,
            SourceVariationId,
            Name,
            SKU,
            UPC,
            EAN,
            GTIN,
            ProductUrl,
            DisplayOrder,
            IsDefault,
            IsActive
        )
        VALUES
        (
            @ProductId,
            @SourceVariationId,
            @Name,
            @SKU,
            @UPC,
            @EAN,
            @GTIN,
            @ProductUrl,
            @DisplayOrder,
            @IsDefault,
            @IsActive
        );

        SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
        """;

    private const string UpdateSql = """
        UPDATE ProductVariation
        SET
            Name = @Name,
            SKU = @SKU,
            UPC = @UPC,
            EAN = @EAN,
            GTIN = @GTIN,
            ProductUrl = @ProductUrl,
            DisplayOrder = @DisplayOrder,
            IsDefault = @IsDefault,
            IsActive = @IsActive,
            UpdatedAt = SYSDATETIME()
        WHERE Id = @Id;
        """;

    #endregion

    public async Task<ProductVariationEntity?> GetBySourceVariationIdAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        long productId,
        string sourceVariationId)
    {
        return await connection.QueryFirstOrDefaultAsync<ProductVariationEntity>(
            SelectBySourceVariationIdSql,
            new
            {
                ProductId = productId,
                SourceVariationId = sourceVariationId
            },
            transaction);
    }

    public async Task<long> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductVariationEntity variation)
    {
        return await connection.ExecuteScalarAsync<long>(
            InsertSql,
            variation,
            transaction);
    }

    public async Task UpdateAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductVariationEntity variation)
    {
        await connection.ExecuteAsync(
            UpdateSql,
            variation,
            transaction);
    }
}