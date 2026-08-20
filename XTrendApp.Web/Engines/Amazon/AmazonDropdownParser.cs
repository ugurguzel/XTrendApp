using Microsoft.Playwright;
using XTrendApp.Web.Models.Amazon;
using XTrendApp.Web.Common;


namespace XTrendApp.Web.Engines.Amazon
{
    public class AmazonDropdownParser
    {
        public async Task ParseAsync(
            ILocator dropdown,
            AmazonVariationResult result)
        {
            var options = dropdown.Locator("option");

            var count = await options.CountAsync();

            Logger.Debug($"Size Options     : {count}");
            
            for (int i = 0; i < count; i++)
            {
                var option = options.Nth(i);

                var name = (await option.InnerTextAsync()).Trim();

                if (string.IsNullOrWhiteSpace(name))
                    continue;

                var value = await option.GetAttributeAsync("value");

                string asin = string.Empty;
                int optionIndex = -1;

                if (!string.IsNullOrWhiteSpace(value))
                {
                    var parts = value.Split(',');

                    if (parts.Length == 2)
                    {
                        int.TryParse(parts[0], out optionIndex);
                        asin = parts[1];
                    }
                }

                bool isSelected =
                    await option.GetAttributeAsync("selected") != null;

                bool isAvailable =
                    await option.GetAttributeAsync("disabled") == null;

                result.Sizes.Add(new AmazonVariationSize
                {
                    Name = name,
                    Asin = asin ?? string.Empty,
                    Selected = isSelected,
                    Available = isAvailable,
                    OptionValue = value ?? "",
                    OptionIndex = i
                });

            }
        }
    }
}