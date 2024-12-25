using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using eBookStore.Repository;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace eBookStore.Controllers;

public class PersonalLibraryController : Controller
{
	private readonly ILogger<PersonalLibraryController> _logger;
	private readonly PersonalLibraryRepository _libraryRepo;

	public PersonalLibraryController(
		IConfiguration configuration,
		ILogger<PersonalLibraryController> logger,
		ILoggerFactory loggerFactory)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		var connectionString = configuration.GetConnectionString("DefaultConnection");
		var repoLogger = loggerFactory.CreateLogger<PersonalLibraryRepository>();
		_libraryRepo = new PersonalLibraryRepository(connectionString, repoLogger);
	}

	// GET: PersonalLibrary
	public IActionResult Index()
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				TempData["Error"] = "You need to be logged in to access your personal library. Please log in to continue.";
				TempData["ReturnUrl"] = "/PersonalLibrary";
				return RedirectToAction("Login", "Auth");
			}

			var userBooks = _libraryRepo.GetUserBooks(userId.Value);

			if (TempData["Success"] == null && userBooks.Count == 0)
			{
				TempData["Info"] = "Your library is currently empty. Browse our collection and add some books to get started!";
			}

			return View(userBooks);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving personal library");
			TempData["Error"] = "We encountered an issue while loading your library. Our team has been notified. Please try again later.";
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	// GET: PersonalLibrary/Details/5
	public IActionResult Details(int id)
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				TempData["Error"] = "You need to be logged in to view book details. Please log in to continue.";
				TempData["ReturnUrl"] = $"/PersonalLibrary/Details/{id}";
				return RedirectToAction("Login", "Auth");
			}

			var book = _libraryRepo.GetBookDetails(id, userId.Value);
			if (book == null)
			{
				TempData["Warning"] = "The requested book was not found in your library. It may have been removed or you might not have access to it.";
				return RedirectToAction(nameof(Index));
			}

			return View(book);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving book details for ID: {BookId}", id);
			TempData["Error"] = "We couldn't load the book details at this time. Please try again later.";
			return RedirectToAction(nameof(Index));
		}
	}

	// POST: PersonalLibrary/AddBook/5
	[HttpPost]
	public IActionResult AddBook(int id)
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				TempData["Error"] = "You need to be logged in to add books to your library. Please log in to continue.";
				TempData["ReturnUrl"] = $"/PersonalLibrary/AddBook/{id}";
				return RedirectToAction("Login", "Auth");
			}

			_libraryRepo.AddBookToLibrary(id, userId.Value);
			TempData["Success"] = "Great choice! The book has been added to your personal library.";
			return RedirectToAction(nameof(Index));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding book ID: {BookId} to library", id);
			TempData["Error"] = "We couldn't add this book to your library right now. Please try again later.";
			return RedirectToAction(nameof(Index));
		}
	}

	// POST: PersonalLibrary/RemoveBook/5
	[HttpPost]
	public IActionResult RemoveBook(int id)
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				TempData["Error"] = "You need to be logged in to manage your library. Please log in to continue.";
				TempData["ReturnUrl"] = "/PersonalLibrary";
				return RedirectToAction("Login", "Auth");
			}

			_libraryRepo.RemoveBookFromLibrary(id, userId.Value);
			TempData["Success"] = "The book has been removed from your library successfully.";
			return RedirectToAction(nameof(Index));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error removing book ID: {BookId} from library", id);
			TempData["Error"] = "We couldn't remove this book from your library right now. Please try again later.";
			return RedirectToAction(nameof(Index));
		}
	}

	// GET: PersonalLibrary/Search
	public IActionResult Search(string? title, int? genreId, DateTime? fromDate, DateTime? toDate)
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				TempData["Error"] = "You need to be logged in to search your library. Please log in to continue.";
				TempData["ReturnUrl"] = "/PersonalLibrary/Search";
				return RedirectToAction("Login", "Auth");
			}

			var books = _libraryRepo.GetUserBooks(userId.Value);

			// Apply filters
			if (!string.IsNullOrEmpty(title))
			{
				books = books.Where(b => b.book.title?.Contains(title, StringComparison.OrdinalIgnoreCase) ?? false).ToList();
			}
			if (genreId.HasValue)
			{
				books = books.Where(b => b.book.genreId == genreId.Value).ToList();
			}
			if (fromDate.HasValue)
			{
				books = books.Where(b => b.book.pubDate >= fromDate.Value).ToList();
			}
			if (toDate.HasValue)
			{
				books = books.Where(b => b.book.pubDate <= toDate.Value).ToList();
			}

			if (books.Count == 0)
			{
				TempData["Info"] = "No books found matching your search criteria. Try adjusting your filters.";
			}

			return View("Index", books);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error searching library");
			TempData["Error"] = "We encountered an issue while searching your library. Please try again later.";
			return RedirectToAction(nameof(Index));
		}
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
