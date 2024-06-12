using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models;

namespace HUFLITCOFFEE.Controllers;

public class ProductController : Controller
{
    private readonly ILogger<ProductController> _logger;


    public ProductController(ILogger<ProductController> logger)
    {
        _logger = logger;
    }

    public IActionResult Detail()
    {
        return View();
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
