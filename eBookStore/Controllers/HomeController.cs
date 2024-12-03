using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;

namespace eBookStore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult LogIn()
    {

        return View();
    }

    public IActionResult showLogin(AdminModel admin)
    {

        return View(admin);
    }

    [HttpPost]
    public IActionResult LogIn(AdminModel admin)
    {
        
        return View(admin);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
