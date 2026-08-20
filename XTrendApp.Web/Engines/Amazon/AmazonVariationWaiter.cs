using Microsoft.Playwright;

namespace XTrendApp.Web.Engines.Amazon;

public class AmazonVariationWaiter
{
    public async Task WaitForVariationAsync(
        IPage page,
        Func<Task> action)
    {
        await page.RunAndWaitForResponseAsync(
            async () =>
            {
                await action();
            },
            response =>
                response.Url.Contains("twisterDimensionSlotsDefault") &&
                response.Status == 200);
    }
}