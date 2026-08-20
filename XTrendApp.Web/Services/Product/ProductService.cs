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


    public async Task<IEnumerable<ProductListViewModel>> GetAllAsync()
    {
        return await _productRepository.GetListAsync();
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