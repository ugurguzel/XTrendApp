using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.Snapshot;

public class ProductSnapshotRepository : IProductSnapshotRepository
{
    private const string InsertSql = """
INSERT INTO ProductSnapshot
(
    ProductVariationId,

    Price,
    ListPrice,
    SalePrice,
    ShippingPrice,

    CurrencyCode,

    Rating,
    ReviewCount,

    StockQuantity,
    IsInStock,

    IsPrime,
    HasBuyBox,

    SellerName,

    BoughtLastMonthText,
    BoughtLastMonthCount,

    CouponText,
    DeliveryText,

    CapturedAt
)
VALUES
(
    @ProductVariationId,

    @Price,
    @ListPrice,
    @SalePrice,
    @ShippingPrice,

    @CurrencyCode,

    @Rating,
    @ReviewCount,

    @StockQuantity,
    @IsInStock,

    @IsPrime,
    @HasBuyBox,

    @SellerName,

    @BoughtLastMonthText,
    @BoughtLastMonthCount,

    @CouponText,
    @DeliveryText,

    @CapturedAt
);

SELECT CAST(SCOPE_IDENTITY() AS BIGINT);
""";

    public async Task<long> InsertAsync(
        IDbConnection connection,
        IDbTransaction transaction,
        ProductSnapshotEntity snapshot)
    {
        return await connection.ExecuteScalarAsync<long>(
            InsertSql,
            snapshot,
            transaction);
    }
}