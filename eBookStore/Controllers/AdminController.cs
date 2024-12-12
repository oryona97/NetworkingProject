using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;

namespace eBookStore.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly IConfiguration _configuration;
    private string? connectionString;
    public AdminController(IConfiguration configuration ,ILogger<AdminController> logger )
    {
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }
    

    public IActionResult Index()
    {
        return View();
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

    public void addBook(BookModel book)
    {

    }
}
