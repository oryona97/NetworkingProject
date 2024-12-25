using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Repository;

namespace eBookStore.Controllers;

public class PersonalLibraryController : Controller
{
	private readonly ILogger<PersonalLibraryController> _logger;
	private readonly IConfiguration _configuration;
	private string? connectionString;
	private BookRepository _bookRepo;

	public PersonalLibraryController(IConfiguration configuration, ILogger<PersonalLibraryController> logger)
	{
		_configuration = configuration;
		connectionString = _configuration.GetConnectionString("DefaultConnection");
		_logger = logger;
		_bookRepo = new BookRepository(connectionString);
	}

	// GET: PersonalLibrary
	public IActionResult Index()
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				return RedirectToAction("showLogin", "Home");
			}

			var userBooks = GetUserBooks(userId.Value);
			return View(userBooks);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving personal library");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	private List<BookViewModel> GetUserBooks(int userId)
	{
		var userBooks = new List<BookViewModel>();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = @"
                    SELECT b.* 
                    FROM Book b
                    INNER JOIN UserBooks ub ON b.Id = ub.BookId
                    WHERE ub.UserId = @UserId";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@UserId", userId);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							userBooks.Add(new BookViewModel
							{
								Id = Convert.ToInt32(reader["Id"]),
								Title = reader["Title"].ToString(),
								Author = reader["Author"].ToString(),
								Price = Convert.ToDecimal(reader["Price"]),
								PublishDate = Convert.ToDateTime(reader["PublishDate"]),
								Description = reader["Description"].ToString(),
								CoverImageUrl = reader["CoverImageUrl"].ToString()
							});
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in GetUserBooks for userId: {UserId}", userId);
			throw;
		}

		return userBooks;
	}

	// GET: PersonalLibrary/Details/5
	public IActionResult Details(int id)
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				return RedirectToAction("showLogin", "Home");
			}

			var book = GetBookDetails(id, userId.Value);
			if (book == null)
			{
				return NotFound();
			}

			return View(book);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving book details");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	private BookViewModel? GetBookDetails(int bookId, int userId)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = @"
                    SELECT b.* 
                    FROM Book b
                    INNER JOIN UserBooks ub ON b.Id = ub.BookId
                    WHERE b.Id = @BookId AND ub.UserId = @UserId";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@BookId", bookId);
					command.Parameters.AddWithValue("@UserId", userId);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							return new BookViewModel
							{
								Id = Convert.ToInt32(reader["Id"]),
								Title = reader["Title"].ToString(),
								Author = reader["Author"].ToString(),
								Price = Convert.ToDecimal(reader["Price"]),
								PublishDate = Convert.ToDateTime(reader["PublishDate"]),
								Description = reader["Description"].ToString(),
								CoverImageUrl = reader["CoverImageUrl"].ToString()
							};
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in GetBookDetails for bookId: {BookId}, userId: {UserId}", bookId, userId);
			throw;
		}

		return null;
	}

	// POST: PersonalLibrary/RemoveBook/5
	[HttpPost]
	public IActionResult RemoveBook(int id)
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				return RedirectToAction("showLogin", "Home");
			}

			RemoveBookFromLibrary(id, userId.Value);
			return RedirectToAction(nameof(Index));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error removing book from library");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	private void RemoveBookFromLibrary(int bookId, int userId)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "DELETE FROM UserBooks WHERE BookId = @BookId AND UserId = @UserId";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@BookId", bookId);
					command.Parameters.AddWithValue("@UserId", userId);
					command.ExecuteNonQuery();
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in RemoveBookFromLibrary for bookId: {BookId}, userId: {UserId}", bookId, userId);
			throw;
		}
	}

	// GET: PersonalLibrary/ReadBook/5
	public IActionResult ReadBook(int id)
	{
		try
		{
			int? userId = HttpContext.Session.GetInt32("userId");
			if (!userId.HasValue)
			{
				return RedirectToAction("showLogin", "Home");
			}

			var book = GetBookDetails(id, userId.Value);
			if (book == null)
			{
				return NotFound();
			}

			return View(book);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error accessing book reader");
			return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
