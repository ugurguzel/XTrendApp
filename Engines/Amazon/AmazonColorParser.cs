using Microsoft.Playwright;
using XTrendApp.Web.Models.Amazon;

namespace XTrendApp.Web.Engines.Amazon;

public class AmazonColorParser
{
    public async Task<List<AmazonVariationColor>> ParseAsync(IPage page)
    {
        var colors = new List<AmazonVariationColor>();

        var items = page.Locator("li[data-asin]");




        var count = await items.CountAsync();

        Console.WriteLine();
        Console.WriteLine($"COLOR ITEM COUNT : {count}");
        Console.WriteLine();

        Console.WriteLine("CURRENT PAGE : " + page.Url);

        Console.WriteLine();

        for (int i = 0; i < count; i++)
        {
            var item = items.Nth(i);

            // Bu li gerçekten Color mı?
            var colorNode = item.Locator("span[id^='color_name_']");

            if (await colorNode.CountAsync() == 0)
                continue;

            var color = new AmazonVariationColor();

            color.Asin =
                await item.GetAttributeAsync("data-asin") ?? "";

            color.Selected =
                (await item.GetAttributeAsync("data-initiallyselected")) == "true";

            color.Available =
                (await item.GetAttributeAsync("data-initiallyunavailable")) != "true";

            //--------------------------------------------------
            // IMAGE / NAME
            //--------------------------------------------------

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

                color.CurrencyCode =
                    page.Url.Contains(".co.uk")
                        ? "GBP"
                        : "USD";
            }

            //--------------------------------------------------
            // STOCK
            //--------------------------------------------------

            color.InStock =
                await item.Locator("#twisterAvailability").CountAsync() > 0;

            //--------------------------------------------------
            // DEBUG
            //--------------------------------------------------

            Console.WriteLine(
                $"COLOR : {color.Name,-20} | ASIN : {color.Asin} | PRICE : {color.CurrentPrice} | STOCK : {color.InStock}");

            if (string.IsNullOrWhiteSpace(color.Name))
            {
                Console.WriteLine();
                Console.WriteLine("========== EMPTY COLOR ==========");
                Console.WriteLine(await item.InnerHTMLAsync());
                Console.WriteLine("=================================");
                Console.WriteLine();

                continue;
            }

            colors.Add(color);
        }

        Console.WriteLine();
        Console.WriteLine($"TOTAL COLORS : {colors.Count}");
        Console.WriteLine();


        Console.WriteLine();
        Console.WriteLine("========== COLOR PARSER ==========");

        foreach (var c in colors)
        {
            Console.WriteLine($"{c.Name,-20} {c.Asin}");
        }

        Console.WriteLine("==================================");
        Console.WriteLine();

        return colors;
    }
}