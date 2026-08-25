using XTrendApp.Web.Models.Product;
using XTrendApp.Web.Repositories.Product;

namespace XTrendApp.Web.Services.Product;

public class ProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(
        IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }


    public async Task<IEnumerable<ProductListViewModel>> GetAllAsync(
    string sortBy = "LastScan",
    string sortDirection = "desc")
    {
        var products = (await _productRepository.GetListAsync()).ToList();

        var descending =
            sortDirection.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase);

        products = sortBy.ToLowerInvariant() switch
        {
            "product" =>
                descending
                    ? products.OrderByDescending(x => x.Name).ToList()
                    : products.OrderBy(x => x.Name).ToList(),

            "brand" =>
                descending
                    ? products.OrderByDescending(x => x.Brand).ToList()
                    : products.OrderBy(x => x.Brand).ToList(),

            "marketplace" =>
                descending
                    ? products.OrderByDescending(x => x.Source).ToList()
                    : products.OrderBy(x => x.Source).ToList(),

            "variations" =>
                descending
                    ? products.OrderByDescending(x => x.VariationCount).ToList()
                    : products.OrderBy(x => x.VariationCount).ToList(),

            "rating" =>
                descending
                    ? products.OrderByDescending(x => x.Rating).ToList()
                    : products.OrderBy(x => x.Rating).ToList(),

            "reviews" =>
                descending
                    ? products.OrderByDescending(x => x.ReviewCount).ToList()
                    : products.OrderBy(x => x.ReviewCount).ToList(),

            "lastscan" =>
                descending
                    ? products.OrderByDescending(x => x.LastCapturedAt).ToList()
                    : products.OrderBy(x => x.LastCapturedAt).ToList(),

            _ =>
                products.OrderByDescending(x => x.LastCapturedAt).ToList()
        };

        return products;
    }

    public async Task<ProductDetailViewModel?> GetDetailAsync(
    long productId)
    {
        var detail = await _productRepository.GetDetailAsync(productId);

        if (detail == null)
            return null;

        detail.Attributes =
            await _productRepository.GetAttributesAsync(productId);

        detail.Variations =
            await _productRepository.GetVariationsAsync(productId);

        detail.ReviewHistory =
            await _productRepository.GetReviewHistoryAsync(productId);

        return detail;
    }
}