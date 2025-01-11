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

    public ShoppingCartController(IConfiguration configuration, ILogger<ShoppingCartController> logger)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
        _shoppingCartRepo = new ShoppingCartRepository(_connectionString);
        _booksRepo = new BookRepository(_connectionString);
        _queueRepo = new QueueRepository(_connectionString);
    }

    [Route("cart")]
    [Route("ShoppingCart")]
    //this method is used to show the shopping cart
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

            if (model.IsBorrowed && borrowedBooks.Count() >= 3)
            {
                await _queueRepo.AddAsync(new BookRentQueueModel
                {
                    bookId = model.BookId,
                    userId = currentUser.Value
                });

                return Json(new { success = false, message = "You have already borrowed 3 books, you have enetered to the queue and will be notified when the book is available" });
            }

            if ((await _queueRepo.GetQueue(model.BookId)).Count() >= 3)
            {
                await _queueRepo.AddAsync(new BookRentQueueModel
                {
                    bookId = model.BookId,
                    userId = currentUser.Value
                });

                return Json(new { success = false, message = "Books can be borrowed only by 3 people at a time, you have now entered to the waiting list and will be notified when available" });
            }

            foreach (var book in borrowedBooks)
            {
                if (model.BookId == book.book.id)
                {
                    return Json(new { success = false, message = "Book is already in your cart." });
                }
            }

            var books = libRepo.GetUserBooks(currentUser.Value);
            foreach (var book in books)
            {
                if (book.book.id == model.BookId)
                {
                    return Json(new { success = false, message = "Book is already in your library." });
                }
            }

            var userBooks = libRepo.GetUserBooks(currentUser.Value);
            _shoppingCartRepo.AddToShoppingCart(
                currentUser.Value,
                model.BookId,
                model.Format
            );

            return Json(new
            {
                success = true,
                message = $"Item added to cart successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add to shopping cart");
            return Json(new
            {
                success = false,
                message = "Failed to add item to cart"
            });
        }
    }

    //this method is used to remove a book from the shopping cart
    [HttpPost]
    public IActionResult RemoveFromShoppingCart([FromBody] RemoveFromCartModel model)
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
            foreach (var book in cart.shoppingCart.Books)
            {
                if (model.BookId == book.bookId)
                    found = true;
            }
            if (!found)
            {
                return Json(new { success = false, message = "Book does not exist in your cart buddy." });
            }

            _shoppingCartRepo.RemoveOneFromShoppingCart(
                currentUser.Value,
                model.BookId
            );

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

    //this method is used to clear the shopping cart
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
