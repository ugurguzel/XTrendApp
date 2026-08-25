using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using XTrendApp.Web.Common;
using XTrendApp.Web.Engines.Amazon;
using XTrendApp.Web.Models.Amazon;
using XTrendApp.Web.Models.ScanJob;
using XTrendApp.Web.Parsers.Amazon;
using XTrendApp.Web.Selectors.Amazon;
using XTrendApp.Web.Services.Product;

namespace XTrendApp.Web.Connectors.Amazon
{
    public class AmazonConnector
    {
        private readonly AmazonOptions _options;
        private readonly IWebHostEnvironment _environment;
        private readonly AmazonSearchParser _searchParser;
        private readonly AmazonDetailParser _detailParser;
        private readonly AmazonVariationEngine _variationEngine;
        private readonly ProductImportService _productImportService;

        public AmazonConnector(
            IOptions<AmazonOptions> options,
            IWebHostEnvironment environment,
            AmazonSearchParser searchParser,
            AmazonDetailParser detailParser,
            AmazonVariationEngine variationEngine,
            ProductImportService productImportService)
        {
            _options = options.Value;
            _environment = environment;
            _searchParser = searchParser;
            _detailParser = detailParser;
            _variationEngine = variationEngine;
            _productImportService = productImportService;
        }

        public async Task<ScanExecutionResult> RunAsync(
    AmazonMarket market,
    long scanExecutionId)
        {
            string baseUrl;
            string sessionFile;
            string searchUrl;

            switch (market)
            {
                case AmazonMarket.US:
                    baseUrl = "https://www.amazon.com";
                    sessionFile = "amazon-us.json";
                    searchUrl = "https://www.amazon.com/s?rh=n:684541011";
                    break;

                case AmazonMarket.UK:
                    baseUrl = "https://www.amazon.co.uk";
                    sessionFile = "amazon-uk.json";
                    searchUrl = "https://www.amazon.co.uk/b?node=3028556031";
                    break;

                default:
                    throw new Exception("Unknown market.");
            }

            Logger.Info("");
            Logger.Info("══════════════════════════════════════════════════════════════════════════════");
            Logger.Info($" AMAZON {market}");
            Logger.Info("══════════════════════════════════════════════════════════════════════════════");
            Logger.Info("");

            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false
                });

            var context = await browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    StorageStatePath = Path.Combine(
                        _environment.ContentRootPath,
                        "App_Data",
                        "Sessions",
                        sessionFile)
                });

            var page = await context.NewPageAsync();

            await page.GotoAsync(
    searchUrl,

    new PageGotoOptions
    {
        WaitUntil = WaitUntilState.DOMContentLoaded
    });

            await page.Locator(AmazonSearchSelectors.ProductCard)
    .First

    .WaitForAsync(new LocatorWaitForOptions
    {
        State = WaitForSelectorState.Visible,
        Timeout = 15000
    });

            //--------------------------------------------------
            // SEARCH PARSER
            //--------------------------------------------------

            var products = await _searchParser.ParseAsync(page, baseUrl);

            int index = 1;

            foreach (var product in products)
            {
                product.CurrencyCode = market switch
                {
                    AmazonMarket.US => "USD",
                    AmazonMarket.UK => "GBP",
                    _ => string.Empty
                };

                Logger.Info($"Processing Product {index} of {products.Count}");
                index++;
                Logger.Info($"ASIN : {product.Asin}");
                Logger.Info("");


            }

            Logger.Info($"Products Found : {products.Count}");


            //--------------------------------------------------
            // DETAIL PARSER
            //--------------------------------------------------

            var scanResult = new ScanExecutionResult();

            foreach (var product in products)
            {
                //await page.GotoAsync(
                //    product.ProductUrl,
                //    new PageGotoOptions
                //    {
                //        WaitUntil = WaitUntilState.DOMContentLoaded
                //    });

                await page.GotoAsync(
    $"{baseUrl}/dp/{product.Asin}",
    new PageGotoOptions
    {
        WaitUntil = WaitUntilState.DOMContentLoaded
    });

                AmazonDetailModel detail =
                    await _detailParser.ParseAsync(page, product);

                //--------------------------------------------------
                // VARITAION ENGINE
                //--------------------------------------------------

                var variation = await _variationEngine.ParseAsync(
    page,
    detail,
    baseUrl,
    market);

                //--------------------------------------------------
                // PRODUCT IMPORT
                //--------------------------------------------------

                var countryCode = market switch
                {
                    AmazonMarket.US => "US",
                    AmazonMarket.UK => "UK",
                    _ => throw new InvalidOperationException("Unknown market.")
                };

                var sourceName = market switch
                {
                    AmazonMarket.US => "amazon-us",
                    AmazonMarket.UK => "amazon-uk",
                    _ => throw new InvalidOperationException("Unknown market.")
                };



                var importResult = await _productImportService.ImportAsync(
    detail,
    variation,
    sourceName,
    countryCode,
    scanExecutionId);


                scanResult.TotalProducts++;

                scanResult.InsertedProducts +=
                    importResult.InsertedProducts;

                scanResult.UpdatedProducts +=
                    importResult.UpdatedProducts;

                scanResult.InsertedVariations +=
                    importResult.InsertedVariations;

                scanResult.UpdatedVariations +=
                    importResult.UpdatedVariations;

                scanResult.SnapshotCount +=
                    importResult.SnapshotCount;


            }

            await browser.CloseAsync();

            Logger.Success("");
            Logger.Success("══════════════════════════════════════════════════════════════════════════════");
            Logger.Success($" AMAZON {market} COMPLETED");
            Logger.Success("══════════════════════════════════════════════════════════════════════════════");
            Logger.Success($"Products Processed : {products.Count}");
            Logger.Success("");


            return scanResult;
        }
    }
}