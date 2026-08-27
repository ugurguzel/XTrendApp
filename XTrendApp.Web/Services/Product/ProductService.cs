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


    public async Task<ProductListPageViewModel> GetAllAsync(
        string sortBy = "LastScan",
        string sortDirection = "desc",
        int page = 1,
        int pageSize = 25)
    {
        var products =
            (await _productRepository.GetListAsync())
            .ToList();


        // ---------------------------------------------------------
        // SORTING
        // ---------------------------------------------------------

        var descending =
            sortDirection.Equals(
                "desc",
                StringComparison.OrdinalIgnoreCase);


        products = sortBy.ToLowerInvariant() switch
        {
            "product" =>
                descending
                    ? products
                        .OrderByDescending(x => x.Name)
                        .ToList()
                    : products
                        .OrderBy(x => x.Name)
                        .ToList(),

            "brand" =>
                descending
                    ? products
                        .OrderByDescending(x => x.Brand)
                        .ToList()
                    : products
                        .OrderBy(x => x.Brand)
                        .ToList(),

            "marketplace" =>
                descending
                    ? products
                        .OrderByDescending(x => x.Source)
                        .ToList()
                    : products
                        .OrderBy(x => x.Source)
                        .ToList(),

            "variations" =>
                descending
                    ? products
                        .OrderByDescending(x => x.VariationCount)
                        .ToList()
                    : products
                        .OrderBy(x => x.VariationCount)
                        .ToList(),

            "rating" =>
                descending
                    ? products
                        .OrderByDescending(x => x.Rating)
                        .ToList()
                    : products
                        .OrderBy(x => x.Rating)
                        .ToList(),

            "reviews" =>
                descending
                    ? products
                        .OrderByDescending(x => x.ReviewCount)
                        .ToList()
                    : products
                        .OrderBy(x => x.ReviewCount)
                        .ToList(),

            "lastscan" =>
                descending
                    ? products
                        .OrderByDescending(x => x.LastCapturedAt)
                        .ToList()
                    : products
                        .OrderBy(x => x.LastCapturedAt)
                        .ToList(),

            _ =>
                products
                    .OrderByDescending(x => x.LastCapturedAt)
                    .ToList()
        };


        // ---------------------------------------------------------
        // PAGINATION
        // ---------------------------------------------------------

        var totalCount = products.Count;


        if (pageSize <= 0)
            pageSize = 25;


        if (page < 1)
            page = 1;


        var totalPages =
            Math.Max(
                1,
                (int)Math.Ceiling(
                    totalCount / (double)pageSize));


        if (page > totalPages)
            page = totalPages;


        var pagedProducts =
            products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


        return new ProductListPageViewModel
        {
            Products = pagedProducts,

            TotalCount = totalCount,

            Page = page,

            PageSize = pageSize,

            TotalPages = totalPages
        };
    }


    public async Task<ProductDetailViewModel?> GetDetailAsync(
        long productId)
    {
        var detail =
            await _productRepository.GetDetailAsync(productId);


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