using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models;

namespace HUFLITCOFFEE.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Caffee()
    {
        return View();
    }
     public IActionResult Product()
    {
        return View();
    }
     public IActionResult Share()
    {
        return View();
    }
     public IActionResult Recruitment()
    {
        return View();
    }
    public IActionResult Aboutus()
    {
        return View();
    }
     public IActionResult Terms()
    {
        return View();
    }
     public IActionResult Policy()
    {
        return View();
    }
     public IActionResult Roots()
    {
        return View();
    }
     public IActionResult Translation()
    {
        return View();
    }
     public IActionResult Job()
    {
        return View();
    }
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
