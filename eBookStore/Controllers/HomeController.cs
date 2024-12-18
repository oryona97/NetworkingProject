using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Repository;
using System.Text;
namespace eBookStore.Controllers;


public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IConfiguration _configuration;
	private string? connectionString;
	BookRepository _bookRepo;

	public HomeController(IConfiguration configuration, ILogger<HomeController> logger)
	{
		_configuration = configuration;
		connectionString = _configuration.GetConnectionString("DefaultConnection");
		_logger = logger;
		_bookRepo = new BookRepository(connectionString);
	}

	// Register Actions
	[HttpGet]
	public IActionResult register()
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
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string queries = @"INSERT INTO [User] 
                    (Username, Password, Email, FirstName, LastName, PhoneNumber)  
                    VALUES (@Username, @Password, @Email, @FirstName, @LastName, @PhoneNumber);";

					using (SqlCommand command = new SqlCommand(queries, connection))
					{
						command.Parameters.AddWithValue("@Username", model.Username);
						command.Parameters.AddWithValue("@Password", model.Password);
						command.Parameters.AddWithValue("@Email", model.Email);
						command.Parameters.AddWithValue("@FirstName", model.FirstName);
						command.Parameters.AddWithValue("@LastName", model.LastName);
						command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);

						int rowAffect = command.ExecuteNonQuery();

						if (rowAffect > 0)
						{
							var userModel = new UserModel
							{
								username = model.Username,
								password = model.Password,
								email = model.Email,
								firstName = model.FirstName,
								lastName = model.LastName,
								phoneNumber = model.PhoneNumber
							};

							return View("showRegister", userModel);
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
	public IActionResult showLogin()
	{
		return View("LogIn", new LoginViewModel());
	}

	[HttpPost]
	public IActionResult LogIn(LoginViewModel model)
	{
		if (ModelState.IsValid)
		{
			try
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
				{
					connection.Open();
					string query = "SELECT * FROM [User] WHERE Username = @username;";

					using (SqlCommand command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@username", model.Username);

						using (SqlDataReader reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								string storedPassword = reader["Password"]?.ToString() ?? string.Empty;

								if (model.Password == storedPassword) //TODO: add proper password hashing
								{
									// Login successful
									var userModel = new UserModel
									{
										username = reader["Username"]?.ToString(),
										email = reader["Email"]?.ToString(),
										firstName = reader["FirstName"]?.ToString(),
										lastName = reader["LastName"]?.ToString(),
										phoneNumber = reader["PhoneNumber"]?.ToString(),
										id = Convert.ToInt32(reader["id"])
									};

									return View("showLogIn", userModel);
								}
							}
						}
					}

					// Invalid login
					ModelState.AddModelError("", "Invalid username or password");
				}
			}
			catch (SqlException ex)
			{
				_logger.LogError(ex, "Database error during login");
				ModelState.AddModelError("", "An error 1occurred during login. Please try again.");
			}
		}

		return View("LogIn", model);
	}

	public IActionResult showBook()
	{
		Console.WriteLine("showBook");
		List<BookViewModel> books = _bookRepo.getAllBooks();
		
		return View(books);
	}
	public IActionResult showBookBySearch(string? title, int? publisherId, int? genreId, float? minPrice, float? maxPrice, DateTime? fromDate, DateTime? toDate)
	{
		List<BookViewModel> books = _bookRepo.SearchBooks(title, publisherId, genreId, minPrice, maxPrice, fromDate, toDate);
		return View("showBook", books);
	}

	public IActionResult SearchForm()
	{
		return View("SearchBooks"); 
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
