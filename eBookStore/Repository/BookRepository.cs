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

	


	//this func to Add  FeadbackModel List to the database 
	public void AddFeedbackModel(List<FeedbackModel> feedbackModel)
	{
		foreach (var feedback in feedbackModel)
		{
			AddFeedback(feedback);
		}
	}

	//this func to Add FeedbackModel to the database

	public void AddFeedback(FeedbackModel feedback)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "INSERT INTO Feedback (userId, bookId, comment, createdAt) VALUES (@userId, @bookId, @comment, @createdAt);";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@userId", feedback.userId);
					command.Parameters.AddWithValue("@bookId", feedback.bookId);
					command.Parameters.AddWithValue("@comment", feedback.comment);
					command.Parameters.AddWithValue("@createdAt", feedback.createdAt);

					command.ExecuteNonQuery();
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine($"Database error during adding feedback {ex}");
			throw;
		}
	}

	//this funct to Add bookViewModel to the database
	public void AddBookViewModel(BookViewModel bookViewModel)
	{
		// Add book
		AddBook(bookViewModel.book);
		AddCoverModel(bookViewModel.coverModel);
		AddPublisherModel(bookViewModel.publisherModel);
		AddGenreModel(bookViewModel.genreModel);
		AddAuthorModel(bookViewModel.authorModel);
	}
	
	//this func to add AuthorModel to the database
	public void AddAuthorModel(AuthorModel authorModel)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "INSERT INTO Auther ( name, bookId, createdAt) VALUES ( @name, @bookId, @createdAt);";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@bookId", authorModel.bookId); 
					command.Parameters.AddWithValue("@name", authorModel.name);
					command.Parameters.AddWithValue("@createdAt", authorModel.createdAt);

					command.ExecuteNonQuery();
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine("Database error during adding author ${ex}");
			throw;
		}
	}
	//this func to add bookModelto the database 
	public void AddBook(BookModel book)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "INSERT INTO Book (publisherId, genreId, amountOfCopies, title, borrowPrice, buyingPrice, pubDate, ageLimit, priceHistory, onSale, canBorrow, starRate, createdAt) VALUES (@publisherId, @genreId, @amountOfCopies, @title, @borrowPrice, @buyingPrice, @pubDate, @ageLimit, @priceHistory, @onSale, @canBorrow, @starRate, @createdAt);";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@publisherId", book.publisherId);
					command.Parameters.AddWithValue("@genreId", book.genreId);
					command.Parameters.AddWithValue("@amountOfCopies", book.amountOfCopies);
					command.Parameters.AddWithValue("@title", book.title);
					command.Parameters.AddWithValue("@borrowPrice", book.borrowPrice);
					command.Parameters.AddWithValue("@buyingPrice", book.buyingPrice);
					command.Parameters.AddWithValue("@pubDate", book.pubDate);
					command.Parameters.AddWithValue("@ageLimit", book.ageLimit);
					command.Parameters.AddWithValue("@priceHistory", book.priceHistory);
					command.Parameters.AddWithValue("@onSale", book.onSale);
					command.Parameters.AddWithValue("@canBorrow", book.canBorrow);
					command.Parameters.AddWithValue("@starRate", book.starRate);
					command.Parameters.AddWithValue("@createdAt", book.createdAt);

					command.ExecuteNonQuery();
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine($"Database error during adding book {ex}");
			throw;
		}
	}
	
	//this func to add PublisherModel to the database
	public bool AddPublisherModel(PublisherModel pubModel)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "INSERT INTO Publisher (name, createdAt) VALUES (@name, @createdAt);";

				using (var command = new SqlCommand(query, connection))
				{
					
					command.Parameters.AddWithValue("@name", pubModel.name);
					command.Parameters.AddWithValue("@createdAt", pubModel.createdAt);

					command.ExecuteNonQuery();
				}
				return true;
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine($"Database error during adding publisher {ex}");
			return false;
		}
		
	}

	//this AddGenreModel to the database
	public void AddGenreModel(GenreModel genreModel)
{
    try
    {
        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();
            string query = "INSERT INTO Genre ( name, createdAt) VALUES ( @name, @createdAt);";

            using (var command = new SqlCommand(query, connection))
            {
                
                command.Parameters.AddWithValue("@name", genreModel.name);
                command.Parameters.AddWithValue("@createdAt", genreModel.createdAt); 

                command.ExecuteNonQuery();
            }
        }
    }
    catch (SqlException ex)
    {
        Console.WriteLine($"Database error during adding genre: {ex.Message}");
        throw;
    }
}
	
	//this func to add RatingModel to the database
	public void AddRating(RatingModel rating)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "INSERT INTO Rating (starRating, userId, bookId, createdAt) VALUES (@starRating, @userId, @bookId, @createdAt);";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@starRating", rating.starRating);
					command.Parameters.AddWithValue("@userId", rating.userId);
					command.Parameters.AddWithValue("@bookId", rating.bookId);
					command.Parameters.AddWithValue("@createdAt", rating.createdAt);

					command.ExecuteNonQuery();
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine($"Database error during adding rating {ex}");
			throw;
		}
	}


	//this func to add coverModel to the database
	public void AddCoverModel(CoverModel coverModel)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "INSERT INTO Cover ( bookId, imgName, createdAt) VALUES ( @bookId, @imgName, @createdAt);";

				using (var command = new SqlCommand(query, connection))
				{
					
					command.Parameters.AddWithValue("@bookId", coverModel.bookId);
					command.Parameters.AddWithValue("@imgName", coverModel.imgName);
					command.Parameters.AddWithValue("@createdAt", coverModel.createdAt);

					command.ExecuteNonQuery();
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine($"Database error during adding cover {ex}");
			throw;
		}
	}
	
	//this func to search for books
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

							bookViewModel.authorModel = getAuthorModelById(book.id);
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


	//this func to get all books
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
								authorModel = this.getAuthorModelById(book.id),
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

								
							bookViewModel.authorModel = this.getAuthorModelById(bookViewModel.book.id);
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

	//this func to get book by id
	public BookViewModel getBookById(int Id)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM [Book] WHERE id = @Id";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@Id", Id);

					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
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
								publisherModel = this.PubModelByBookId(book.publisherId),
								feedbackModel = this.getfeedbackModelById(book.id),
								rating = this.getRatingModel(book.id),
								coverModel = this.getCoverModelById(book.id),
								genreModel =this.getGenreModelById(book.genreId),
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
							return bookViewModel;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine($"Error fetching book: {ex.Message}");
		}

		return null; 
	}

	//this func to get book by title
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

	//this func to get user by id
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

	//this func to get cover by id
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

	//this func to get rating by id
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

	//this func to get feedback by id
    public List <FeedbackModel> getfeedbackModelById(int Id)
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

	//this func to get publisher by id
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
	

	//this func to get all publishers
	public List<PublisherModel> getAllPublishers (int pubId)
	{
		var pubList = new List<PublisherModel>();
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM Publisher WHERE id = @pubId;";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@pubId", pubId);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while(reader.Read())
						{
							var publisherModel = new PublisherModel
							{
								id = Convert.ToInt32(reader["id"]),
								name = reader["name"]?.ToString(),
								createdAt = Convert.ToDateTime(reader["createdAt"])
							};
							pubList.Add(publisherModel);
						}
							return pubList;
					}
				}
			}
		}
		catch (SqlException ex)
		{
		    Console.WriteLine("Database error during fetching Publisher ${ex}");
			throw; 
		}

		return pubList; 
	}

	//this func to get Aothers by id

	public AuthorModel getAuthorModelById(int bookId)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM Auther WHERE bookId = @Id;";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					
					command.Parameters.AddWithValue("@Id", bookId);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							
							var authorModel = new AuthorModel
							{
								id = Convert.ToInt32(reader["id"]),
								bookId = Convert.ToInt32(reader["bookId"]),
								name = reader["name"].ToString(),
								createdAt = Convert.ToDateTime(reader["createdAt"])
							};

							return authorModel;
						}
					}
				}
			}
		}
		catch (SqlException ex)
		{
		    Console.WriteLine("Database error during fetching Author ${ex}");
			throw; 
		}

		return null; 
	}

	//this func return all Genres name that exsist in db
	public List<string> getAllGenres()
	{
		var genreList = new List<string>();
		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT name FROM Genre;";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while(reader.Read())
						{
							genreList.Add(reader["name"].ToString());
						}
						return genreList;
					}
				}
			}
		}
		catch (SqlException ex)
		{
		    Console.WriteLine($"Database error during fetching Genre {ex}");
			throw; 
		}

		return genreList; 
	}


	//func to get all books can be borrowed
	public List<BookViewModel> getBorrowableBooks()
	{
		var bookViewModelList = new List<BookViewModel>();

		try
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM [Book] WHERE canBorrow = 1";

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
								publisherModel = this.PubModelByBookId(book.publisherId),
								feedbackModel = this.getfeedbackModelById(book.id),
								rating = this.getRatingModel(book.id),
								coverModel = this.getCoverModelById(book.id),
								genreModel = this.getGenreModelById(book.genreId),
								authorModel = this.getAuthorModelById(book.id),
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

							bookViewModel.authorModel = this.getAuthorModelById(book.id);
							bookViewModelList.Add(bookViewModel);
							
						}
					}
				}
			}
			
		}catch (SqlException ex)
		{
			Console.WriteLine($"Database error during fetching books {ex}");
			throw;
		}
		return bookViewModelList;
	}

	//this func update amount of copies of a book if someome borrows it amountOfCopies--
	public void updateAmountOfCopies(int bookId)
	{
		try
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				string query = "UPDATE Book SET amountOfCopies = amountOfCopies - 1 WHERE id = @bookId;";

				using (var command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@bookId", bookId);

					command.ExecuteNonQuery();
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine($"Database error during borrowing book {ex}");
			throw;
		}
	}

}


