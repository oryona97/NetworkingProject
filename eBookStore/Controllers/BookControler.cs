using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Repository;
using eBookStore.Models;
using eBookStore.Models.ViewModels;

namespace eBookStore.Controllers;

public class BookController : Controller
{
    private readonly ILogger<BookController> _logger;
	private readonly IConfiguration _configuration;
	private string? connectionString;
    BookRepository bookRepo  ;
    //create instance of bookViewModel and initialize all the models in it with values with the func in the model

    public  BookController(ILogger<BookController> logger, IConfiguration configuration )
    {
        _logger = logger;
        _configuration = configuration;
        this.connectionString = connectionString = _configuration.GetConnectionString("DefaultConnection");
        bookRepo = new BookRepository(connectionString);
    }
    private BookViewModel bookViewModel = new BookViewModel
    {
        book = new BookModel
        {
            publisherId = 1,
            genreId = 1,
            amountOfCopies = 3,
            title = "The Alchemist",
            borrowPrice = 2.5f,
            buyingPrice = 10.5f,
            pubDate = DateTime.Now,
            ageLimit = 18,
            priceHistory = 10,
            onSale = true,
            canBorrow = true,
            starRate = 4.5f,
            createdAt = DateTime.Now

        },
        rating = new RatingModel
        {
            starRating = 4,
            userId = 1,
            bookId = 9,
            createdAt = DateTime.Now
        },
        feedbackModel = new List<FeedbackModel>
        {
            new FeedbackModel
            {
                userId = 1,
                bookId = 9,
                comment = "This is a great book",
                createdAt = DateTime.Now
            },
            new FeedbackModel
            {
                id = 2,
                bookId = 9,
                comment = "I love this book",
                createdAt = DateTime.Now
            }
        },
        publisherModel = new PublisherModel
        {
            id = 6,
            name = "HarperCollins",
            createdAt = DateTime.Now
        },
        coverModel = new CoverModel
        {
            id = 1,
            bookId = 1,
            imgName = "TheAlchemist.jpg",
            createdAt = DateTime.Now
        },
        
        
    };



    public IActionResult AddBook()
    {
        return View(bookViewModel);
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
