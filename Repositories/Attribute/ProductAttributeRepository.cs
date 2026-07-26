using System.Data;
using Dapper;
using XTrendApp.Web.Models.Entities;

namespace XTrendApp.Web.Repositories.Attribute;

public class ProductAttributeRepository : IProductAttributeRepository
{
    private const string DeleteSql = """
DELETE
FROM ProductAttribute
WHERE ProductId = @ProductId;
""";

    private const string InsertSql = """
INSERT INTO ProductAttribute
(
    ProductId,
    AttributeGroup,
    AttributeName,
    AttributeValue,
    SortOrder
)
VALUES
(
    @ProductId,
    @AttributeGroup,
    @AttributeName,
    @AttributeValue,
    @SortOrder
);
""";

    public async Task DeleteByProductIdAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    long productId)
    {
        await connection.ExecuteAsync(
            DeleteSql,
            new
            {
                ProductId = productId
            },
            transaction);
    }

    public async Task InsertAsync(
    IDbConnection connection,
    IDbTransaction transaction,
    ProductAttributeEntity attribute)
    {
        await connection.ExecuteAsync(
            InsertSql,
            attribute,
            transaction);
    }
}


