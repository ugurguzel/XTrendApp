using System.Text.Json;
using Microsoft.Playwright;
using XTrendApp.Web.Common;
using XTrendApp.Web.Models.Amazon;

namespace XTrendApp.Web.Engines.Amazon;

public class AmazonImageParser
{
    public async Task ParseAsync(
        IPage page,
        AmazonVariationSize size)
    {
        var html = await page.ContentAsync();

        var json = ExtractColorImages(html);

        if (string.IsNullOrWhiteSpace(json))
        {
            //Logger.Info("COLOR IMAGES JSON NOT FOUND");
            return;
        }

        var images = ParseImages(json);

        //Logger.Info("");
        //Logger.Info("========== IMAGE PARSER ==========");

        foreach (var color in size.Colors)
        {
            var key = $"{size.Name} {color.Name}";

            if (!images.TryGetValue(key, out var list))
            {
                //Logger.Info($"NOT FOUND : {key}");
                continue;
            }

            var mainImage = list
                .FirstOrDefault(x => x.Variant == "MAIN");

            if (mainImage == null)
            {
                //Logger.Info($"MAIN IMAGE NOT FOUND : {key}");
                continue;
            }

            color.ImageUrl = mainImage.HiRes;

            //Logger.Info($"{key}");
            //Logger.Info($" -> {color.ImageUrl}");
        }

        //Logger.Info("==================================");
        //Logger.Info("");
    }

    private static Dictionary<string, List<AmazonColorImageJson>> ParseImages(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result =
            JsonSerializer.Deserialize<
                Dictionary<string, List<AmazonColorImageJson>>
            >(json, options);

        return result ?? new Dictionary<string, List<AmazonColorImageJson>>();
    }

    private static string? ExtractColorImages(string html)
    {
        const string anchor = "landingAsinColor";

        var anchorIndex =
            html.IndexOf(anchor, StringComparison.OrdinalIgnoreCase);

        if (anchorIndex < 0)
            return null;

        var colorImagesIndex =
            html.IndexOf(
                "colorImages",
                anchorIndex,
                StringComparison.OrdinalIgnoreCase);

        if (colorImagesIndex < 0)
            return null;

        var firstBrace =
            html.IndexOf('{', colorImagesIndex);

        if (firstBrace < 0)
            return null;

        int depth = 0;

        for (int i = firstBrace; i < html.Length; i++)
        {
            if (html[i] == '{')
                depth++;

            else if (html[i] == '}')
            {
                depth--;

                if (depth == 0)
                {
                    return html.Substring(
                        firstBrace,
                        i - firstBrace + 1);
                }
            }
        }

        return null;
    }
}