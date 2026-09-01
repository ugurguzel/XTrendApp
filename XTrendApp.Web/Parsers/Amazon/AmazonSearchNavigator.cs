using Microsoft.Playwright;
using XTrendApp.Web.Models.Common;
using XTrendApp.Web.Selectors.Amazon;

namespace XTrendApp.Web.Parsers.Amazon;

public class AmazonSearchNavigator
{
    public async Task<bool> GoToNextPageAsync(IPage page)
    {
        try
        {
            var nextButton = page.Locator(
                AmazonSearchSelectors.NextPage);

            if (await nextButton.CountAsync() == 0)
            {
                Logger.Debug("Amazon next page button not found.");
                return false;
            }

            var isDisabled = await nextButton.IsDisabledAsync();

            if (isDisabled)
            {
                Logger.Debug("Amazon next page button is disabled.");
                return false;
            }

            var currentUrl = page.Url;

            await nextButton.ClickAsync();

            await page.WaitForURLAsync(
                url => url != currentUrl,
                new PageWaitForURLOptions
                {
                    Timeout = 15000
                });

            await page.Locator(
                    AmazonSearchSelectors.ProductCard)
                .First
                .WaitForAsync(
                    new LocatorWaitForOptions
                    {
                        State = WaitForSelectorState.Visible,
                        Timeout = 15000
                    });

            Logger.Debug(
                $"Amazon moved to next search page: {page.Url}");

            return true;
        }
        catch (TimeoutException ex)
        {
            Logger.Error(
                $"Amazon next page timeout: {ex.Message}");

            return false;
        }
        catch (Exception ex)
        {
            Logger.Error(
                $"Amazon next page error: {ex.Message}");

            return false;
        }
    }
}