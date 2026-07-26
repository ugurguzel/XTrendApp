using Microsoft.Playwright;
using XTrendApp.Web.Models.Amazon;

namespace XTrendApp.Web.Engines.Amazon;

public class AmazonVariationNavigator
{
    public async Task<string> GoToSizeAsync(
    IPage page,
    AmazonVariationSize size)
    {
        if (size.Selected)
            return size.Asin;

        Console.WriteLine($"Selecting Size : {size.Name}");

        //--------------------------------------------------
        // BUTTON
        //--------------------------------------------------

        if (size.OptionIndex < 0)
        {
            // Şimdilik button desteği yok
            return size.Asin;
        }

        //--------------------------------------------------
        // DROPDOWN
        //--------------------------------------------------

        var dropdownButton = page.Locator("#dropdown_selected_size_name");

        if (await dropdownButton.CountAsync() == 0)
            return size.Asin;

        await dropdownButton.ClickAsync();

        await page
            .Locator(".a-popover.a-dropdown")
            .WaitForAsync();

        var option = page.Locator(
            $"#native_dropdown_selected_size_name_{size.OptionIndex}");

        var previousUrl = page.Url;

        await option.ClickAsync();

        await page.WaitForFunctionAsync(
        @"previous => window.location.href !== previous",
        previousUrl);

        await page.WaitForTimeoutAsync(300);

        Console.WriteLine($"Current URL : {page.Url}");

        var currentAsin = string.Empty;

        var match = System.Text.RegularExpressions.Regex.Match(
            page.Url,
            @"/dp/([A-Z0-9]{10})");

        if (match.Success)
        {
            currentAsin = match.Groups[1].Value;
        }

        return currentAsin;
    }
}