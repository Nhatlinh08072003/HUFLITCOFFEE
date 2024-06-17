using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models;
using HUFLITCOFFEE.Models.Main;

namespace HUFLITCOFFEE.Controllers;

public class AdminController : Controller
{
    private readonly HuflitcoffeeContext _huflitcoffeeContext;
    private readonly IHttpContextAccessor _httpContext;

    // Constructor
    public AdminController(HuflitcoffeeContext huflitcoffeeContext, IHttpContextAccessor httpContextAccessor)
    {
        _huflitcoffeeContext = huflitcoffeeContext;
        _httpContext = httpContextAccessor;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult AdminOrder()
    {
        return View();
    }
    public IActionResult AdminDelivery()
    {
        return View();
    }
    public IActionResult AdminProduct()
    {

        var products = _huflitcoffeeContext.Products.ToList();
        return View(products);
    }
    public IActionResult AdminCustomer()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
