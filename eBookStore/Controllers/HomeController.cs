using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using eBookStore.Repository;
namespace eBookStore.Controllers;


public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private string? connectionString;
    BookRepository _bookRepo;
    private ShoppingCartRepository shoppingCartRepo;

    public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
    {
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
        _bookRepo = new BookRepository(connectionString);
        shoppingCartRepo = new ShoppingCartRepository(connectionString);
    }
    public IActionResult showBook()
    {
        Console.WriteLine("showBook");
        List<BookViewModel> books = _bookRepo.getAllBooks();

        return View(books);
    }
    public IActionResult showBookBySearch(string? title, int? publisherId, int? genreId, float? minPrice, float? maxPrice, DateTime? fromDate, DateTime? toDate)
    {
        List<BookViewModel> books = _bookRepo.SearchBooks(title, publisherId, genreId, minPrice, maxPrice, fromDate, toDate);
        return View("showBook", books);
    }

    public IActionResult SearchForm()
    {
        return View("SearchBooks");
    }

    public IActionResult Profile()
    {
        try
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "You need to be logged in to access your Account Overview. Please log in to continue.";
                TempData["ReturnUrl"] = "/accountOverview";
                return RedirectToAction("Login", "Auth");
            }
            TempData["Error"] = null;


            UserModel userInfo = _bookRepo.getUserModelById(userId.Value)!;

            return View(userInfo);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error showing user: @userid");
            throw;
        }
    }

    public IActionResult landingPage()
    {

        LandingPageViewModel info = new LandingPageViewModel();
        info.allBooks = _bookRepo.getAllBooks();
        info.SpecialSales = _bookRepo.getAllBooks();
        info.listOfCategorys = _bookRepo.getAllGenres();

        return View(info);

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

    public IActionResult Logout()
    {
        HttpContext.Session.Clear(); // Clear the session
        return RedirectToAction("landingPage");
    }
}
