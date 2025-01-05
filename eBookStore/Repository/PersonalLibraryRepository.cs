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
                    publishers = _bookRepository.PubModelByBookId(book.id),
                    feedbackModel = _bookRepository.getfeedbackModelById(book.id),
                    coverModel = _bookRepository.getCoverModelById(book.id) ?? new CoverModel(),
                    genreModel = _bookRepository.getGenreModelById(book.genreId) ?? new GenreModel()
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


    public BookViewModel? GetBookDetails(int bookId, int userId)
    {
        try
        {
            var books = _bookRepository.getBookById(bookId);
            return books;
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
