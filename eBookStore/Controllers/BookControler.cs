using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Repositories;
using eBookStore.Models;

namespace eBookStore.Controllers;

public class BookController : Controller
{

    BookRepository bookRepo = new BookRepository();
    //create instance of bookViewModel and initialize all the models in it with values with the func in the model
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
                feedback = "This is a great book",
                createdAt = DateTime.Now
            },
            new FeedbackModel
            {
                id = 2,
                bookId = 9,
                feedback = "I love this book",
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
        userModel = bookRepo.getUserById(1),
        genreModel = bookRepo.getGenreById(1),
        ,
        authorModel = bookRepo.getAuthorById(1)
    };

    private readonly ILogger<BookController> _logger;

    public BookController(ILogger<BookController> logger)
    {
        _logger = logger;
    }

    public IActionResult AddBook()
    {
        
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
