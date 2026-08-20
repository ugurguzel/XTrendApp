using Microsoft.AspNetCore.Mvc;
using XTrendApp.Web.Services.Product;

namespace XTrendApp.Web.Controllers;

public class ProductController : Controller
{
    private readonly ProductService _productService;

    public ProductController(ProductService productService)
    {
        _productService = productService;
    }


    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllAsync();

        return View(products);
    }

    public async Task<IActionResult> Detail(long id)
    {
        var product = await _productService.GetDetailAsync(id);

        if (product == null)
            return NotFound();

        return View(product);
    }
}