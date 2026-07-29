using Microsoft.Playwright;
using XTrendApp.Web.Connectors.Amazon;
using XTrendApp.Web.Models.Amazon;
using XTrendApp.Web.Common;

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
            

            await _navigator.GoToSizeAsync(page, baseUrl,
size);

            Logger.Debug("");
            Logger.Debug("====================================");
            Logger.Debug($"EXPECTED SIZE : {size.Name}");
            Logger.Debug($"EXPECTED ASIN : {size.Asin}");
            Logger.Debug($"CURRENT URL   : {page.Url}");

            var currentSize = page.Locator("#native_dropdown_selected_size_name");

            if (await currentSize.CountAsync() > 0)
            {
                Logger.Debug($"PAGE SIZE     : {await currentSize.InputValueAsync()}");
            }

            Logger.Debug("====================================");
            Logger.Debug("");

            var colors = await _colorParser.ParseAsync(page);

            Logger.Debug("");
            Logger.Debug($"SIZE : {size.Name}");

            foreach (var c in colors)
            {
                Logger.Debug($"{c.Name} -> {c.Asin}");
            }

            Logger.Debug("");

            size.Colors = colors;

            var duplicateGroups = variation.Sizes
    .SelectMany(s => s.Colors.Select(c => new
    {
        Size = s.Name,
        Color = c.Name,
        c.Asin
    }))
    .GroupBy(x => x.Asin)
    .Where(g => g.Count() > 1);

            foreach (var group in duplicateGroups)
            {
                Logger.Debug($"DUPLICATE ASIN : {group.Key}");

                foreach (var item in group)
                {
                    Logger.Debug($"   {item.Size} -> {item.Color}");
                }

                Logger.Debug("");
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

        Logger.Debug("");
        Logger.Debug("========== IMAGE DEBUG ==========");
        Logger.Debug($"Page : {page.Url}");
        Logger.Debug($"src  : {src}");
        Logger.Debug($"hires: {hires}");
        Logger.Debug("=================================");
        Logger.Debug("");

        return null;
    }
}