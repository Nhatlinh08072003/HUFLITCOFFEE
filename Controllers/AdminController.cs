using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models;

namespace HUFLITCOFFEE.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;

   
    public AdminController(ILogger<AdminController> logger)
    {
        _logger = logger;
    }

    public IActionResult Overview()
    {
        return View();
    }
   
     public IActionResult Order()
    {
        return View();
    }
       public IActionResult Shipping()
    {
        return View();
    }
       public IActionResult Productlist()
    {
        return View();
    }
       public IActionResult Customer()
    {
        return View();
    }
       public IActionResult Staff()
    {
        return View();
    }
    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
