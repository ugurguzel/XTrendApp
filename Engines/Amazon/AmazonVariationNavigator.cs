using Microsoft.Playwright;
using System.Text.RegularExpressions;
using XTrendApp.Web.Models.Amazon;
using XTrendApp.Web.Common;

namespace XTrendApp.Web.Engines.Amazon;

public class AmazonVariationNavigator
{
    public async Task<string> GoToSizeAsync(
        IPage page,
        string baseUrl,
        AmazonVariationSize size)
    {
        Logger.Debug($"Selecting Size : {size.Name}");

        await page.GotoAsync(
            $"{baseUrl}/dp/{size.Asin}",
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60000
            });


        Logger.Debug("PAGE TITLE : " + await page.TitleAsync());


        // Amazon'da NetworkIdle güvenilir değil.
        // Bunun yerine Twister'ın oluşmasını bekliyoruz.

        await page.Locator("li[data-asin]")
            .First
            .WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = 30000
            });

        // DOM'un tamamen güncellenmesi için kısa bekleme
        //await page.WaitForTimeoutAsync(500);

        Logger.Debug($"Current URL : {page.Url}");

        var match = Regex.Match(
            page.Url,
            @"/dp/([A-Z0-9]{10})");

        Logger.Debug("");
        Logger.Debug("====================================");
        Logger.Debug($"EXPECTED SIZE : {size.Name}");
        Logger.Debug($"EXPECTED ASIN : {size.Asin}");
        Logger.Debug($"CURRENT URL   : {page.Url}");
        Logger.Debug("====================================");
        Logger.Debug("");

        return match.Success
            ? match.Groups[1].Value
            : string.Empty;
    }
}