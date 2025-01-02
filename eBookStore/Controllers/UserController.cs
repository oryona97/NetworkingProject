using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Repository;

namespace eBookStore.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _configuration;
    private string? connectionString;
    public UserController(IConfiguration configuration ,ILogger<UserController> logger )
    {
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
    }

    public IActionResult changeBuingPriceAndUpdateBookAndHistoryBuingPrice(int bookId, float newPrice)
    {
        BookRepository bookRepo = new BookRepository(connectionString);
        bookRepo.addHistoryBookPriceModel(bookId, newPrice);
        return RedirectToAction("showBook","Home");
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
