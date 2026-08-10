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
}