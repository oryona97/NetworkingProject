using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using eBookStore.Repository;

namespace eBookStore.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly ILogger<UserRepository> _loggerUserRepo ;
    private readonly IConfiguration _configuration;
    private string? connectionString;

    private BookRepository _bookRepo;

    private UserRepository _UserRepository;
    public UserController(IConfiguration configuration ,ILogger<UserController> logger )
    {
        _configuration = configuration;
        connectionString = _configuration.GetConnectionString("DefaultConnection");
        _logger = logger;
        _loggerUserRepo = new Logger<UserRepository>(new LoggerFactory());
        _bookRepo = new BookRepository(connectionString);
    }

    public IActionResult changeBuingPriceAndUpdateBookAndHistoryBuingPrice(int bookId, float newPrice)
    {
        BookRepository bookRepo = new BookRepository(connectionString);
        bookRepo.addHistoryBookPriceModel(bookId, newPrice);
        return RedirectToAction("showBook","Home");
    }
    
    
    [HttpPost]
    [HttpGet]
    public async Task<IActionResult> SendNotifications()
    {
        Console.WriteLine("Sending notifications...");
        UserRepository userRepo = new UserRepository(connectionString, _loggerUserRepo);

        try
        {
            await userRepo.NotifyUsersOnBorrowedBooksAsync();
            Console.WriteLine("Notifications sent successfully!");
            ViewBag.Message = "Notifications sent successfully!";
        }
        catch (Exception ex)
        {
            ViewBag.Message = $"Error: {ex.Message}";
        }

        return View();
    }


    //this func is used to add discount to a book and update the book for admin
    [HttpPost]
    [HttpGet]
    public IActionResult AddDiscountAndUpdateBook(int bookId, float discountPercentage, DateTime saleEndDate)
    {
        try
        {
            UserRepository userRepo = new UserRepository(connectionString, _loggerUserRepo);

            userRepo.AddDiscountAndUpdateBook(bookId, discountPercentage, saleEndDate);
            Console.WriteLine($"Discount added and book updated successfully for bookId {bookId}.");
            return Json(new { message = "Discount added and book updated successfully", bookId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding discount for bookId {bookId}", bookId);
            Console.WriteLine($"Error: {ex.Message}");
            return Json(new { error = ex.Message });
        }
    }

    //handle jpg uoload(bookCovers)
    [HttpPost]
    public async Task<IActionResult> UploadCover(IFormFile coverImage)
    {
        if (coverImage == null || coverImage.Length == 0)
        {
            ModelState.AddModelError("coverImage", "Please upload a valid JPG image.");
            return View();
        }

        // Validate the file type
        var fileExtension = Path.GetExtension(coverImage.FileName).ToLower();
        if (fileExtension != ".jpg" && fileExtension != ".jpeg")
        {
            ModelState.AddModelError("coverImage", "Only JPG images are allowed.");
            return View();
        }

        // Define the path to save the file
        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/bookCovers");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, Path.GetFileName(coverImage.FileName));

        // Save the file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await coverImage.CopyToAsync(stream);
        }

        ViewBag.Message = "File uploaded successfully!";
        return View();
    }

   [HttpPost]
    public IActionResult CreateBook(AdminDashViewModel model)
    {

        // Save the model to the database or process as needed
        Console.WriteLine("Testetetet");

        _bookRepo.AddBookViewModel(model.bookViewModel);

        return View("adminDash",model); // Redirect to a success page
    }




    //Admin Dashboad view
    public IActionResult adminDash()
    {
        int? adminID = HttpContext.Session.GetInt32("userId");
        AdminDashViewModel data = new AdminDashViewModel();
        data.userModel = _bookRepo.getUserModelById(adminID.Value);
        data.bookViewModel= new BookViewModel();
        data.publishersList= _bookRepo.getAllPublishers();
        data.genreList = _bookRepo.getAllGenres();
        if(data.userModel.type=="admin")
        {
            return View(data);
        }
        return RedirectToAction("landingpage","Home");
    }


    //this func is for reset password

    
    public async Task<IActionResult> ResetPassword(string email)
    {
        UserRepository userRepo = new UserRepository(connectionString, _loggerUserRepo);
        var result = await userRepo.ResetPasswordAsync(email);
        if (result)
        {
            ViewBag.Message = "Password reset successfully. Check your email for the new password.";
        }
        else
        {
            ViewBag.Message = "Email not found.";
        }
        
        return View("ResetPassword");
    }

    [HttpGet]
    public IActionResult UserNotifications()
    {
        int? userId = HttpContext.Session.GetInt32("userId"); // Get the logged-in user's ID from Session

        if (userId == null)
        {
            return RedirectToAction("Login", "Auth"); // Redirect to login if no user is logged in
        }

        UserRepository userRepo = new UserRepository(connectionString, _loggerUserRepo);

        try
        {
            // Fetch notifications for the user
            var notifications = userRepo.GetUserNotifications(userId.Value);
            return View(notifications); // Pass the list of notifications to the View
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching notifications for userId {UserId}", userId);
            return View("Error"); // Redirect to an error page in case of failure
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
}
