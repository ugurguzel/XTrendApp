using Microsoft.Playwright;
using XTrendApp.Web.Connectors.Amazon;
using XTrendApp.Web.Models.Amazon;

namespace XTrendApp.Web.Engines.Amazon;

public class AmazonVariationScanner
{

    private readonly AmazonVariationNavigator _navigator;
    private readonly AmazonColorParser _colorParser;

    public AmazonVariationScanner(AmazonVariationNavigator navigator, AmazonColorParser colorParser)
    {
        _navigator = navigator;
        _colorParser = colorParser;
    }



    public async Task ParseAsync(
        IPage page,
        AmazonVariationResult variation,
        string baseUrl,
        AmazonMarket market)
    {
        if (variation.Sizes.Count == 0)
            return;

        foreach (var size in variation.Sizes)
        {
            //await page.GotoAsync(
            //    $"{baseUrl}/dp/{size.Asin}",
            //    new PageGotoOptions
            //    {
            //        WaitUntil = WaitUntilState.DOMContentLoaded
            //    });

            //await page.WaitForTimeoutAsync(800);

            await _navigator.GoToSizeAsync(page, size);

            var colors = await _colorParser.ParseAsync(page);

            size.Colors = colors;

            Console.WriteLine();
            Console.WriteLine($"========== SIZE : {size.Name} ==========");

            foreach (var color in colors)
            {
                Console.WriteLine(
                    $"{color.Name} | {color.Asin} | {color.CurrentPrice}");
            }

            Console.WriteLine("======================================");
            Console.WriteLine();

            var titleLocator = page.Locator("#productTitle").First;

            if (await titleLocator.CountAsync() > 0)
            {
                size.Title = (await titleLocator.InnerTextAsync()).Trim();
            }

            size.Price = await TryGetPriceAsync(page);
            size.ListPrice = await TryGetListPriceAsync(page);
            size.CurrencyCode = market == AmazonMarket.US
    ? "USD"
    : "GBP";

            size.ImageUrl = await TryGetImageAsync(page);
        }
    }

    private async Task<decimal?> TryGetPriceAsync(IPage page)
    {
        string[] selectors =
        {
            ".apexPriceToPay .a-offscreen",
            ".reinventPricePriceToPayMargin .a-offscreen",
            ".a-price .a-offscreen"
        };

        foreach (var selector in selectors)
        {



            var locator = page.Locator(selector).First;

            if (await locator.CountAsync() == 0)
                continue;

            var text = await locator.TextContentAsync();

            if (string.IsNullOrWhiteSpace(text))
                continue;

            text = text
                .Replace("$", "")
                .Replace("£", "")
                .Replace("€", "")
                .Replace(",", "")
                .Trim();

            if (decimal.TryParse(
                    text,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var price))
            {
                return price;
            }
        }

        return null;
    }

    private async Task<decimal?> TryGetListPriceAsync(IPage page)
    {
        var locator = page.Locator(".apex-basisprice-value .a-offscreen").First;

        if (await locator.CountAsync() == 0)
            return null;

        var text = await locator.TextContentAsync();

        if (string.IsNullOrWhiteSpace(text))
            return null;

        text = text
            .Replace("$", "")
            .Replace("£", "")
            .Replace("€", "")
            .Replace(",", "")
            .Trim();

        if (decimal.TryParse(
                text,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var value))
        {
            return value;
        }

        return null;
    }

    private async Task<string?> TryGetImageAsync(IPage page)
    {
        var image = page.Locator("#landingImage");

        try
        {
            await image.WaitForAsync(new()
            {
                State = WaitForSelectorState.Visible,
                Timeout = 3000
            });
        }
        catch
        {
            return null;
        }

        var src = await image.GetAttributeAsync("src");

        if (!string.IsNullOrWhiteSpace(src))
            return src;

        var hires = await image.GetAttributeAsync("data-old-hires");

        if (!string.IsNullOrWhiteSpace(hires))
            return hires;

        Console.WriteLine();
        Console.WriteLine("========== IMAGE DEBUG ==========");
        Console.WriteLine($"Page : {page.Url}");
        Console.WriteLine($"src  : {src}");
        Console.WriteLine($"hires: {hires}");
        Console.WriteLine("=================================");
        Console.WriteLine();

        return null;
    }
}