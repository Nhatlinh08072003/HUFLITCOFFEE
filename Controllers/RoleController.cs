using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HUFLITCOFFEE.Models;

namespace HUFLITCOFFEE.Controllers;

public class RoleController : Controller
{
    private readonly ILogger<RoleController> _logger;


    public RoleController(ILogger<RoleController> logger)
    {
        _logger = logger;
    }

    public IActionResult Admin()
    {
        return View();
    }

    public IActionResult Customer()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
