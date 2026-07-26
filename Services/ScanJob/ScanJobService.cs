using XTrendApp.Web.Connectors.Amazon;
using XTrendApp.Web.Models.ScanJob;
using XTrendApp.Web.Repositories.ScanJob;

namespace XTrendApp.Web.Services.ScanJob
{
    public class ScanJobService
    {
        private readonly ScanJobRepository _scanJobRepository;
        private readonly AmazonConnector _amazonConnector;

        public ScanJobService(
            ScanJobRepository scanJobRepository,
            AmazonConnector amazonConnector)
        {
            _scanJobRepository = scanJobRepository;
            _amazonConnector = amazonConnector;
        }

        public List<ScanJobModel> GetAll()
        {
            return _scanJobRepository.GetAll();
        }

        public async Task RunAsync(string code)
        {
            switch (code)
            {
                case "AMAZON_US":

                    await _amazonConnector.RunAsync(AmazonMarket.US);

                    break;

                case "AMAZON_UK":

                    await _amazonConnector.RunAsync(AmazonMarket.UK);

                    break;

                case "WAYFAIR":

                    throw new NotImplementedException();

                default:

                    throw new Exception("Unknown scan job.");
            }
        }
    }
}