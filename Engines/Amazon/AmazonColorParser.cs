using Microsoft.Playwright;
using XTrendApp.Web.Models.Amazon;

namespace XTrendApp.Web.Engines.Amazon;

public class AmazonColorParser
{
    public async Task<List<AmazonVariationColor>> ParseAsync(
        IPage page)
    {
        var colors = new List<AmazonVariationColor>();

        var items = page.Locator(
    "#tp-inline-twister-dim-values-container li[data-asin]");

        var count = await items.CountAsync();

        Console.WriteLine($"Color Swatches : {count}");
        Console.WriteLine();

        for (int i = 0; i < count; i++)
        {
            var item = items.Nth(i);

            var isColor =
    await item.Locator("[id^='color_name_']").CountAsync() > 0;

            if (!isColor)
                continue;

            var color = new AmazonVariationColor();

            color.Asin =
                await item.GetAttributeAsync("data-asin") ?? "";

            color.Selected =
                (await item.GetAttributeAsync("data-initiallyselected")) == "true";

            color.Available =
                (await item.GetAttributeAsync("data-initiallyunavailable")) != "true";

            var image = item.Locator("img").First;

            if (await image.CountAsync() > 0)
            {
                color.Name =
                    await image.GetAttributeAsync("alt") ?? "";

                color.ImageUrl =
                    await image.GetAttributeAsync("src");
            }

            //--------------------------------------------------
            // PRICE
            //--------------------------------------------------

            var priceLocator =
    item.Locator(".apex-pricetopay-value span[aria-hidden='true']").First;

            if (await priceLocator.CountAsync() > 0)
            {
                var priceText = await priceLocator.InnerTextAsync();

                priceText = priceText
                    .Replace("£", "")
                    .Replace("$", "")
                    .Replace("€", "")
                    .Replace(",", "")
                    .Trim();

                if (decimal.TryParse(
                    priceText,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out var currentPrice))
                {
                    color.CurrentPrice = currentPrice;
                }

                color.CurrencyCode = page.Url.Contains(".co.uk")
                    ? "GBP"
                    : "USD";
            }

            //--------------------------------------------------
            // STOCK
            //--------------------------------------------------

            var stockLocator = item.Locator("#twisterAvailability");

            color.InStock =
                await stockLocator.CountAsync() > 0;

            if (string.IsNullOrWhiteSpace(color.Name))
            {
                Console.WriteLine(await item.InnerHTMLAsync());
                break;
            }

            colors.Add(color);
        }

        return colors;
    }
}