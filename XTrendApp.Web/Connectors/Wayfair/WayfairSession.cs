using Microsoft.Extensions.Options;
using Microsoft.Playwright;
using Microsoft.AspNetCore.Hosting;
using XTrendApp.Web.Models.Common;

namespace XTrendApp.Web.Connectors.Wayfair
{
    public class WayfairSession
    {
        private readonly WayfairOptions _options;
        private readonly IWebHostEnvironment _environment;

        public WayfairSession(
    IOptions<WayfairOptions> options,
    IWebHostEnvironment environment)
        {
            _options = options.Value;
            _environment = environment;
        }

        public async Task ConfigureSessionAsync()
        {
            using var playwright = await Playwright.CreateAsync();

            var profilePath = Path.Combine(
    _environment.ContentRootPath,
    "App_Data",
    "BrowserProfiles",
    "Wayfair");

            Directory.CreateDirectory(profilePath);

            var context =
                await playwright.Chromium.LaunchPersistentContextAsync(
                    profilePath,
                    new BrowserTypeLaunchPersistentContextOptions
                    {
                        Channel = "chrome",

                        Headless = false,

                        UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/137.0.0.0 Safari/537.36",

                        ViewportSize = new ViewportSize
                        {
                            Width = 1920,
                            Height = 1080
                        },

                        Locale = "en-US",

                        TimezoneId = "America/New_York",

                        ColorScheme = ColorScheme.Light,

                        SlowMo = 200
                    });

            var page = context.Pages.FirstOrDefault();

            if (page == null)
                page = await context.NewPageAsync();

            await page.GotoAsync(_options.BaseUrl);

            Logger.Debug("Wayfair profile is open. Complete the verification and close the browser manually.");

            await page.WaitForTimeoutAsync(150000);

        }
    }
}