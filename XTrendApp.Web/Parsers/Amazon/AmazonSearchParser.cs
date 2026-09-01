using Microsoft.Playwright;
using System.Globalization;
using System.Text.RegularExpressions;
using XTrendApp.Web.Models.Amazon;
using XTrendApp.Web.Selectors.Amazon;
using XTrendApp.Web.Models.Common;

namespace XTrendApp.Web.Parsers.Amazon
{
    public class AmazonSearchParser
    {
        public async Task<List<AmazonSearchModel>> ParseAsync(
            IPage page,
            string baseUrl)
        {
            var products = new List<AmazonSearchModel>();

            var cards =
                page.Locator(
                    AmazonSearchSelectors.ProductCard);

            var count =
                await cards.CountAsync();

            Logger.Debug(
                $"Product Cards : {count}");

            if (count == 0)
                return products;


            // --------------------------------------------------
            // PARSE ALL PRODUCTS ON CURRENT SEARCH PAGE
            // --------------------------------------------------

            for (int i = 0; i < count; i++)
            {
                try
                {
                    var product =
                        await ParseProductAsync(
                            cards.Nth(i),
                            baseUrl);


                    if (string.IsNullOrWhiteSpace(
                        product.ProductUrl))
                    {
                        Logger.Error(
                            $"⚠ Product #{i + 1} skipped (URL not found)");

                        continue;
                    }


                    products.Add(product);


                    Logger.Debug(
                        $"Search Product {i + 1}/{count} : {product.Asin}");
                }
                catch (TimeoutException ex)
                {
                    Logger.Error(
                        $"⚠ Product #{i + 1} timeout : {ex.Message}");
                }
                catch (Exception ex)
                {
                    Logger.Error(
                        $"⚠ Product #{i + 1} error : {ex.Message}");
                }
            }


            Logger.Debug(
                $"Products Parsed : {products.Count}");


            return products;
        }


        private async Task<AmazonSearchModel> ParseProductAsync(
            ILocator card,
            string baseUrl)
        {
            var model =
                new AmazonSearchModel();


            model.Asin =
                await GetAsinAsync(card);


            model.Title =
                await GetTitleAsync(card);


            model.ProductUrl =
                await GetProductUrlAsync(
                    card,
                    baseUrl);


            model.ImageUrl =
                await GetImageUrlAsync(card);


            model.Price =
                await GetPriceAsync(card);


            model.ListPrice =
                await GetListPriceAsync(card);


            model.Rating =
                await GetRatingAsync(card);


            model.ReviewCount =
                await GetReviewCountAsync(card);


            model.BoughtLastMonthText =
                await GetBoughtLastMonthTextAsync(card);


            model.BoughtLastMonthCount =
                ParseBoughtLastMonth(
                    model.BoughtLastMonthText);


            model.VariationCount =
                await GetVariationCountAsync(card);


            return model;
        }


