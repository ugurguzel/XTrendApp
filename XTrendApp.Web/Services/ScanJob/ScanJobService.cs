using System.Data;
using XTrendApp.Web.Connectors.Amazon;
using XTrendApp.Web.Data;
using XTrendApp.Web.Models.ScanJob;
using XTrendApp.Web.Repositories.ScanJob;
using XTrendApp.Web.Repositories.ScanExecution;

namespace XTrendApp.Web.Services.ScanJob
{
    public class ScanJobService
    {
        
        private readonly DapperContext _context;
        private readonly ScanJobRepository _scanJobRepository;
        private readonly IScanExecutionRepository _scanExecutionRepository;
        private readonly AmazonConnector _amazonConnector;

        
        public ScanJobService(
    DapperContext context,
    ScanJobRepository scanJobRepository,
    IScanExecutionRepository scanExecutionRepository,
    AmazonConnector amazonConnector)
        {
            _context = context;
            _scanJobRepository = scanJobRepository;
            _scanExecutionRepository = scanExecutionRepository;
            _amazonConnector = amazonConnector;
        }

        public List<ScanJobModel> GetAll()
        {
            return _scanJobRepository.GetAll();
        }

        public async Task RunAsync(string code)
        {
            var scanJob =
    _scanJobRepository.GetAll()
        .FirstOrDefault(x =>
            x.Code.Equals(
                code,
                StringComparison.OrdinalIgnoreCase));

            if (scanJob == null)
            {
                throw new Exception(
                    $"Unknown scan job: {code}");
            }

            var scanJobId = scanJob.Id;

            AmazonMarket? market = code switch
            {
                "AMAZON_US" => AmazonMarket.US,
                "AMAZON_UK" => AmazonMarket.UK,
                "WAYFAIR" => null,
                _ => null
            };

            if (market == null)
            {
                throw new NotImplementedException(
                    $"Scan source is not implemented: {code}");
            }

            var productLimit = scanJob.ProductLimit;
        
            using var connection = _context.CreateConnection();

            connection.Open();

            using var transaction = connection.BeginTransaction();


            long executionId;

            try
            {
                executionId = await _scanExecutionRepository.StartAsync(
    connection,
    transaction,
    scanJobId,
    code,
    productLimit);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }


            ScanExecutionResult scanResult;

            try
            {
                scanResult = await _amazonConnector.RunAsync(
                    market.Value,
                    executionId,
                    productLimit);
            }
            catch
            {
                using var failConnection = _context.CreateConnection();

                failConnection.Open();

                using var failTransaction =
                    failConnection.BeginTransaction();

                try
                {
                    await _scanExecutionRepository.FailAsync(
                        failConnection,
                        failTransaction,
                        executionId);

                    failTransaction.Commit();
                }
                catch
                {
                    failTransaction.Rollback();
                }

                throw;
            }


            using var completeConnection = _context.CreateConnection();

            completeConnection.Open();

            using var completeTransaction =
                completeConnection.BeginTransaction();

            try
            {
                await _scanExecutionRepository.CompleteAsync(
    completeConnection,
    completeTransaction,
    executionId,
    scanResult.TotalProducts,
    scanResult.InsertedProducts,
    scanResult.UpdatedProducts,
    scanResult.FailedProducts);

                completeTransaction.Commit();
            }
            catch
            {
                completeTransaction.Rollback();
                throw;
            }
        }

        public async Task<bool> UpdateProductLimitAsync(
    int id,
    int productLimit)
        {
            if (productLimit < 1 || productLimit > 100)
                return false;

            return await _scanJobRepository.UpdateProductLimitAsync(
                id,
                productLimit);
        }
    }
}