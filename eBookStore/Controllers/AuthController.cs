using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Repository;
using System.Text;
namespace eBookStore.Controllers;


public class AuthController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly IConfiguration _configuration;
	private string? _connectionString;
	private UserRepository _userRepo;

	public AuthController(IConfiguration configuration, ILogger<HomeController> logger)
	{
		_configuration = configuration;
		_connectionString = _configuration.GetConnectionString("DefaultConnection");
		_logger = logger;
		_userRepo = new UserRepository(_connectionString);
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
									HttpContext.Session.SetInt32("userId", Convert.ToInt32(reader["id"]));
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

									return View("Login", userModel);
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
		return View("Login", model);
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
