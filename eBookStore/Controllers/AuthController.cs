using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Repository;
namespace eBookStore.Controllers;


public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly IConfiguration _configuration;
    private string? _connectionString;
    private UserRepository _userRepo;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
        _logger = logger;

        // Create a logger for UserRepository using ILoggerFactory
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var userRepoLogger = loggerFactory.CreateLogger<UserRepository>();

        _userRepo = new UserRepository(_connectionString, userRepoLogger);
    }

    // Register Actions
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    public IActionResult register(RegisterViewModel model)
    {
        try
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string queries = @"INSERT INTO [User] 
                    (Username, Password, Email, Type ,FirstName, LastName, PhoneNumber,createdAt)  
                    VALUES (@Username, @Password, @Email, @Type , @FirstName, @LastName, @PhoneNumber, @createdAt);";

                    using (SqlCommand command = new SqlCommand(queries, connection))
                    {
                        command.Parameters.AddWithValue("@Username", model.Username);
                        command.Parameters.AddWithValue("@Password", _userRepo.HashPassword(model.Password));
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
                        command.Parameters.AddWithValue("@LastName", model.LastName);
                        command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                        command.Parameters.AddWithValue("@createdAt", DateTime.Now);
                        command.Parameters.AddWithValue("@Type", "user");
                        

                        int rowAffect = command.ExecuteNonQuery();

                        if (rowAffect > 0)
                        {
                            var userModel = new RegisterViewModel
                            {
                                Username = model.Username,
                                Password = model.Password,
                                Email = model.Email,
                                FirstName = model.FirstName,
                                LastName = model.LastName,
                                Type = model.Type,
                                PhoneNumber = model.PhoneNumber
                            };

                            return View("Register", userModel);
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Registration failed");
            ModelState.AddModelError("", "Registration failed. Please try again.");
        }

        return View(model);
    }

    // Login Actions
    [HttpGet]
    public IActionResult Login()
    {
        return View("Login", new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM [User] WHERE Username = @username";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", model.Username);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPassword = reader["Password"]?.ToString() ?? string.Empty;

                                if (_userRepo.HashPassword(model.Password) == storedPassword)
                                {
                                    HttpContext.Session.SetInt32("userId", Convert.ToInt32(reader["id"]));
                                    HttpContext.Session.SetString("userType", reader["type"]?.ToString() ?? string.Empty);
                                    _logger.LogInformation("Setting userId in session: {UserId}", Convert.ToInt32(reader["id"]));

                                    var returnUrl = TempData["ReturnUrl"]?.ToString();
                                    if (!string.IsNullOrEmpty(returnUrl))
                                    {
                                        return RedirectToAction("landingPage", "Home");
                                    }

                                    return RedirectToAction("landingPage", "Home");
                                }
                            }
                        }
                    }

                    ModelState.AddModelError("", "Invalid username or password");
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error during login");
                ModelState.AddModelError("", "An error occurred during login. Please try again.");
            }
        }
        return View("Login", model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
