using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace eBookStore.Repository;
public class BookRepository
{
	private string? connectionString;

	public BookRepository(string _connectionString)
	{
		connectionString = _connectionString;
	}

	public List<BookViewModel> SearchBooks(
	string? title = null,
	int? publisherId = null,
	int? genreId = null,
	float? minPrice = null,
	float? maxPrice = null,
	DateTime? fromDate = null,
	DateTime? toDate = null)
	{
		var bookViewModelList = new List<BookViewModel>();

		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();


				var query = new StringBuilder("SELECT * FROM [Book] WHERE 1=1");
				var parameters = new List<SqlParameter>();

				if (!string.IsNullOrEmpty(title))
				{
					query.Append(" AND title LIKE @title");
					parameters.Add(new SqlParameter("@title", $"%{title}%"));
				}

				if (publisherId.HasValue)
				{
					query.Append(" AND publisherId = @publisherId");
					parameters.Add(new SqlParameter("@publisherId", publisherId.Value));
				}

				if (genreId.HasValue)
				{
					query.Append(" AND genreId = @genreId");
					parameters.Add(new SqlParameter("@genreId", genreId.Value));
				}

				if (minPrice.HasValue)
				{
					query.Append(" AND borrowPrice >= @minPrice");
					parameters.Add(new SqlParameter("@minPrice", minPrice.Value));
				}

				if (maxPrice.HasValue)
				{
					query.Append(" AND borrowPrice <= @maxPrice");
					parameters.Add(new SqlParameter("@maxPrice", maxPrice.Value));
				}

				if (fromDate.HasValue)
				{
					query.Append(" AND pubDate >= @fromDate");
					parameters.Add(new SqlParameter("@fromDate", fromDate.Value));
				}

				if (toDate.HasValue)
				{
					query.Append(" AND pubDate <= @toDate");
					parameters.Add(new SqlParameter("@toDate", toDate.Value));
				}

				using (var command = new SqlCommand(query.ToString(), connection))
				{
					command.Parameters.AddRange(parameters.ToArray());

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var book = new BookModel
							{
								id = reader["id"] != DBNull.Value ? Convert.ToInt32(reader["id"]) : 0,
								publisherId = reader["publisherId"] != DBNull.Value ? Convert.ToInt32(reader["publisherId"]) : 0,
								genreId = reader["genreId"] != DBNull.Value ? Convert.ToInt32(reader["genreId"]) : 0,
								amountOfCopies = reader["amountOfCopies"] != DBNull.Value ? Convert.ToInt32(reader["amountOfCopies"]) : 0,
								title = reader["title"]?.ToString(),
								borrowPrice = reader["borrowPrice"] != DBNull.Value ? Convert.ToSingle(reader["borrowPrice"]) : 0,
								buyingPrice = reader["buyingPrice"] != DBNull.Value ? Convert.ToSingle(reader["buyingPrice"]) : 0,
								pubDate = reader["pubDate"] != DBNull.Value ? Convert.ToDateTime(reader["pubDate"]) : DateTime.MinValue,
								ageLimit = reader["ageLimit"] != DBNull.Value ? Convert.ToInt32(reader["ageLimit"]) : 0,
								priceHistory = reader["priceHistory"] != DBNull.Value ? Convert.ToInt32(reader["priceHistory"]) : 0,
								onSale = reader["onSale"] != DBNull.Value && Convert.ToBoolean(reader["onSale"]),
								canBorrow = reader["canBorrow"] != DBNull.Value && Convert.ToBoolean(reader["canBorrow"]),
								starRate = reader["starRate"] != DBNull.Value ? Convert.ToSingle(reader["starRate"]) : 0,
								createdAt = reader["createdAt"] != DBNull.Value ? Convert.ToDateTime(reader["createdAt"]) : DateTime.MinValue
							};

							var bookViewModel = new BookViewModel
							{
								book = book,
								publisherModel = PubModelByBookId(book.publisherId),
								feedbackModel = getfeedbackModelById(book.id),
								rating = getRatingModel(book.id),
								coverModel = getCoverModelById(book.id),
								genreModel = this.getGenreModelById(book.genreId),
							};

							foreach (var feedback in bookViewModel.feedbackModel)
							{
								var user = this.getUserModelById(feedback.userId);
								if (user != null)
								{
									feedback.userModel = user;
								}
							}
							bookViewModelList.Add(bookViewModel);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error during book search: {ex.Message}");
		}

		return bookViewModelList;
	}

