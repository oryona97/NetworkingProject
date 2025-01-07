using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using eBookStore.Repository;

namespace eBookStore.Controllers;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly ILogger<UserRepository> _loggerUserRepo;
    private readonly IConfiguration _configuration;
    private string? connectionString;

    private BookRepository _bookRepo;

    private UserRepository? _UserRepository;
    public UserController(IConfiguration configuration, ILogger<UserController> logger)
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
        return RedirectToAction("showBook", "Home");
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
    public async Task<IActionResult> UploadCover(IFormFile coverImage,string fullCoverName)
    {

        if (coverImage == null)
        {
            ModelState.AddModelError("coverImage", "Please upload a valid JPG image.");
            return View();
        }
        Console.WriteLine(coverImage.FileName);

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

        var filePath = Path.Combine(uploadPath,fullCoverName);
        Console.WriteLine("uploadPath: "+uploadPath);

        // Save the file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await coverImage.CopyToAsync(stream);
        }

        ViewBag.Message = "File uploaded successfully!";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateBook(AdminDashViewModel model,IFormFile coverImage)
    {

        // Save the model to the database or process as needed
        _bookRepo.AddBookViewModel(model.bookViewModel);
        await UploadCover(coverImage,_bookRepo.getCoverModelById(_bookRepo.getBookIDByName(model.bookViewModel.book.title)).imgName);

        return View("adminDash", model); // Redirect to a success page
    }




    //Admin Dashboad view
    public IActionResult adminDash()
    {
        int? adminID = HttpContext.Session.GetInt32("userId");
        AdminDashViewModel data = new AdminDashViewModel();
        data.userModel = _bookRepo.getUserModelById(adminID!.Value)!;
        data.bookViewModel = new BookViewModel();
        data.publishersList = _bookRepo.getAllPublishers();
        data.genreList = _bookRepo.getAllGenres();
        data.allUsers = _bookRepo.GetAllUserModels();
        if (data.userModel?.type == "admin")
        {
            return View(data);
        }
        return RedirectToAction("landingpage", "Home");
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


    public IActionResult DeleteBook(string title)
    {
        var bookTitle = _bookRepo.getBookIDByName(title);
        _bookRepo.DeleteBookViewModel(bookTitle);
        return RedirectToAction("adminDash");
    }
    public IActionResult Privacy()
    {
        return View();
    }

    //this func to add Discount
    [HttpPost]
    public IActionResult ApplyDiscount(string _title, float discountPercentage, DateTime saleEndDate)
    {
        var bookId = 0;
        Console.WriteLine("Title: {0}, Discount: {1}, Sale End Date: {2}", _title, discountPercentage, saleEndDate);
        try
        {
            var userRepo = new UserRepository(connectionString, _loggerUserRepo);
            bookId = _bookRepo.getBookIDByName(_title);
            userRepo.AddDiscountAndUpdateBook(bookId, discountPercentage, saleEndDate);
            TempData["Message"] = "Discount applied successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying discount for bookId {BookId}", bookId);
            TempData["Error"] = "An error occurred while applying the discount.";
        }

        return RedirectToAction("AdminDash");
    }


    //thif func to change the user password
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ChangePassword(string newPassword)
    {
        int? userId = HttpContext.Session.GetInt32("userId");
        if (userId == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            UserRepository userRepo = new UserRepository(connectionString, _loggerUserRepo);
            userRepo.ChangePassword(userId.Value, newPassword);
            ViewBag.Message = "Password changed successfully!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for userId {UserId}", userId);
            ViewBag.Message = "An error occurred while changing the password. Please try again.";
        }

        return View();
    }

    //this func to make User admin
    [HttpPost]
    public IActionResult MakeAdmin(int userId)
    {
        Console.WriteLine("User ID: {0}", userId);
        try
        {
            UserRepository userRepo = new UserRepository(connectionString, _loggerUserRepo);
            userRepo.UpdateUserToAdmin(userId); 
            TempData["SuccessMessage"] = "User has been successfully updated to Admin.";
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error updating user to admin: {ex.Message}");
            TempData["ErrorMessage"] = "Failed to update user to Admin.";
        }

        return RedirectToAction("adminDash");
    }

    [HttpPost]
    public IActionResult ToggleAdmin(int userId)
    {
        try
        {
            
            UserRepository userRepo = new UserRepository(connectionString, _loggerUserRepo);
            var user = _bookRepo.getUserModelById(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("adminDash");
            }

            if (user.type == "admin")
            {
                
                userRepo.UpdateUserType(userId, "user");
                TempData["SuccessMessage"] = $"User {user.username} is no longer an Admin.";
            }
            else
            {
         
                userRepo.UpdateUserType(userId, "admin");
                TempData["SuccessMessage"] = $"User {user.username} is now an Admin.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error toggling admin status for user {userId}: {ex.Message}");
            TempData["ErrorMessage"] = "An error occurred while updating the user.";
        }

        return RedirectToAction("adminDash");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