        private async Task<string> GetAsinAsync(
            ILocator card)
        {
            try
            {
                var links =
                    card.Locator(
                        AmazonSearchSelectors.Link);


                if (await links.CountAsync() == 0)
                    return string.Empty;


                var href =
                    await links.First
                        .GetAttributeAsync("href");


                if (string.IsNullOrWhiteSpace(href))
                    return string.Empty;


                var match =
                    Regex.Match(
                        href,
                        @"/dp/([A-Z0-9]{10})");


                return match.Success
                    ? match.Groups[1].Value
                    : string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }


        private async Task<string> GetTitleAsync(
            ILocator card)
        {
            var title =
                await card
                    .Locator(
                        AmazonSearchSelectors.Title)
                    .InnerTextAsync();


            return title.Trim();
        }


        private async Task<string> GetProductUrlAsync(
            ILocator card,
            string baseUrl)
        {
            try
            {
                var links =
                    card.Locator(
                        AmazonSearchSelectors.Link);


                if (await links.CountAsync() == 0)
                    return string.Empty;


                var href =
                    await links.First
                        .GetAttributeAsync("href");


                if (string.IsNullOrWhiteSpace(href))
                    return string.Empty;


                return href.StartsWith("/")
                    ? baseUrl + href
                    : href;
            }
            catch (TimeoutException)
            {
                Logger.Error(
                    "⚠ Product link timeout.");

                return string.Empty;
            }
        }


        private async Task<string> GetImageUrlAsync(
            ILocator card)
        {
            var image =
                card
                    .Locator(
                        AmazonSearchSelectors.Image)
                    .First;


            if (await image.CountAsync() == 0)
                return string.Empty;


            var src =
                await image.GetAttributeAsync("src");


            return src ?? string.Empty;
        }


        private async Task<decimal?> GetPriceAsync(
            ILocator card)
        {
            var price =
                card
                    .Locator(
                        AmazonSearchSelectors.Price)
                    .First;


            if (await price.CountAsync() == 0)
                return null;


            var whole =
                await price
                    .Locator(".a-price-whole")
                    .InnerTextAsync();


            var fraction =
                await price
                    .Locator(".a-price-fraction")
                    .InnerTextAsync();


            whole =
                whole.Replace(",", "")
                     .Replace(".", "")
                     .Trim();


            var value =
                $"{whole}.{fraction}";


            if (decimal.TryParse(
                value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal result))
            {
                return result;
            }


            return null;
        }


        private async Task<decimal?> GetListPriceAsync(
            ILocator card)
        {
            var price =
                card
                    .Locator(
                        AmazonSearchSelectors.ListPrice)
                    .First;


            if (await price.CountAsync() == 0)
                return null;


            var text =
                await price
                    .Locator(".a-offscreen")
                    .InnerTextAsync();


            if (string.IsNullOrWhiteSpace(text))
                return null;


            text =
                text.Replace("$", "")
                    .Replace("£", "")
                    .Replace("€", "")
                    .Replace(",", "")
                    .Trim();


            if (decimal.TryParse(
                text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal result))
            {
                return result;
            }


            return null;
        }


        private async Task<decimal?> GetRatingAsync(
            ILocator card)
        {
            var text =
                await TryGetInnerTextAsync(
                    card.Locator(
                        AmazonSearchSelectors.Rating));


            if (string.IsNullOrWhiteSpace(text))
                return null;


            text =
                text.Split(' ')[0];


            if (decimal.TryParse(
                text,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal result))
            {
                return result;
            }


            return null;
        }


        private async Task<int?> GetReviewCountAsync(
            ILocator card)
        {
            var text =
                await TryGetInnerTextAsync(
                    card.Locator(
                        AmazonSearchSelectors.ReviewCount));


            if (string.IsNullOrWhiteSpace(text))
                return null;


            text =
                text.Replace("(", "")
                    .Replace(")", "")
                    .Trim();


            return ParseReviewCount(text);
        }


        private int? ParseReviewCount(
            string value)
        {
            value =
                value.Trim()
                     .ToUpperInvariant();


            if (value.EndsWith("K"))
            {
                value =
                    value.Replace("K", "");


                if (decimal.TryParse(
                    value,
                    NumberStyles.Any,
                    CultureInfo.InvariantCulture,
                    out decimal number))
                {
                    return (int)(number * 1000);
                }
            }


            if (int.TryParse(
                value,
                out int result))
            {
                return result;
            }


            return null;
        }


        private async Task<string?> GetBoughtLastMonthTextAsync(
            ILocator card)
        {
            return await TryGetInnerTextAsync(
                card.Locator(
                    AmazonSearchSelectors.BoughtLastMonth));
        }


        private int? ParseBoughtLastMonth(
            string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;


            text =
                text.ToUpperInvariant();


            var match =
                Regex.Match(
                    text,
                    @"([\d\.]+)\s*([KM]?)");


            if (!match.Success)
                return null;


            if (!decimal.TryParse(
                match.Groups[1].Value,
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal number))
            {
                return null;
            }


            switch (match.Groups[2].Value)
            {
                case "K":
                    number *= 1000;
                    break;


                case "M":
                    number *= 1000000;
                    break;
            }


            return (int)number;
        }


        private async Task<int?> GetVariationCountAsync(
            ILocator card)
        {
            var item =
                card
                    .Locator(
                        AmazonSearchSelectors.VariationCount)
                    .First;


            if (await item.CountAsync() == 0)
                return null;


            var text =
                await item.InnerTextAsync();


            if (string.IsNullOrWhiteSpace(text))
                return null;


            return ParseVariationCount(text);
        }


        private int? ParseVariationCount(
            string text)
        {
            var match =
                Regex.Match(
                    text,
                    @"\d+");


            if (!match.Success)
                return null;


            return int.Parse(
                match.Value);
        }


        private static async Task<string?> TryGetInnerTextAsync(
            ILocator locator)
        {
            try
            {
                if (await locator.CountAsync() == 0)
                    return null;


                return (
                    await locator.First
                        .InnerTextAsync())
                    .Trim();
            }
            catch
            {
                return null;
            }
        }
    }
}