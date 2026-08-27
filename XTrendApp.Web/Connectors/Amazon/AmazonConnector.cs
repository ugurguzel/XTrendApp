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
        private readonly AmazonSearchNavigator _searchNavigator;
        private readonly AmazonDetailParser _detailParser;
        private readonly AmazonVariationEngine _variationEngine;
        private readonly ProductImportService _productImportService;

        public AmazonConnector(
            IOptions<AmazonOptions> options,
            IWebHostEnvironment environment,
            AmazonSearchParser searchParser,
            AmazonSearchNavigator searchNavigator,
            AmazonDetailParser detailParser,
            AmazonVariationEngine variationEngine,
            ProductImportService productImportService)
        {
            _options = options.Value;
            _environment = environment;
            _searchParser = searchParser;
            _searchNavigator = searchNavigator;
            _detailParser = detailParser;
            _variationEngine = variationEngine;
            _productImportService = productImportService;
        }

        public async Task<ScanExecutionResult> RunAsync(
    AmazonMarket market,
    long scanExecutionId,
    int productLimit)
        {
            string baseUrl;
            string sessionFile;
            string searchUrl;

            switch (market)
            {
                case AmazonMarket.US:
                    baseUrl = "https://www.amazon.com";
                    sessionFile = "amazon-us.json";
                    searchUrl = "https://www.amazon.com/s?i=garden&rh=n%3A684541011&s=popularity-rank&fs=true&ref=lp_684541011_sar";
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

            var products = new List<AmazonSearchModel>();

            var seenAsins = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            var searchPage = 1;

            while (products.Count < productLimit)
            {
                Logger.Info("");
                Logger.Info(
                    $"SEARCH PAGE {searchPage}");

                var pageProducts =
                    await _searchParser.ParseAsync(
                        page,
                        baseUrl);

                Logger.Info(
                    $"Page {searchPage} Products : {pageProducts.Count}");


                var addedThisPage = 0;


                foreach (var product in pageProducts)
                {
                    if (string.IsNullOrWhiteSpace(product.Asin))
                    {
                        Logger.Info(
                            "Search product skipped: ASIN not found.");

                        continue;
                    }

                    if (!seenAsins.Add(product.Asin))
                    {
                        Logger.Debug(
                            $"Duplicate ASIN skipped : {product.Asin}");

                        continue;
                    }

                    product.CurrencyCode = market switch
                    {
                        AmazonMarket.US => "USD",
                        AmazonMarket.UK => "GBP",
                        _ => string.Empty
                    };

                    products.Add(product);

                    addedThisPage++;

                    Logger.Debug(
                        $"Product added : {product.Asin}");

                    if (products.Count >= productLimit)
                        break;
                }

                Logger.Info(
                    $"Unique Products Collected : {products.Count}/{productLimit}");

                if (products.Count >= productLimit)
                {
                    Logger.Info(
                        $"Maximum product target reached : {productLimit}");

                    break;
                }


                Logger.Info(
    $"Unique Products Collected : {products.Count}/{productLimit}");


                // --------------------------------------------------
                // TARGET REACHED
                // --------------------------------------------------

                if (products.Count >= productLimit)
                {
                    Logger.Info(
                        $"Maximum product target reached : {productLimit}");

                    break;
                }


                // --------------------------------------------------
                // NO NEW PRODUCTS
                // --------------------------------------------------

                if (addedThisPage == 0)
                {
                    Logger.Info(
                        "No new products found on current search page.");

                    break;
                }


                // --------------------------------------------------
                // NEXT PAGE
                // --------------------------------------------------

                Logger.Info(
                    $"Moving to Amazon Search Page {searchPage + 1}...");


                var movedToNextPage =
                    await _searchNavigator.GoToNextPageAsync(page);


                if (!movedToNextPage)
                {
                    Logger.Info(
                        "No more Amazon search pages available.");

                    break;
                }


                searchPage++;
            }


            Logger.Info("");
            Logger.Info(
                $"Products Found : {products.Count}");


            //--------------------------------------------------
            // DETAIL PARSER
            //--------------------------------------------------

            var scanResult = new ScanExecutionResult();

            foreach (var product in products)
            {
                IPage? productPage = null;

                try
                {
                    Logger.Info("");
                    Logger.Info("──────────────────────────────────────────────────────────────────────────────");
                    Logger.Info($"PROCESSING PRODUCT : {product.Asin}");
                    Logger.Info("──────────────────────────────────────────────────────────────────────────────");

                    //--------------------------------------------------
                    // PRODUCT PAGE
                    //--------------------------------------------------

                    productPage = await context.NewPageAsync();

                    Logger.Info(
                        $"PRODUCT NAVIGATION : {baseUrl}/dp/{product.Asin}");

                    await productPage.GotoAsync(
                        $"{baseUrl}/dp/{product.Asin}",
                        new PageGotoOptions
                        {
                            WaitUntil = WaitUntilState.DOMContentLoaded
                        });

                    Logger.Info(
                        $"PRODUCT PAGE LOADED : {product.Asin}");

                    //--------------------------------------------------
                    // DETAIL PARSER
                    //--------------------------------------------------

                    Logger.Info(
                        $"DETAIL PARSER START : {product.Asin}");

                    AmazonDetailModel detail =
                        await _detailParser.ParseAsync(
                            productPage,
                            product);

                    Logger.Info(
                        $"DETAIL PARSER COMPLETED : {product.Asin}");

                    //--------------------------------------------------
                    // VARIATION ENGINE
                    //--------------------------------------------------

                    var variation = await _variationEngine.ParseAsync(
                        productPage,
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
                        _ => throw new InvalidOperationException(
                            "Unknown market.")
                    };

                    var sourceName = market switch
                    {
                        AmazonMarket.US => "amazon-us",
                        AmazonMarket.UK => "amazon-uk",
                        _ => throw new InvalidOperationException(
                            "Unknown market.")
                    };

                    var importResult =
                        await _productImportService.ImportAsync(
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

                    Logger.Success(
                        $"PRODUCT COMPLETED : {product.Asin}");
                }
                catch (Exception ex)
                {
                    Logger.Error("");
                    Logger.Error(
                        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                    Logger.Error(
                        $"PRODUCT FAILED : {product.Asin}");

                    Logger.Error(
                        $"PRODUCT URL    : {baseUrl}/dp/{product.Asin}");

                    Logger.Error(
                        $"ERROR          : {ex.Message}");

                    Logger.Error(
                        $"EXCEPTION      : {ex.GetType().Name}");

                    Logger.Error(
                        "!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");

                    Logger.Error("");

                    continue;
                }
                finally
                {
                    if (productPage != null)
                    {
                        try
                        {
                            await productPage.CloseAsync();
                        }
                        catch
                        {
                            // Sayfa zaten kapanmış olabilir.
                        }
                    }
                }
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