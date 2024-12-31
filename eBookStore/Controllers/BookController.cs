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
        connectionString = _configuration.GetConnectionString("DefaultConnection");
        bookRepo = new BookRepository(connectionString);
    }


    //texting model
    private BookViewModel bookViewModel = new BookViewModel
    {
        book = new BookModel
        {
            publisherId = 2,
            genreId = 1,
            amountOfCopies = 3,
            title = "Dor the gibor",
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
            userId = 3,
            bookId = 2,
            createdAt = DateTime.Now
        },
        feedbackModel = new List<FeedbackModel>
        {
            new FeedbackModel
            {
                userId = 5,
                bookId = 1,
                comment = "Dor is the best",
                createdAt = DateTime.Now
            },
            new FeedbackModel
            {
                userId = 2,
                bookId = 9,
                comment = "Dor is the man",
                createdAt = DateTime.Now
            }
        },
        publisherModel = new PublisherModel
        {
            
            name = "HarperCollinsasd",
            createdAt = DateTime.Now
        },
        coverModel = new CoverModel
        {
            
            bookId = 1,
            imgName = "Dorthegiboer.jpg",
            createdAt = DateTime.Now
        },
        genreModel = new GenreModel
        {
          
            name = "Adventure",
            createdAt = DateTime.Now
        },
        
        authorModel = new AuthorModel
        {
            bookId = 50,
            name = "Dor",
            createdAt = DateTime.Now
        },
        
    };

    


    //this func adds feedback of the book
    public void AddFeedback(FeedbackModel feedbackModel)
    {
        Console.WriteLine("Feedback Added");
        bookRepo.AddFeedback(feedbackModel);
        Console.WriteLine("Feedback Added");
    }



    //this func adds Rating of the book
    public void AddRating(RatingModel ratingModel)
    {
        Console.WriteLine("Rating Added");
        bookRepo.AddRating(ratingModel);
        Console.WriteLine("Rating Added");
    }
    public void AddBookModel(BookModel bookModel)
    {
        Console.WriteLine("Book Added");
        bookRepo.AddBook(bookModel);
        Console.WriteLine("Book Added");
    }

    //this func adds the cover of the book
    public void AddCoverModel(CoverModel coverModel)
    {
        Console.WriteLine("Cover Added");
        bookRepo.AddCoverModel(coverModel);
        Console.WriteLine("Cover Added");
    }

    //this func adds the genre of the book
    public void AddGenreModel(GenreModel genreModel)
    {
        Console.WriteLine("Genre Added");
        bookRepo.AddGenreModel(genreModel);
        Console.WriteLine("Genre Added");
    }


    
    [Route("Book/AddBook")]
    public void AddBook(BookViewModel bookViewModel)
    {
        Console.WriteLine("Book Added");
        bookRepo.AddBookViewModel(bookViewModel);
        Console.WriteLine("Book Added");
    }

    
    //this func return all Genres name that exsist in db
    public List<string> GetAllGenres()
    {
        Console.WriteLine("Genres Returned");
        return bookRepo.getAllGenres();
    }
    
    
    //this func return all book can be borrowed
    public List<BookViewModel> GetAllBorrowableBooks()
    {
        Console.WriteLine("Borrowable Books");
        return bookRepo.getBorrowableBooks();
    }

    //this func update amount of copies of the book before borrowing
    public void UpdateAmountOfCopies(int bookId)
    {
        Console.WriteLine("Amount of Copies Updated");
        bookRepo.checkAmountOfCopies(bookId);
        Console.WriteLine("Amount of Copies Updated");
    }


    //this func update amount of copies of the book after returning
    public void UpdateAmountOfCopiesAfterReturn(int bookId)
    {
        Console.WriteLine("Amount of Copies Updated");
        bookRepo.returnRentedBook(bookId);
        Console.WriteLine("Amount of Copies Updated");
    }
    
    
    //Book/Index/1
    public IActionResult Index(int id)
    {
       
        BookViewModel bookViewModel = bookRepo.getBookById(id);
        Console.WriteLine(bookViewModel.authorModel+" No author found");
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
