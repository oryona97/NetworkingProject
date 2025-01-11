using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using eBookStore.Repository;

namespace eBookStore.Controllers;

public class ShoppingCartController : Controller
{
  private readonly ILogger<ShoppingCartController> _logger;
  private readonly IConfiguration _configuration;
  private string? _connectionString;
  private ShoppingCartRepository _shoppingCartRepo;
  private BookRepository _booksRepo;
  private QueueRepository _queueRepo;
  private NotificationRepository _notificationRepo;

  public ShoppingCartController(IConfiguration configuration, ILogger<ShoppingCartController> logger)
  {
    _configuration = configuration;
    _connectionString = _configuration.GetConnectionString("DefaultConnection");
    _logger = logger;
    _shoppingCartRepo = new ShoppingCartRepository(_connectionString);
    _booksRepo = new BookRepository(_connectionString);
    _queueRepo = new QueueRepository(_connectionString);
    _notificationRepo = new NotificationRepository(_connectionString);
  }

  [Route("cart")]
  [Route("ShoppingCart")]
  public IActionResult Index()
  {
    ViewData["StripePublicKey"] = _configuration["Stripe:PublicKey"];
    try
    {
      int? nullableUserId = HttpContext.Session.GetInt32("userId");
      if (nullableUserId.HasValue)
      {
        var cart = _shoppingCartRepo.GetShoppingCart(nullableUserId.Value);
        if (cart == null || cart.shoppingCart == null)
        {
          cart = new ShoppingCartViewModel { shoppingCart = new ShoppingCartModel() };
        }
        var books = cart.shoppingCart.Books;
        var bookViews = new List<BookViewModel>();
        foreach (var v in books)
        {
          var bookView = _booksRepo.getBookById(v.bookId);
          if (bookView != null)
          {
            bookViews.Add(bookView);
          }
        }
        return View("Index", bookViews);
      }
      else
      {
        return RedirectToAction("Login", "Auth");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to load shopping cart");
      return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }

  [HttpPost]
  public async Task<IActionResult> AddToShoppingCart([FromBody] AddToCartModel model)
  {
    try
    {
      int? currentUser = HttpContext.Session.GetInt32("userId");
      if (!currentUser.HasValue)
      {
        return Json(new { success = false, message = "Please log in to continue" });
      }

      var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
      var repoLogger = loggerFactory.CreateLogger<PersonalLibraryRepository>();
      var libRepo = new PersonalLibraryRepository(_connectionString, repoLogger);

      var borrowedBooks = await libRepo.GetBorrowedBooksAsync(currentUser.Value);
      if (borrowedBooks.Any(b => b.book.id == model.BookId))
      {
        return Json(new { success = false, message = "You have already borrowed this book." });
      }

      var userBooks = libRepo.GetUserBooks(currentUser.Value);
      if (userBooks.Any(b => b.book.id == model.BookId))
      {
        return Json(new { success = false, message = "You already own this book." });
      }

      if (model.IsBorrowed)
      {
        const int MAX_BORROWED_BOOKS = 3;

        // Get current book to check copies
        var book = _booksRepo.getBookById(model.BookId);
        if (book == null)
        {
          return Json(new { success = false, message = "Book not found." });
        }

        if (borrowedBooks.Count() >= MAX_BORROWED_BOOKS)
        {
          var queuePosition = await AddToQueueAndGetPosition(currentUser.Value, model.BookId);
          return Json(new
          {
            success = false,
            message = $"You have already borrowed {MAX_BORROWED_BOOKS} books. You are now number {queuePosition} in the queue."
          });
        }

        // Check if copies are available
        if (book.book.amountOfCopies <= 0)
        {
          var queuePosition = await AddToQueueAndGetPosition(currentUser.Value, model.BookId);
          return Json(new
          {
            success = false,
            message = $"No copies available. You are now number {queuePosition} in the queue."
          });
        }
      }

      _shoppingCartRepo.AddToShoppingCart(
          currentUser.Value,
          model.BookId,
          model.Format
      );

      return Json(new { success = true, message = "Item added to cart successfully" });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to add to shopping cart");
      return Json(new { success = false, message = "Failed to add item to cart" });
    }
  }

  private async Task NotifyNextUserInQueue(int bookId)
  {
    try
    {
      var queue = await _queueRepo.GetQueue(bookId);
      if (queue.Any())
      {
        var nextUser = queue.OrderBy(q => q.createdAt).First();
        var book = _booksRepo.getBookById(bookId);

        if (book != null)
        {
          var notification = new UserNotificationModel
          {
            userId = nextUser.userId,
            Message = $"Good news! The book '{book.book.title}' is now available for borrowing. You can add it to your cart.",
            CreatedAt = DateTime.UtcNow
          };

          await _notificationRepo.AddAsync(notification);
          _logger.LogInformation($"Notification sent to user {nextUser.userId} about book {bookId} availability");
        }
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, $"Error notifying next user in queue for book {bookId}");
    }
  }

  private async Task<int> AddToQueueAndGetPosition(int userId, int bookId)
  {
    await _queueRepo.AddAsync(new BookRentQueueModel
    {
      bookId = bookId,
      userId = userId,
      createdAt = DateTime.UtcNow
    });

    var position = await _queueRepo.GetQueuePosition(userId, bookId);

    if (position.HasValue)
    {
      var book = _booksRepo.getBookById(bookId);
      if (book != null)
      {
        var message = position.Value == 1
            ? $"You are next in line for '{book.book.title}'. We'll notify you when it becomes available."
            : $"You are position {position.Value} in line for '{book.book.title}'. We'll notify you when it becomes available.";

        await _notificationRepo.AddAsync(new UserNotificationModel
        {
          userId = userId,
          Message = message,
          CreatedAt = DateTime.UtcNow
        });
      }
    }

    return position ?? 0;
  }

  [HttpPost]
  public async Task<IActionResult> RemoveFromShoppingCart([FromBody] RemoveFromCartModel model)
  {
    try
    {
      int? currentUser = HttpContext.Session.GetInt32("userId");
      if (!currentUser.HasValue)
      {
        return Json(new { success = false, message = "Please log in to continue" });
      }

      var cart = _shoppingCartRepo.GetShoppingCart(currentUser.Value);
      bool found = false;
      bool isBorrowed = false;

      foreach (var book in cart.shoppingCart.Books)
      {
        if (model.BookId == book.bookId)
        {
          found = true;
          break;
        }
      }

      if (!found)
      {
        return Json(new { success = false, message = "Book does not exist in your cart buddy." });
      }

      _shoppingCartRepo.RemoveOneFromShoppingCart(
          currentUser.Value,
          model.BookId
      );

      if (isBorrowed)
      {
        // Update book's copy count
        var book = _booksRepo.getBookById(model.BookId);
        if (book != null)
        {
          _booksRepo.updateAmountOfCopies(model.BookId, book.book.amountOfCopies + 1);
        }

        // Remove from queue if user was in it
        await _queueRepo.DeleteAsync(currentUser.Value, model.BookId);

        // Notify next user in queue
        await NotifyNextUserInQueue(model.BookId);
      }

      return Json(new
      {
        success = true,
        message = $"Item removed from cart successfully"
      });
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to remove from shopping cart");
      return Json(new
      {
        success = false,
        message = "Failed to remove item from cart"
      });
    }
  }

  public IActionResult ClearShoppingCart(int? userId)
  {
    try
    {
      int? nullableUserId = HttpContext.Session.GetInt32("userId");
      if (nullableUserId.HasValue)
      {
        _shoppingCartRepo.RemoveAllFromShoppingCart(nullableUserId.Value);
        return View("ShowShoppingCart", _shoppingCartRepo.GetShoppingCart(nullableUserId.Value));
      }
      else
      {
        return RedirectToAction("auth/login");
      }
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Failed to clear shopping cart");
      return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }

  public class AddToCartModel
  {
    public int BookId { get; set; }
    public string Format { get; set; } = "pdf";
    public bool IsBorrowed { get; set; }
  }

  public class RemoveFromCartModel
  {
    public int BookId { get; set; }
  }
}
