using eBookStore.Models;
using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;

namespace eBookStore.Repository;

public class PersonalLibraryRepository
{
	private readonly string _connectionString;
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
			using var connection = new SqlConnection(_connectionString);
			connection.Open();

			const string query = @"
                SELECT b.*, p.name as PublisherName, g.name as GenreName,
                       a.name as AuthorName, c.imgName as CoverImage
                FROM [book] b
                INNER JOIN [personallibrary] pl ON b.id = pl.bookid
                LEFT JOIN [publisher] p ON b.publisherid = p.id
                LEFT JOIN [genre] g ON b.genreid = g.id
                LEFT JOIN [auther] a ON b.id = a.bookid
                LEFT JOIN [cover] c ON b.id = c.bookid
                WHERE pl.userid = @UserId
                ORDER BY pl.createdat DESC";

			using var command = new SqlCommand(query, connection);
			command.Parameters.AddWithValue("@UserId", userId);

			using var reader = command.ExecuteReader();
			while (reader.Read())
			{
				var book = new BookModel
				{
					id = reader.GetInt32(reader.GetOrdinal("id")),
					publisherId = reader.GetInt32(reader.GetOrdinal("publisherid")),
					genreId = reader.GetInt32(reader.GetOrdinal("genreid")),
					amountOfCopies = reader.GetInt32(reader.GetOrdinal("amountofcopies")),
					title = reader["title"].ToString(),
					borrowPrice = Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("borrowprice"))),
					buyingPrice = Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("buyingprice"))),
					pubDate = Convert.ToDateTime(reader["pubdate"]),
					ageLimit = reader.GetInt32(reader.GetOrdinal("agelimit")),
					priceHistory = reader.GetInt32(reader.GetOrdinal("pricehistory")),
					onSale = reader.GetBoolean(reader.GetOrdinal("onsale")),
					canBorrow = reader.GetBoolean(reader.GetOrdinal("canborrow")),
					starRate = Convert.ToSingle(reader["starrate"]),
					createdAt = Convert.ToDateTime(reader["createdat"])
				};

				var bookViewModel = new BookViewModel
				{
					book = book,
					publisherModel = _bookRepository.PubModelByBookId(book.id),
					feedbackModel = _bookRepository.getfeedbackModelById(book.id),
					coverModel = _bookRepository.getCoverModelById(book.id),
					genreModel = _bookRepository.getGenreModelById(book.genreId)
				};

				userBooks.Add(bookViewModel);
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error in GetUserBooks for userId: {UserId}", userId);
			throw;
		}

		return userBooks;
	}


	public BookViewModel GetBookDetails(int bookId, int userId)
	{
		try
		{
			using var connection = new SqlConnection(_connectionString);
			connection.Open();

			const string sql = @"
        SELECT 
            b.id, b.publisherid, b.genreid, b.amountofcopies,
            b.title, b.borrowprice, b.buyingprice, b.pubdate,
            b.agelimit, b.pricehistory, b.onsale, b.canborrow,
            b.starrate, b.createdat,
            p.id as publisher_id, p.name as publisher_name,
            g.id as genre_id, g.name as genre_name,
            f.comment, f.createdat as feedback_createdat,
            f.userid as feedback_userid, u.username as feedback_username,
            pl.userid as owner_id,
            c.id as cover_id, c.imgname as cover_image
        FROM [book] b
        LEFT JOIN [publisher] p ON b.publisherid = p.id
        LEFT JOIN [genre] g ON b.genreid = g.id
        LEFT JOIN [feedback] f ON b.id = f.bookid
        LEFT JOIN [user] u ON f.userid = u.id
        LEFT JOIN [personallibrary] pl ON b.id = pl.bookid
        LEFT JOIN [cover] c ON b.id = c.bookid
        WHERE b.id = @BookId";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@BookId", bookId);

			using var reader = command.ExecuteReader();
			BookViewModel? bookViewModel = null;
			var ownerIds = new HashSet<int>();
			var processedFeedbacks = new HashSet<string>();

			while (reader.Read())
			{
				if (bookViewModel == null)
				{
					var book = new BookModel
					{
						id = reader.GetInt32(reader.GetOrdinal("id")),
						publisherId = reader.GetInt32(reader.GetOrdinal("publisherid")),
						genreId = reader.GetInt32(reader.GetOrdinal("genreid")),
						amountOfCopies = reader.GetInt32(reader.GetOrdinal("amountofcopies")),
						title = reader["title"]?.ToString(),
						borrowPrice = reader["borrowprice"] != DBNull.Value
							? Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("borrowprice")))
							: 0.0f,
						buyingPrice = reader["buyingprice"] != DBNull.Value
							? Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("buyingprice")))
							: 0.0f,
						pubDate = reader.GetDateTime(reader.GetOrdinal("pubdate")),
						ageLimit = reader.GetInt32(reader.GetOrdinal("agelimit")),
						priceHistory = reader.GetInt32(reader.GetOrdinal("pricehistory")),
						onSale = reader.GetBoolean(reader.GetOrdinal("onsale")),
						canBorrow = reader.GetBoolean(reader.GetOrdinal("canborrow")),
						starRate = reader["starrate"] != DBNull.Value
							? Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("starrate")))
							: 0.0f,
						createdAt = reader.GetDateTime(reader.GetOrdinal("createdat"))
					};

					var publisherModel = new PublisherModel
					{
						id = reader.GetInt32(reader.GetOrdinal("publisher_id")),
						name = reader["publisher_name"]?.ToString(),
						createdAt = DateTime.Now
					};

					var genreModel = new GenreModel
					{
						id = reader.GetInt32(reader.GetOrdinal("genre_id")),
						name = reader["genre_name"]?.ToString(),
						createdAt = DateTime.Now
					};

					var coverModel = new CoverModel
					{
						id = reader["cover_id"] != DBNull.Value ? reader.GetInt32(reader.GetOrdinal("cover_id")) : 0,
						imgName = reader["cover_image"]?.ToString(),
						bookId = bookId,
						createdAt = DateTime.Now
					};

					bookViewModel = new BookViewModel
					{
						book = book,
						publisherModel = publisherModel,
						genreModel = genreModel,
						coverModel = coverModel,
						feedbackModel = new List<FeedbackModel>(),
						ownerUserIds = new List<int>()
					};
				}

				// Handle nullable owner_id
				if (reader["owner_id"] != DBNull.Value)
				{
					var ownerId = reader.GetInt32(reader.GetOrdinal("owner_id"));
					if (!ownerIds.Contains(ownerId))
					{
						ownerIds.Add(ownerId);
						bookViewModel.ownerUserIds.Add(ownerId);
					}
				}

				// Handle nullable feedback fields
				if (reader["feedback_userid"] != DBNull.Value)
				{
					var feedbackUserId = reader.GetInt32(reader.GetOrdinal("feedback_userid"));
					var comment = reader["comment"];
					var createdAt = reader["feedback_createdat"] != DBNull.Value
						? reader.GetDateTime(reader.GetOrdinal("feedback_createdat"))
						: DateTime.Now;

					var feedbackKey = $"{feedbackUserId}|{comment}|{createdAt}";
					if (!processedFeedbacks.Contains(feedbackKey))
					{
						processedFeedbacks.Add(feedbackKey);

						var feedback = new FeedbackModel
						{
							bookId = bookId,
							userId = feedbackUserId,
							comment = comment?.ToString(),
							createdAt = createdAt,
							userModel = new UserModel
							{
								id = feedbackUserId,
								username = reader["feedback_username"]?.ToString()
							}
						};

						bookViewModel.feedbackModel.Add(feedback);
					}
				}
			}

			if (bookViewModel == null)
			{
				return null;
			}

			bookViewModel.feedbackModel = bookViewModel.feedbackModel
				.OrderByDescending(f => f.createdAt)
				.ToList();

			return bookViewModel;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error retrieving book details for ID: {BookId}", bookId);
			throw;
		}
	}

	public void AddBookToLibrary(int bookId, int userId)
	{
		try
		{
			using var connection = new SqlConnection(_connectionString);
			connection.Open();

			const string sql = @"
                IF NOT EXISTS (
                    SELECT 1 FROM [personallibrary] 
                    WHERE bookid = @BookId AND userid = @UserId
                )
                BEGIN
                    INSERT INTO [personallibrary] (userid, bookid, createdat)
                    VALUES (@UserId, @BookId, @CreatedAt)
                END";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@BookId", bookId);
			command.Parameters.AddWithValue("@UserId", userId);
			command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

			command.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding book ID: {BookId} to library for user ID: {UserId}", bookId, userId);
			throw;
		}
	}

	public void RemoveBookFromLibrary(int bookId, int userId)
	{
		try
		{
			using var connection = new SqlConnection(_connectionString);
			connection.Open();

			const string sql = "DELETE FROM [personallibrary] WHERE bookid = @BookId AND userid = @UserId";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@BookId", bookId);
			command.Parameters.AddWithValue("@UserId", userId);

			command.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error removing book ID: {BookId} from library for user ID: {UserId}", bookId, userId);
			throw;
		}
	}

	public void AddReview(int bookId, int userId, string comment)
	{
		try
		{
			using var connection = new SqlConnection(_connectionString);
			connection.Open();

			const string sql = @"
                INSERT INTO [feedback] (bookid, userid, comment, createdat)
                VALUES (@BookId, @UserId, @Comment, @CreatedAt)";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@BookId", bookId);
			command.Parameters.AddWithValue("@UserId", userId);
			command.Parameters.AddWithValue("@Comment", comment);
			command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

			command.ExecuteNonQuery();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding review for book ID: {BookId}", bookId);
			throw;
		}
	}

	public bool DeleteReview(int bookId, int userId)
	{
		try
		{
			using var connection = new SqlConnection(_connectionString);
			connection.Open();

			const string sql = @"
                DELETE FROM [feedback] 
                WHERE bookid = @BookId AND userid = @UserId;
                SELECT @@ROWCOUNT;";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@BookId", bookId);
			command.Parameters.AddWithValue("@UserId", userId);

			var rowsAffected = (int)command.ExecuteScalar();
			return rowsAffected > 0;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting review for book ID: {BookId}, user ID: {UserId}", bookId, userId);
			throw;
		}
	}

	public async Task AddReviewAsync(int bookId, int userId, string comment)
	{
		try
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();

			const string sql = @"
            INSERT INTO [feedback] (bookid, userid, comment, createdat)
            VALUES (@BookId, @UserId, @Comment, @CreatedAt)";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@BookId", bookId);
			command.Parameters.AddWithValue("@UserId", userId);
			command.Parameters.AddWithValue("@Comment", comment);
			command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

			await command.ExecuteNonQueryAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error adding review for book ID: {BookId}", bookId);
			throw;
		}
	}

	public async Task<bool> DeleteReviewAsync(int bookId, int userId)
	{
		try
		{
			using var connection = new SqlConnection(_connectionString);
			await connection.OpenAsync();
			const string sql = @"
            DELETE FROM [feedback] 
            WHERE bookid = @BookId AND userid = @UserId;";

			using var command = new SqlCommand(sql, connection);
			command.Parameters.AddWithValue("@BookId", bookId);
			command.Parameters.AddWithValue("@UserId", userId);

			var rowsAffected = await command.ExecuteNonQueryAsync();
			return rowsAffected > 0;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error deleting review for book ID: {BookId}, user ID: {UserId}", bookId, userId);
			throw;
		}
	}
}
