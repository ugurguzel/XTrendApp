using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.ProductVariationOption;

public class ProductVariationOptionRepository : IProductVariationOptionRepository
{
    private const string SelectSql = """
SELECT TOP (1) *
FROM ProductVariationOption
WHERE ProductVariationId = @ProductVariationId
AND OptionName = @OptionName;
""";

    private const string InsertSql = """
INSERT INTO ProductVariationOption
(
    ProductVariationId,
    OptionName,
    OptionValue,
    DisplayOrder
)
VALUES
(
    @ProductVariationId,
    @OptionName,
    @OptionValue,
    @DisplayOrder
);

SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
""";

    
    public async Task<long> InsertAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    ProductVariationOptionEntity option)
    {
        return await connection.ExecuteScalarAsync<long>(
            InsertSql,
            option,
            transaction);
    }
    public async Task<ProductVariationOptionEntity?> GetByVariationIdAndOptionNameAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productVariationId,
    string optionName)
    {
        return await connection.QueryFirstOrDefaultAsync<ProductVariationOptionEntity>(
            SelectSql,
            new
            {
                ProductVariationId = productVariationId,
                OptionName = optionName
            },
            transaction);
    }

    

}