	public List<BookViewModel> getAllBooks()
	{
		var bookViewModelList = new List<BookViewModel>();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM [Book]";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{

							var book = new BookModel
							{
								id = Convert.ToInt32(reader["id"]),
								publisherId = Convert.ToInt32(reader["publisherId"]),
								genreId = Convert.ToInt32(reader["genreId"]),
								amountOfCopies = Convert.ToInt32(reader["amountOfCopies"]),
								title = reader["title"]?.ToString(),
								borrowPrice = Convert.ToSingle(reader["borrowPrice"]),
								buyingPrice = Convert.ToSingle(reader["buyingPrice"]),
								pubDate = Convert.ToDateTime(reader["pubDate"]),
								ageLimit = Convert.ToInt32(reader["ageLimit"]),
								priceHistory = Convert.ToInt32(reader["priceHistory"]),
								onSale = Convert.ToBoolean(reader["onSale"]),
								canBorrow = Convert.ToBoolean(reader["canBorrow"]),
								starRate = Convert.ToSingle(reader["starRate"]),
								createdAt = Convert.ToDateTime(reader["createdAt"])
							};


							var bookViewModel = new BookViewModel
							{
								book = book,
								publisherModel = this.PubModelByBookId(book.id),
								feedbackModel = this.getfeedbackModelById(book.id),
								rating = this.getRatingModel(book.id),
								coverModel = this.getCoverModelById(book.id),
								genreModel = this.getGenreModelById(book.genreId),
							};


							if (bookViewModel.feedbackModel != null)
							{
								foreach (var feedback in bookViewModel.feedbackModel)
								{
									var user = this.getUserModelById(feedback.userId);
									if (user != null)
									{
										feedback.userModel = user;
									}
								}
							}
							bookViewModelList.Add(bookViewModel);
						}
					}
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine("Database error during fetching books ${ex}");
		}

		return bookViewModelList;
	}

	public GenreModel getGenreModelById(int Id)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM [Genre] WHERE id = @Id";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Id", Id);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							return new GenreModel
							{
								name = reader["name"]?.ToString(),
								id = Convert.ToInt32(reader["id"]),
								createdAt = Convert.ToDateTime(reader["createdAt"])
							};
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching user: {ex.Message}");
		}

		return null;
	}
	public UserModel getUserModelById(int Id)
	{

		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM [User] WHERE id = @Id";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Id", Id);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							return new UserModel
							{
								username = reader["Username"]?.ToString(),
								email = reader["Email"]?.ToString(),
								firstName = reader["FirstName"]?.ToString(),
								lastName = reader["LastName"]?.ToString(),
								phoneNumber = reader["PhoneNumber"]?.ToString(),
								type = reader["type"]?.ToString(),
								id = Convert.ToInt32(reader["id"]),
								createAt = Convert.ToDateTime(reader["createdAt"])
							};


						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching user: {ex.Message}");
		}

		return null;
	}
	public CoverModel getCoverModelById(int Id)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM Cover WHERE bookId = @Id;";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Id", Id);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							var coverModel = new CoverModel
							{
								id = Convert.ToInt32(reader["id"]),
								bookId = Convert.ToInt32(reader["bookId"]),
								imgName = reader["imgName"].ToString(),
								createdAt = Convert.ToDateTime(reader["createdAt"])
							};

							return coverModel;
						}
					}
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine("Database error during fetching Cover {$ex}");
			throw;
		}

		return null;
	}
	public RatingModel getRatingModel(int Id)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM Rating WHERE bookId = @Id;";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Id", Id);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							var rating = new RatingModel
							{
								starRating = Convert.ToInt32(reader["starRating"]),
								userId = Convert.ToInt32(reader["userId"]),
								bookId = Convert.ToInt32(reader["bookId"]),
								createdAt = Convert.ToDateTime(reader["createdAt"])
							};

							return rating;
						}
					}
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine("Database error during fetching Rating ${ex}");
			throw;
		}

		return null;
	}
	public List<FeedbackModel> getfeedbackModelById(int Id)
	{
		var fList = new List<FeedbackModel>();
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM Feedback WHERE bookId = @Id;";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Id", Id);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var feedbackModel = new FeedbackModel
							{
								userId = Convert.ToInt32(reader["userId"]),
								bookId = Convert.ToInt32(reader["bookId"]),
								comment = reader["comment"].ToString(),
								createdAt = Convert.ToDateTime(reader["createdAt"]),
							};
							feedbackModel.userModel = getUserModelById(feedbackModel.userId);
							fList.Add(feedbackModel);
						}
						return fList;
					}
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine("Database error during fetching Feedback ${ex}");
			throw;
		}

		return null;
	}
	public PublisherModel PubModelByBookId(int Id)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM Publisher WHERE id = @Id;";

				using (SqlCommand command = new SqlCommand(query, connection))
				{

					command.Parameters.AddWithValue("@Id", Id);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{

							var publisherModel = new PublisherModel
							{
								id = Convert.ToInt32(reader["id"]),
								name = reader["name"]?.ToString(),
								createdAt = Convert.ToDateTime(reader["createdAt"])
							};

							return publisherModel;
						}
					}
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine("Database error during fetching publisher ${ex}");
			throw;
		}

		return null;
	}

}
