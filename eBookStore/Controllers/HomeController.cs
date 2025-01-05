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

	private GeneralFeedbackModelRepository _generalFeedbackRepo;

	public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
	{
		_configuration = configuration;
		connectionString = _configuration.GetConnectionString("DefaultConnection");
		_logger = logger;
		_bookRepo = new BookRepository(connectionString);
		shoppingCartRepo = new ShoppingCartRepository(connectionString);
		_generalFeedbackRepo = new GeneralFeedbackModelRepository(connectionString);
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

	public async Task<IActionResult> AddReview(string comment)
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				TempData["Review_Error"] = "You need to be logged in to add a review. Please log in to continue.";
				return RedirectToAction("Login", "Auth");
			}

			// Validate comment length
			if (string.IsNullOrWhiteSpace(comment) || comment.Length < 10 || comment.Length > 500)
			{
				TempData["Review_Error"] = "Review must be between 10 and 500 characters.";
				return RedirectToAction("landingPage");
			}

			// Verify user owns the book
			if (userId==0)
			{
				TempData["Review_Error"] = "You can only review books in your library.";
				return RedirectToAction("landingPage");
			}

			GeneralFeedbackModel feedback = new GeneralFeedbackModel();
			feedback.userId = userId ?? 0;
			feedback.comment=comment;
			_generalFeedbackRepo.AddGeneralFeedback(feedback);
			TempData["Review_Success"] = "Your review has been added successfully.";

			return RedirectToAction("landingPage");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding review for website ID: {userId}");
			TempData["Review_Error"] = "We couldn't add your review at this time. Please try again later.";
			return RedirectToAction("landingPage");
		}
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
