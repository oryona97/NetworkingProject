using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Repository;

namespace eBookStore.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly ILogger<UserRepository> _loggerUserRepo ;
    private readonly IConfiguration _configuration;
    private string? connectionString;
    public UserController(IConfiguration configuration ,ILogger<UserController> logger )
    {
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
        _loggerUserRepo = new Logger<UserRepository>(new LoggerFactory());
        
    }

    public IActionResult changeBuingPriceAndUpdateBookAndHistoryBuingPrice(int bookId, float newPrice)
    {
        BookRepository bookRepo = new BookRepository(connectionString);
        bookRepo.addHistoryBookPriceModel(bookId, newPrice);
        return RedirectToAction("showBook","Home");
    }
    
    
    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> SendNotifications()
    {
        Console.WriteLine("Sending notifications...");
        UserRepository userRepo = new UserRepository(connectionString, _loggerUserRepo);

        try
        {
            await userRepo.NotifyUsersOnBorrowedBooksAsync();
            Console.WriteLine("Notifications sent successfully!");
            ViewBag.Message = "Notifications sent successfully!";
        }
        catch (Exception ex)
        {
            ViewBag.Message = $"Error: {ex.Message}";
        }

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
}
