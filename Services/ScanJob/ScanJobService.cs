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
            long scanJobId;

            AmazonMarket? market = null;

            switch (code)
            {
                case "AMAZON_US":

                    scanJobId = 1;
                    market = AmazonMarket.US;

                    break;

                case "AMAZON_UK":

                    scanJobId = 2;
                    market = AmazonMarket.UK;

                    break;

                case "WAYFAIR":

                    throw new NotImplementedException();

                default:

                    throw new Exception("Unknown scan job.");
            }


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
                    code);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }


            try
            {
                await _amazonConnector.RunAsync(
                    market.Value,
                    executionId);
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
                    0,
                    0,
                    0,
                    0);

                completeTransaction.Commit();
            }
            catch
            {
                completeTransaction.Rollback();
                throw;
            }
        }
    }
}