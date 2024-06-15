using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models;
using HUFLITCOFFEE.Models.Main;

namespace HUFLITCOFFEE.Controllers;

public class ProductController : Controller
{
    private readonly HuflitcoffeeContext _huflitcoffeeContext;
    private readonly IHttpContextAccessor _httpContext;

    // Constructor
    public ProductController(HuflitcoffeeContext huflitcoffeeContext, IHttpContextAccessor httpContextAccessor)
    {
        _huflitcoffeeContext = huflitcoffeeContext;
        _httpContext = httpContextAccessor;
    }
    public IActionResult Index()
    {
        var products = _huflitcoffeeContext.Products.ToList();
        return View(products);
    }
    // Method to display product details
    [Route("detail/{id}")]
    public IActionResult Detail(int id)
    {
        var mainProduct = _huflitcoffeeContext.Products.FirstOrDefault(p => p.ProductId == id);
        if (mainProduct == null)
        {
            return NotFound();
        }

        // Lấy thêm các sản phẩm có ProductId là 73, 74 và 75
        var relatedProducts = _huflitcoffeeContext.Products
                                    .Where(p => p.ProductId == 73 || p.ProductId == 74 || p.ProductId == 75)
                                    .ToList();

        // Tạo một model chứa sản phẩm chính và các sản phẩm liên quan
        var model = new DetailViewModel
        {
            MainProduct = mainProduct,
            RelatedProducts = relatedProducts
        };

        return View(model); // Trả về view với danh sách sản phẩm
    }
    public IActionResult Cart()
    {
        return View();
    }
    public IActionResult Products()
    {
        return View();
    }
    public IActionResult Sale()
    {
        return View();
    }
    public IActionResult Shipping()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
