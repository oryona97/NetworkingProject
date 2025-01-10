using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Repository;

namespace eBookStore.Controllers;

public class PersonalLibraryController : Controller
{
    private readonly ILogger<PersonalLibraryController> _logger;
    private readonly PersonalLibraryRepository _libraryRepo;
    private string? _connectionString;

    public PersonalLibraryController(
        IConfiguration configuration,
        ILogger<PersonalLibraryController> logger,
        ILoggerFactory loggerFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _connectionString = configuration.GetConnectionString("DefaultConnection");
        var repoLogger = loggerFactory.CreateLogger<PersonalLibraryRepository>();
        _libraryRepo = new PersonalLibraryRepository(_connectionString, repoLogger);
    }

    [Route("library")]
    [Route("PersonalLibrary")]
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
            TempData["Error"] = null;

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
            _logger.LogInformation("RemoveBook Action - userId: {UserId}", userId); // Changed the log message

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

    [HttpPost]
    public async Task<IActionResult> AddReview(int bookId, string comment)
    {
        try
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                TempData["Review_Error"] = "You need to be logged in to add a review. Please log in to continue.";
                TempData["ReturnUrl"] = $"/PersonalLibrary/Details/{bookId}";
                return RedirectToAction("Login", "Auth");
            }

            // Validate comment length
            if (string.IsNullOrWhiteSpace(comment) || comment.Length < 10 || comment.Length > 500)
            {
                TempData["Review_Error"] = "Review must be between 10 and 500 characters.";
                return RedirectToAction("Details", new { id = bookId });
            }

            // Verify user owns the book
            var userBooks = _libraryRepo.GetUserBooks(userId.Value);
            if (!userBooks.Any(ub => ub.book.id == bookId))
            {
                TempData["Review_Error"] = "You can only review books in your library.";
                return RedirectToAction("Details", new { id = bookId });
            }

            await _libraryRepo.AddReviewAsync(bookId, userId.Value, comment);
            TempData["Review_Success"] = "Your review has been added successfully.";

            return RedirectToAction("Details", new { id = bookId, anchor = "latest-review" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding review for book ID: {BookId}", bookId);
            TempData["Review_Error"] = "We couldn't add your review at this time. Please try again later.";
            return RedirectToAction("Details", new { id = bookId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteReview(int bookId, int reviewId)
    {
        try
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                TempData["Review_Error"] = "You need to be logged in to delete a review.";
                return RedirectToAction("Login", "Auth");
            }

            var success = await _libraryRepo.DeleteReviewAsync(bookId, userId.Value);
            if (success)
            {
                TempData["Review_Success"] = "Your review has been deleted successfully.";
            }
            else
            {
                TempData["Review_Error"] = "You don't have permission to delete this review.";
            }

            return RedirectToAction("Details", new { id = bookId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting review ID: {ReviewId} for book ID: {BookId}", reviewId, bookId);
            TempData["Error"] = "We couldn't delete your review at this time. Please try again later.";
            return RedirectToAction("Details", new { id = bookId });
        }
    }

    [HttpPost]
    public async Task<IActionResult> BorrowBook(int id)
    {
        try
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "You need to be logged in to borrow books. Please log in to continue.";
                TempData["ReturnUrl"] = $"/PersonalLibrary/Details/{id}";
                return RedirectToAction("Login", "Auth");
            }

            await _libraryRepo.BorrowBookAsync(userId.Value, id);
            TempData["Success"] = "Book has been borrowed successfully. Remember to return it within 30 days!";
            return RedirectToAction("Details", new { id = id });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid borrow attempt for book ID: {BookId}", id);
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", new { id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error borrowing book ID: {BookId}", id);
            TempData["Error"] = "We couldn't process your borrow request at this time. Please try again later.";
            return RedirectToAction("Details", new { id = id });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ReturnBook(int id)
    {
        try
        {
            int? userId = HttpContext.Session.GetInt32("userId");
            if (!userId.HasValue)
            {
                TempData["Error"] = "You need to be logged in to return books. Please log in to continue.";
                TempData["ReturnUrl"] = $"/PersonalLibrary/Details/{id}";
                return RedirectToAction("Login", "Auth");
            }

            await _libraryRepo.ReturnBookAsync(userId.Value, id);
            TempData["Success"] = "Book has been returned successfully. Thank you!";
            return RedirectToAction("Details", new { id = id });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid return attempt for book ID: {BookId}", id);
            TempData["Error"] = ex.Message;
            return RedirectToAction("Details", new { id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error returning book ID: {BookId}", id);
            TempData["Error"] = "We couldn't process your return request at this time. Please try again later.";
            return RedirectToAction("Details", new { id = id });
        }
    }

    public async Task<IActionResult> Details(int id)
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

            // Get borrowed books to check status
            var borrowedBooks = await _libraryRepo.GetBorrowedBooksAsync(userId.Value);
            ViewBag.IsBorrowed = borrowedBooks.Any(b => b.book.id == id);
            ViewBag.BorrowedBooks = borrowedBooks;

            return View(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving book details for ID: {BookId}", id);
            TempData["Error"] = "We couldn't load the book details at this time. Please try again later.";
            return RedirectToAction(nameof(Index));
        }
    }
}
