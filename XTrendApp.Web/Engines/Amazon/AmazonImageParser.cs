using System.Text.Json;
using Microsoft.Playwright;
using XTrendApp.Web.Models.Common;
using XTrendApp.Web.Models.Amazon;

namespace XTrendApp.Web.Engines.Amazon;

public class AmazonImageParser
{
    public async Task ParseAsync(
    IPage page,
    AmazonVariationSize size)
    {
        var html = await page.ContentAsync();

        // ===========================
        // AMAZON US
        // ===========================
        if (page.Url.Contains("amazon.com"))
        {
            var json = ExtractColorImagesAmazonUS(html);

            if (string.IsNullOrWhiteSpace(json))
                return;

            Logger.Info("");
            Logger.Info("========== US RAW IMAGE JSON ==========");
            Logger.Info(json.Substring(0, Math.Min(15000, json.Length)));
            Logger.Info("=======================================");
            Logger.Info("");

            var usImages = ParseImagesAmazonUS(json);

            Logger.Info($"US Images : {usImages.Count}");

            // Şimdilik burada duruyoruz.
            // Bir sonraki adımda usImages -> size.Colors eşleştirmesini yapacağız.

            return;
        }

        // ===========================
        // AMAZON UK
        // ===========================

        var ukJson = ExtractColorImagesAmazonUK(html);

        if (string.IsNullOrWhiteSpace(ukJson))
            return;

        var images = ParseImagesAmazonUK(ukJson);

        foreach (var color in size.Colors)
        {
            var key = $"{size.Name} {color.Name}";

            if (!images.TryGetValue(key, out var list))
                continue;

            var mainImage =
                list.FirstOrDefault(x => x.Variant == "MAIN");

            if (mainImage == null)
                continue;

            color.ImageUrl = mainImage.HiRes;
        }
    }

    private static Dictionary<string, List<AmazonColorImageJson>> ParseImagesAmazonUK(string json)
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

    private static List<AmazonColorImageJson>
    ParseImagesAmazonUS(string json)
    {
        json = json.Replace("'", "\"");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var result =
            JsonSerializer.Deserialize<AmazonUsImageRoot>(
                json,
                options);

        return result?.Initial
               ?? new List<AmazonColorImageJson>();
    }

    private static string? ExtractColorImagesAmazonUK(string html)
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

    private static string? ExtractColorImagesAmazonUS(string html)
    {

        int pos = 0;

        while ((pos = html.IndexOf("colorImages", pos, StringComparison.OrdinalIgnoreCase)) >= 0)
        {
            Logger.Info("");
            Logger.Info($"========== colorImages @ {pos} ==========");

            Logger.Info(
                html.Substring(
                    pos,
                    Math.Min(2000, html.Length - pos)));

            Logger.Info("=========================================");
            Logger.Info("");

            pos += 10;
        }

        const string anchor = "colorImages':";

        var anchorIndex =
            html.IndexOf(
                anchor,
                StringComparison.OrdinalIgnoreCase);

        if (anchorIndex < 0)
        {
            Logger.Info("US : colorImages anchor NOT FOUND");
            return null;
        }

        Logger.Info($"US : colorImages anchor = {anchorIndex}");

        var firstBrace =
            html.IndexOf('{', anchorIndex);

        if (firstBrace < 0)
        {
            Logger.Info("US : first brace NOT FOUND");
            return null;
        }

        Logger.Info($"US : first brace = {firstBrace}");

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
                    var json = html.Substring(
                        firstBrace,
                        i - firstBrace + 1);

                    Logger.Info("");
                    Logger.Info("========== US RAW JSON ==========");
                    Logger.Info(json.Substring(0, Math.Min(1000, json.Length)));
                    Logger.Info("=================================");
                    Logger.Info("");

                    return json;
                }
            }
        }

        Logger.Info("US : closing brace NOT FOUND");

        return null;
    }
}