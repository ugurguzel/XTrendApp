    using Microsoft.Playwright;
using System.Buffers.Text;
using XTrendApp.Web.Connectors.Amazon;
using XTrendApp.Web.Models.Amazon;

namespace XTrendApp.Web.Engines.Amazon
{
    public class AmazonVariationEngine
    {
        private readonly AmazonDropdownParser _dropdownParser;
        private readonly AmazonButtonParser _buttonParser;
        private readonly AmazonPriceEngine _priceEngine;

        public AmazonVariationEngine(
            AmazonDropdownParser dropdownParser,
            AmazonButtonParser buttonParser,
            AmazonPriceEngine priceEngine)
        {
            _dropdownParser = dropdownParser;
            _buttonParser = buttonParser;
            _priceEngine = priceEngine;
        }

        public async Task<AmazonVariationResult> ParseAsync(
    IPage page,
    AmazonDetailModel detail,
    string baseUrl,
    AmazonMarket market)
        {
            var result = new AmazonVariationResult
            {
                ParentAsin = detail.Asin
            };

            var root = page.Locator("#twister-plus-inline-twister-card");

            if (await root.CountAsync() == 0)
            {
                return result;
            }

            //--------------------------------------------------
            // DROPDOWN or BUTTON
            //--------------------------------------------------

            result.Sizes.Clear();

            var dropdown = root.Locator("#native_dropdown_selected_size_name");

            

            if (await dropdown.CountAsync() > 0)
            {
                


                await _dropdownParser.ParseAsync(root, result);
            }
            else
            {
                await _buttonParser.ParseAsync(root, result);
            }

            await _priceEngine.ParseAsync(
    page,
    result,
    baseUrl,
    market);

            return result;
        }
    }
}