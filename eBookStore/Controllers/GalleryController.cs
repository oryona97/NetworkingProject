using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using eBookStore.Repository;


namespace eBookStore.Controllers;

public class GalleryController : Controller
{

	private readonly ILogger<HomeController> _logger;
	private readonly IConfiguration _configuration;
	private string? connectionString;
	private BookRepository _bookRepo;
	private UserRepository _userRepo;

	public GalleryController(IConfiguration configuration, ILogger<HomeController> logger)
	{
		_configuration = configuration;
		connectionString = _configuration.GetConnectionString("DefaultConnection");
		_logger = logger;
		_bookRepo = new BookRepository(connectionString);
	}



    public IActionResult Index()
    {
        GalleryPageViewModel data = new GalleryPageViewModel();

        data.allBooks = _bookRepo.getAllBooks();
        data.listOfCategorys = _bookRepo.getAllGenres();

        return View(data);
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
