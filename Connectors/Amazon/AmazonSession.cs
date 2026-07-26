using Microsoft.Extensions.Options;
using Microsoft.Playwright;

namespace XTrendApp.Web.Connectors.Amazon
{
    public class AmazonSession
    {
        private readonly AmazonOptions _options;

        public AmazonSession(IOptions<AmazonOptions> options)
        {
            _options = options.Value;
        }

        public async Task ConfigureUsSessionAsync()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 300
                });

            var context = await browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    Locale = "en-US",
                    TimezoneId = "America/New_York"
                });

            var page = await context.NewPageAsync();

            await page.GotoAsync("https://www.amazon.com");

            // BURADA DURUYORUZ.
            // Sen Continue Shopping'e bas.
            // ZIP'i 43215 yap.
            // Amazon ana sayfasına geldiğinde bu URL'yi aç:
            //
            // https://localhost:7055/Connector/SaveAmazonUsSession
            //
            // Browser'ı KAPATMA.
            // Açık bırak.
            //
            SessionHolder.Context = context;

            while (SessionHolder.Context != null)
            {
                await Task.Delay(1000);
            }
        }

        public async Task ConfigureUkSessionAsync()
        {
            using var playwright = await Playwright.CreateAsync();

            await using var browser = await playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = false,
                    SlowMo = 300
                });

            var context = await browser.NewContextAsync();

            new BrowserNewContextOptions
            {
                Locale = "en-GB",
                TimezoneId = "Europe/London"
            };

            var page = await context.NewPageAsync();

            await page.GotoAsync("https://www.amazon.co.uk");

            SessionHolder.Context = context;

            while (SessionHolder.Context != null)
            {
                await Task.Delay(1000);
            }
        }
    }

    public static class SessionHolder
    {
        public static IBrowserContext? Context { get; set; }
    }
}