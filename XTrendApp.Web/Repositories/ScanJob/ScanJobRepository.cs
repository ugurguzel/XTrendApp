using Dapper;
using XTrendApp.Web.Data;
using XTrendApp.Web.Models.ScanJob;

namespace XTrendApp.Web.Repositories.ScanJob
{
    public class ScanJobRepository
    {
        private readonly DapperContext _context;

        public ScanJobRepository(DapperContext context)
        {
            _context = context;
        }

        public List<ScanJobModel> GetAll()
        {
            using var connection = _context.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    JobType AS Code,
                    JobType AS Name,
                    CASE
                        WHEN JobType LIKE 'AMAZON_%'
                            THEN 'Amazon'
                        WHEN JobType = 'WAYFAIR'
                            THEN 'Wayfair'
                        ELSE JobType
                    END AS Source,
                    CAST(1 AS bit) AS IsEnabled,
                    ProductLimit,
                    ProductsPerRun,
                    CurrentPage
                FROM ScanJob
                ORDER BY Id;
                """;

            return connection
                .Query<ScanJobModel>(sql)
                .ToList();
        }

        public async Task<ScanJobModel?> GetByIdAsync(
            long id)
        {
            using var connection = _context.CreateConnection();

            const string sql = """
                SELECT
                    Id,
                    JobType AS Code,
                    JobType AS Name,
                    CASE
                        WHEN JobType LIKE 'AMAZON_%'
                            THEN 'Amazon'
                        WHEN JobType = 'WAYFAIR'
                            THEN 'Wayfair'
                        ELSE JobType
                    END AS Source,
                    CAST(1 AS bit) AS IsEnabled,
                    ProductLimit,
                    ProductsPerRun,
                    CurrentPage
                FROM ScanJob
                WHERE Id = @Id;
                """;

            return await connection.QueryFirstOrDefaultAsync<ScanJobModel>(
                sql,
                new
                {
                    Id = id
                });
        }

        public async Task<bool> UpdateProductLimitAsync(
            int id,
            int productLimit)
        {
            if (productLimit < 1 || productLimit > 100)
                return false;

            using var connection = _context.CreateConnection();

            const string sql = """
                UPDATE ScanJob
                SET ProductLimit = @ProductLimit
                WHERE Id = @Id;
                """;

            var affectedRows = await connection.ExecuteAsync(
                sql,
                new
                {
                    Id = id,
                    ProductLimit = productLimit
                });

            return affectedRows > 0;
        }

        public async Task<bool> UpdateProductsPerRunAsync(
            int id,
            int productsPerRun)
        {
            if (productsPerRun < 1 || productsPerRun > 24)
                return false;

            using var connection = _context.CreateConnection();

            const string sql = """
                UPDATE ScanJob
                SET ProductsPerRun = @ProductsPerRun
                WHERE Id = @Id;
                """;

            var affectedRows = await connection.ExecuteAsync(
                sql,
                new
                {
                    Id = id,
                    ProductsPerRun = productsPerRun
                });

            return affectedRows > 0;
        }

        public async Task<bool> UpdateCurrentPageAsync(
            int id,
            int currentPage)
        {
            if (currentPage < 1)
                return false;

            using var connection = _context.CreateConnection();

            const string sql = """
                UPDATE ScanJob
                SET CurrentPage = @CurrentPage
                WHERE Id = @Id;
                """;

            var affectedRows = await connection.ExecuteAsync(
                sql,
                new
                {
                    Id = id,
                    CurrentPage = currentPage
                });

            return affectedRows > 0;
        }
    }
}