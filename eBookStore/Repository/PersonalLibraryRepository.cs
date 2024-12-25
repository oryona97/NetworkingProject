using eBookStore.Models;
using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;

namespace eBookStore.Repository;

public class PersonalLibraryRepository
{
	private readonly string? _connectionString;
	private readonly ILogger<PersonalLibraryRepository> _logger;
	private readonly BookRepository _bookRepository;

	public PersonalLibraryRepository(string? connectionString, ILogger<PersonalLibraryRepository> logger)
	{
		_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_bookRepository = new BookRepository(connectionString);
	}

	public List<BookViewModel> GetUserBooks(int userId)
	{
		var userBooks = new List<BookViewModel>();

		try
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = @"
                    SELECT b.*, p.name as PublisherName, g.name as GenreName,
                           a.name as AuthorName, c.imgName as CoverImage
                    FROM Book b
                    INNER JOIN PersonalLibrary pl ON b.id = pl.bookId
                    LEFT JOIN Publisher p ON b.publisherId = p.id
                    LEFT JOIN Genre g ON b.genreId = g.id
                    LEFT JOIN Auther a ON b.id = a.bookId
                    LEFT JOIN Cover c ON b.id = c.bookId
                    WHERE pl.userId = @UserId
                    ORDER BY pl.createdAt DESC";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@UserId", userId);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							var book = new BookModel
							{
								id = reader.GetInt32(reader.GetOrdinal("id")),
								publisherId = reader.GetInt32(reader.GetOrdinal("publisherId")),
								genreId = reader.GetInt32(reader.GetOrdinal("genreId")),
								amountOfCopies = reader.GetInt32(reader.GetOrdinal("amountOfCopies")),
								title = reader["title"].ToString(),
								borrowPrice = Convert.ToSingle(reader["borrowPrice"]),
								buyingPrice = Convert.ToSingle(reader["buyingPrice"]),
								pubDate = Convert.ToDateTime(reader["pubDate"]),
								ageLimit = reader.GetInt32(reader.GetOrdinal("ageLimit")),
								priceHistory = reader.GetInt32(reader.GetOrdinal("priceHistory")),
								onSale = reader.GetBoolean(reader.GetOrdinal("onSale")),
								canBorrow = reader.GetBoolean(reader.GetOrdinal("canBorrow")),
								starRate = Convert.ToSingle(reader["starRate"]),
								createdAt = Convert.ToDateTime(reader["createdAt"])
							};

							var bookViewModel = new BookViewModel
							{
								book = book,
								publisherModel = _bookRepository.PubModelByBookId(book.id),
								feedbackModel = _bookRepository.getfeedbackModelById(book.id),
								rating = _bookRepository.getRatingModel(book.id),
								coverModel = _bookRepository.getCoverModelById(book.id),
								genreModel = _bookRepository.getGenreModelById(book.genreId)
							};

							userBooks.Add(bookViewModel);
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

	public BookViewModel? GetBookDetails(int bookId, int userId)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = @"
                    SELECT b.*, p.name as PublisherName, g.name as GenreName,
                           a.name as AuthorName, c.imgName as CoverImage
                    FROM Book b
                    INNER JOIN PersonalLibrary pl ON b.id = pl.bookId
                    LEFT JOIN Publisher p ON b.publisherId = p.id
                    LEFT JOIN Genre g ON b.genreId = g.id
                    LEFT JOIN Auther a ON b.id = a.bookId
                    LEFT JOIN Cover c ON b.id = c.bookId
                    WHERE b.id = @BookId AND pl.userId = @UserId";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@BookId", bookId);
					command.Parameters.AddWithValue("@UserId", userId);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							var book = new BookModel
							{
								id = reader.GetInt32(reader.GetOrdinal("id")),
								publisherId = reader.GetInt32(reader.GetOrdinal("publisherId")),
								genreId = reader.GetInt32(reader.GetOrdinal("genreId")),
								amountOfCopies = reader.GetInt32(reader.GetOrdinal("amountOfCopies")),
								title = reader["title"].ToString(),
								borrowPrice = Convert.ToSingle(reader["borrowPrice"]),
								buyingPrice = Convert.ToSingle(reader["buyingPrice"]),
								pubDate = Convert.ToDateTime(reader["pubDate"]),
								ageLimit = reader.GetInt32(reader.GetOrdinal("ageLimit")),
								priceHistory = reader.GetInt32(reader.GetOrdinal("priceHistory")),
								onSale = reader.GetBoolean(reader.GetOrdinal("onSale")),
								canBorrow = reader.GetBoolean(reader.GetOrdinal("canBorrow")),
								starRate = Convert.ToSingle(reader["starRate"]),
								createdAt = Convert.ToDateTime(reader["createdAt"])
							};

							return new BookViewModel
							{
								book = book,
								publisherModel = _bookRepository.PubModelByBookId(book.id),
								feedbackModel = _bookRepository.getfeedbackModelById(book.id),
								rating = _bookRepository.getRatingModel(book.id),
								coverModel = _bookRepository.getCoverModelById(book.id),
								genreModel = _bookRepository.getGenreModelById(book.genreId)
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

	public void RemoveBookFromLibrary(int bookId, int userId)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = "DELETE FROM PersonalLibrary WHERE bookId = @BookId AND userId = @UserId";

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

	public void AddBookToLibrary(int bookId, int userId)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = @"
                    IF NOT EXISTS (SELECT 1 FROM PersonalLibrary WHERE bookId = @BookId AND userId = @UserId)
                    BEGIN
                        INSERT INTO PersonalLibrary (userId, bookId, createdAt)
                        VALUES (@UserId, @BookId, @CreatedAt)
                    END";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@BookId", bookId);
					command.Parameters.AddWithValue("@UserId", userId);
					command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);
					command.ExecuteNonQuery();
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in AddBookToLibrary for bookId: {BookId}, userId: {UserId}", bookId, userId);
			throw;
		}
	}
}
