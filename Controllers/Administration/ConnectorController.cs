using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XTrendApp.Web.Connectors.Amazon;
using XTrendApp.Web.Connectors.Wayfair;

namespace XTrendApp.Web.Controllers.Administration
{
    [Authorize]
    public class ConnectorController : Controller
    {
        private readonly AmazonSession _amazonSession;
        private readonly AmazonConnector _amazonConnector;
        private readonly WayfairSession _wayfairSession;

        public ConnectorController(
    AmazonSession amazonSession,
    AmazonConnector amazonConnector,
    WayfairSession wayfairSession)
        {
            _amazonSession = amazonSession;
            _amazonConnector = amazonConnector;
            _wayfairSession = wayfairSession;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> TestAmazonUs()
        {
            await _amazonConnector.RunAsync(AmazonMarket.US);

            return Content("Amazon US OK");
        }

        public async Task<IActionResult> ConfigureAmazonUsSession()
        {
            await _amazonSession.ConfigureUsSessionAsync();

            return Content("Amazon US session configured.");
        }

        public async Task<IActionResult> SaveAmazonUsSession()
        {
            if (SessionHolder.Context == null)
                return Content("No active session.");

            await SessionHolder.Context.StorageStateAsync(new()
            {
                Path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "App_Data",
                    "Sessions",
                    "amazon-us.json")
            });

            SessionHolder.Context = null;

            return Content("Amazon US session saved.");
        }


        public async Task<IActionResult> TestAmazonUk()
        {
            await _amazonConnector.RunAsync(AmazonMarket.UK);

            return Content("Amazon UK OK");
        }

        public async Task<IActionResult> ConfigureAmazonUkSession()
        {
            await _amazonSession.ConfigureUkSessionAsync();

            return Content("Amazon UK session configured.");
        }

        public async Task<IActionResult> SaveAmazonUkSession()
        {
            if (SessionHolder.Context == null)
                return Content("No active session.");

            await SessionHolder.Context.StorageStateAsync(new()
            {
                Path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "App_Data",
                    "Sessions",
                    "amazon-uk.json")
            });

            SessionHolder.Context = null;

            return Content("Amazon UK session saved.");
        }

        public async Task<IActionResult> ConfigureWayfair()
        {
            await _wayfairSession.ConfigureSessionAsync();

            return Content("Wayfair session completed.");
        }

    }
}