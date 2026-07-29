using Microsoft.Playwright;
using System.Text.RegularExpressions;
using XTrendApp.Web.Models.Amazon;

namespace XTrendApp.Web.Engines.Amazon;

public class AmazonVariationNavigator
{
    public async Task<string> GoToSizeAsync(
        IPage page,
        string baseUrl,
        AmazonVariationSize size)
    {
        Console.WriteLine($"Selecting Size : {size.Name}");

        await page.GotoAsync(
            $"{baseUrl}/dp/{size.Asin}",
            new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 60000
            });


        Console.WriteLine("PAGE TITLE : " + await page.TitleAsync());


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

        Console.WriteLine($"Current URL : {page.Url}");

        var match = Regex.Match(
            page.Url,
            @"/dp/([A-Z0-9]{10})");

        Console.WriteLine();
        Console.WriteLine("====================================");
        Console.WriteLine($"EXPECTED SIZE : {size.Name}");
        Console.WriteLine($"EXPECTED ASIN : {size.Asin}");
        Console.WriteLine($"CURRENT URL   : {page.Url}");
        Console.WriteLine("====================================");
        Console.WriteLine();

        return match.Success
            ? match.Groups[1].Value
            : string.Empty;
    }
}