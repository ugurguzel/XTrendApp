using Microsoft.Playwright;
using XTrendApp.Web.Models.Amazon;

namespace XTrendApp.Web.Engines.Amazon
{
    public class AmazonButtonParser
    {
        public async Task ParseAsync(
            ILocator root,
            AmazonVariationResult result)
        {
            var items = root.Locator("li.swatch-list-item-text");

            var count = await items.CountAsync();

            Console.WriteLine($"Size Swatches    : {count}");
            Console.WriteLine();


            for (int i = 0; i < count; i++)
            {
                var item = items.Nth(i);

                if (i == 0)
                {
                    //Console.WriteLine();
                    //Console.WriteLine("========== FIRST SIZE HTML ==========");
                    //Console.WriteLine(await item.EvaluateAsync<string>("e => e.outerHTML"));
                    //Console.WriteLine("=====================================");
                    //Console.WriteLine();
                }

                var name = (await item.InnerTextAsync()).Trim();

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var asin =
                    await item.GetAttributeAsync("data-asin");

                bool isSelected =
                    (await item.GetAttributeAsync("data-initiallyselected"))
                    == "true";

                bool isAvailable =
                    (await item.GetAttributeAsync("data-initiallyunavailable"))
                    != "true";

                result.Sizes.Add(new AmazonVariationSize
                {
                    Name = name,
                    Asin = asin ?? string.Empty,
                    Selected = isSelected,
                    Available = isAvailable
                });
            }

        }
    }
}