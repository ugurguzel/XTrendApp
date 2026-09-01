using Microsoft.Playwright;
using XTrendApp.Web.Models.Amazon;
using XTrendApp.Web.Models.Common;

public sealed class AmazonDetailParser
{
    public async Task<AmazonDetailModel> ParseAsync(
        IPage page,
        AmazonSearchModel searchModel)
    {
        var detail = new AmazonDetailModel
        {
            // Search
            Asin = searchModel.Asin,
            Title = searchModel.Title,
            

            ProductUrl = searchModel.ProductUrl,
            ImageUrl = searchModel.ImageUrl,

            CurrencyCode = searchModel.CurrencyCode,

            // Snapshot
            Price = searchModel.Price,
            Rating = searchModel.Rating,
            ReviewCount = searchModel.ReviewCount
        };

        await ParseProductInformationAsync(page, detail);

        FillGeneralInformation(detail);

        return detail;
    }

    private async Task ParseProductInformationAsync(
    IPage page,
    AmazonDetailModel detail)
    {
            Logger.Debug("----------------------------------------");
        Logger.Debug("PRODUCT INFORMATION");
        Logger.Debug("----------------------------------------");

        await ParseSectionAsync(page, detail, "Features & Specs");
        await ParseSectionAsync(page, detail, "Item details");
        await ParseSectionAsync(page, detail, "Style");
        await ParseSectionAsync(page, detail, "Measurements");
        await ParseSectionAsync(page, detail, "Materials & Care");
    }

    private async Task ParseSectionAsync(
    IPage page,
    AmazonDetailModel detail,
    string sectionName)
    {
        Logger.Debug("");
        Logger.Debug($"[{sectionName}]");

        var section = page
            .Locator("#prodDetails .a-expander-container")
            .Filter(new() { HasText = sectionName })
            .First;

        if (await section.CountAsync() == 0)
        {
            Logger.Debug("Not Found");
            return;
        }

        var values = await ReadSectionAsync(section);

        detail.Sections[sectionName] = values;

        foreach (var item in values)
        {
            PrintKeyValue(item.Key, item.Value);
        }
    }

    private static void PrintKeyValue(
    string key,
    string value)
    {
        Logger.Debug($"{key,-40} : {value}");
    }

    private async Task<Dictionary<string, string>> ReadSectionAsync(
    ILocator section)
    {
        var values = new Dictionary<string, string>();

        var rows = section.Locator("tbody tr");

        var rowCount = await rows.CountAsync();

        for (int i = 0; i < rowCount; i++)
        {
            var row = rows.Nth(i);

            var key = (await row
                .Locator("th")
                .InnerTextAsync())
                .Trim();

            key = NormalizeKey(key);

            var value = await row
                .Locator("td")
                .InnerTextAsync();

            value = CleanValue(key, value);

            if (!values.ContainsKey(key))
            {
                values.Add(key, value);
            }
        }

        return values;
    }
    private static string CleanValue(
    string key,
    string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        value = value.Replace("\r", " ");
        value = value.Replace("\n", " ");
        value = value.Replace("(See Top 100 in Home & Kitchen)", "");
        value = value.Replace("  #", " | #");

        while (value.Contains("  "))
            value = value.Replace("  ", " ");

        if (value.Contains("var "))
            value = value[..value.IndexOf("var ")];

        if (value.Contains("P.when"))
            value = value[..value.IndexOf("P.when")];

        value = value.Trim();

        return value;
    }

    
    private static string NormalizeKey(string key)
    {
        return key
            .Replace("Other Special Features of the Product", "Special Features")
            .Replace("Product Care Instructions", "Care Instructions")
            .Replace("Back Material Type", "Back Material")
            .Replace("Material Type", "Material")
            .Replace("Item Dimensions L x W", "Dimensions")
            .Replace("Item Shape", "Shape")
            .Trim();
    }

    private static void FillGeneralInformation(AmazonDetailModel detail)
    {
        foreach (var section in detail.Sections.Values)
        {
            if (string.IsNullOrWhiteSpace(detail.Brand))
            {
                if (section.TryGetValue("Brand Name", out var brand))
                {
                    detail.Brand = brand;
                }
            }

            if (string.IsNullOrWhiteSpace(detail.Collection))
            {
                if (section.TryGetValue("Collection", out var collection))
                {
                    detail.Collection = collection;
                }
            }
        }
    }

    
}



