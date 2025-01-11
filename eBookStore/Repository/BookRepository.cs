using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Models;
using System.Text;

namespace eBookStore.Repository;
public class BookRepository
{
    private string? connectionString;

    public BookRepository(string? _connectionString)
    {
        connectionString = _connectionString;
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
        // בדוק אם הז'אנר קיים לפי שם
        int genreId = GetGenreIdByName(bookViewModel.genreModel?.name ?? "");
        if (genreId == 0)
        {
            // הוסף ז'אנר חדש אם הוא לא קיים
            if (bookViewModel.genreModel != null)
                AddGenreModel(bookViewModel.genreModel);
            genreId = GetGenreIdByName(bookViewModel.genreModel?.name ?? "");
        }
        bookViewModel.book.genreId = genreId;



        // הגדר ערכי ברירת מחדל לספר
        bookViewModel.book.onSale = false;
        bookViewModel.book.amountOfCopies = 3;

        // הוסף את הספר
        AddBook(bookViewModel.book);

        // קבל את ה-id של הספר שנוסף
        int bookId = getBookIDByName(bookViewModel.book.title ?? "");
        if (bookId == 0)
        {
            throw new Exception("Failed to retrieve book ID after adding the book.");
        }
        bookViewModel.coverModel = new CoverModel
        {
            bookId = bookId,
            imgName = $"{bookViewModel.book.title}_cover.jpg".Replace(" ", "_"),
            createdAt = DateTime.Now
        };

        int? authorId = getAuthorIDByName(bookViewModel.authorModel.name);
        if (authorId == 0)
        {
            if (bookViewModel.authorModel != null)
                bookViewModel.authorModel.bookId = bookId;
            AddAuthorModel(bookViewModel.authorModel);

        }
        bookViewModel.authorModel = getAuthorModelById(bookId);

        // בצע את אותה בדיקה ל-Publisher
        foreach (var publisher in bookViewModel.publishers)
        {
            publisher.bookId = bookId;
            int publisherId = gettingPublisherIdByName(publisher.name ?? "");
            if (publisherId == 0)
            {
                Console.WriteLine(publisher.name);
                AddPublisherModel(publisher);
                publisherId = gettingPublisherIdByName(publisher.name ?? "");
            }
            bookViewModel.book.publisherId = publisherId;
        }




        AddCoverModel(bookViewModel.coverModel);
    }

    public int GetGenreIdByName(string genreName)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id FROM Genre WHERE name = @name;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", genreName);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return Convert.ToInt32(reader["id"]);
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during fetching genreId: {ex.Message}");
        }
        return 0;
    }

    public int getBookIDByName(string bookTitle)
    {
        var bookID = 0;
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM [Book] WHERE title = @bookTitle";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookTitle", bookTitle);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bookID = Convert.ToInt32(reader["id"]);
                            return bookID;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching book: {ex.Message}");
        }
        return 0;
    }

    //this func to delete bookViewModel from db
    public void DeleteBookViewModel(int bookId)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                //dlelete book model
                string query = "DELETE FROM Book WHERE id = @bookId;";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookId", bookId);
                    command.ExecuteNonQuery();
                }

            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during deleting book {ex}");
            throw;
        }
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
            Console.WriteLine($"Database error during adding author {ex}");
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
                string query = "INSERT INTO Publisher (bookId, name) VALUES (@bookId, @name);";

                using (var command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@bookId", pubModel.bookId);
                    command.Parameters.AddWithValue("@name", pubModel.name);

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
                                publishers = PubModelByBookId(book.publisherId),
                                feedbackModel = getfeedbackModelById(book.id),
                                rating = getRatingModel(book.id) ?? new RatingModel(),
                                coverModel = getCoverModelById(book.id) ?? new CoverModel(),
                                genreModel = this.getGenreModelById(book.genreId) ?? new GenreModel(),
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

                            bookViewModel.authorModel = getAuthorModelById(book.id) ?? new AuthorModel();
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
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            const string query = @"
            SELECT b.*, pl.userId as owner_id
            FROM [Book] b
            LEFT JOIN [PersonalLibrary] pl ON b.id = pl.bookId";

            using var command = new SqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            var processedBooks = new Dictionary<int, BookViewModel>();

            while (reader.Read())
            {
                var bookId = reader.GetInt32(reader.GetOrdinal("id"));

                if (!processedBooks.ContainsKey(bookId))
                {
                    var book = new BookModel
                    {
                        id = bookId,
                        publisherId = reader["publisherId"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("publisherId"))
                            : 0,
                        genreId = reader["genreId"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("genreId"))
                            : 0,
                        amountOfCopies = reader["amountOfCopies"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("amountOfCopies"))
                            : 0,
                        title = reader["title"]?.ToString(),
                        borrowPrice = reader["borrowPrice"] != DBNull.Value
                            ? Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("borrowPrice")))
                            : 0.0f,
                        buyingPrice = reader["buyingPrice"] != DBNull.Value
                            ? Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("buyingPrice")))
                            : 0.0f,
                        pubDate = reader["pubDate"] != DBNull.Value
                            ? reader.GetDateTime(reader.GetOrdinal("pubDate"))
                            : DateTime.MinValue,
                        ageLimit = reader["ageLimit"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("ageLimit"))
                            : 0,
                        priceHistory = reader["priceHistory"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("priceHistory"))
                            : 0,
                        onSale = reader["onSale"] != DBNull.Value
                            ? reader.GetBoolean(reader.GetOrdinal("onSale"))
                            : false,
                        canBorrow = reader["canBorrow"] != DBNull.Value
                            ? reader.GetBoolean(reader.GetOrdinal("canBorrow"))
                            : false,
                        starRate = reader["starRate"] != DBNull.Value
                            ? Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("starRate")))
                            : 0.0f,
                        createdAt = reader["createdAt"] != DBNull.Value
                            ? reader.GetDateTime(reader.GetOrdinal("createdAt"))
                            : DateTime.Now
                    };

                    var bookViewModel = new BookViewModel
                    {
                        book = book,
                        publishers = this.PubModelByBookId(book.id) ?? new List<PublisherModel>(),
                        feedbackModel = this.getfeedbackModelById(book.id) ?? new List<FeedbackModel>(),
                        rating = this.getRatingModel(book.id) ?? new RatingModel(),
                        coverModel = this.getCoverModelById(book.id) ?? new CoverModel(),
                        genreModel = this.getGenreModelById(book.genreId) ?? new GenreModel(),
                        authorModel = this.getAuthorModelById(book.id) ?? new AuthorModel(),
                    };
                    if (book.onSale)
                    {
                        bookViewModel.bookDiscountModel = getBookDiscountModelById(book.id);
                    }

                    if (bookViewModel.feedbackModel?.Any() == true)
                    {
                        foreach (var feedback in bookViewModel.feedbackModel)
                        {
                            feedback.userModel = getUserModelById(feedback.userId) ?? new UserModel();
                        }
                    }

                    processedBooks.Add(bookId, bookViewModel);
                }

                if (reader["owner_id"] != DBNull.Value)
                {
                    var ownerId = reader.GetInt32(reader.GetOrdinal("owner_id"));
                    if (!processedBooks[bookId].ownerUserIds.Contains(ownerId))
                    {
                        processedBooks[bookId].ownerUserIds.Add(ownerId);
                    }
                }
            }

            return processedBooks.Values.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database error during fetching books: {ex.Message}");
            throw;
        }
    }

    //this func to get book by id
    public BookViewModel? getBookById(int id)
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            const string query = @"
            SELECT b.*, pl.userId as owner_id
            FROM [Book] b
            LEFT JOIN [PersonalLibrary] pl ON b.id = pl.bookId
            WHERE b.id = @id";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Id", id);
            using var reader = command.ExecuteReader();

            BookViewModel? bookViewModel = null;
            var ownerIds = new HashSet<int>();

            while (reader.Read())
            {
                if (bookViewModel == null)
                {
                    var book = new BookModel
                    {
                        id = reader.GetInt32(reader.GetOrdinal("id")),
                        publisherId = reader["publisherId"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("publisherId"))
                            : 0,
                        genreId = reader["genreId"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("genreId"))
                            : 0,
                        amountOfCopies = reader["amountOfCopies"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("amountOfCopies"))
                            : 0,
                        title = reader["title"]?.ToString(),
                        borrowPrice = reader["borrowPrice"] != DBNull.Value
                            ? Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("borrowPrice")))
                            : 0.0f,
                        buyingPrice = reader["buyingPrice"] != DBNull.Value
                            ? Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("buyingPrice")))
                            : 0.0f,
                        pubDate = reader["pubDate"] != DBNull.Value
                            ? reader.GetDateTime(reader.GetOrdinal("pubDate"))
                            : DateTime.MinValue,
                        ageLimit = reader["ageLimit"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("ageLimit"))
                            : 0,
                        priceHistory = reader["priceHistory"] != DBNull.Value
                            ? reader.GetInt32(reader.GetOrdinal("priceHistory"))
                            : 0,
                        onSale = reader["onSale"] != DBNull.Value
                            ? reader.GetBoolean(reader.GetOrdinal("onSale"))
                            : false,
                        canBorrow = reader["canBorrow"] != DBNull.Value
                            ? reader.GetBoolean(reader.GetOrdinal("canBorrow"))
                            : false,
                        starRate = reader["starRate"] != DBNull.Value
                            ? Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("starRate")))
                            : 0.0f,
                        createdAt = reader["createdAt"] != DBNull.Value
                            ? reader.GetDateTime(reader.GetOrdinal("createdAt"))
                            : DateTime.Now
                    };

                    bookViewModel = new BookViewModel
                    {
                        book = book,
                        publishers = PubModelByBookId(book.id) ?? new List<PublisherModel>(),
                        feedbackModel = getfeedbackModelById(book.id) ?? new List<FeedbackModel>(),
                        rating = getRatingModel(book.id) ?? new RatingModel(),
                        coverModel = getCoverModelById(book.id) ?? new CoverModel(),
                        genreModel = getGenreModelById(book.genreId) ?? new GenreModel(),
                        authorModel = getAuthorModelById(book.id) ?? new AuthorModel()
                    };
                    if (book.onSale)
                    {
                        bookViewModel.bookDiscountModel = getBookDiscountModelById(book.id);
                    }

                }

                if (reader["owner_id"] != DBNull.Value)
                {
                    var ownerId = reader.GetInt32(reader.GetOrdinal("owner_id"));
                    ownerIds.Add(ownerId);
                }
            }

            if (bookViewModel == null) return null;

            bookViewModel.ownerUserIds = ownerIds.ToList();

            if (bookViewModel.feedbackModel?.Any() == true)
            {
                foreach (var feedback in bookViewModel.feedbackModel)
                {
                    feedback.userModel = getUserModelById(feedback.userId) ?? new UserModel();
                }
            }

            Console.WriteLine("pubs: ", bookViewModel.publishers.Select(p => p.name));
            return bookViewModel;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching book with ID: {id}, {ex.Message}");
            throw;
        }
    }

    //this func to get book by title
    public GenreModel? getGenreModelById(int Id)
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
    public UserModel? getUserModelById(int Id)
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
                                type = reader["type"]?.ToString()!,
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
    public CoverModel? getCoverModelById(int Id)
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
                                imgName = reader["imgName"].ToString()!,
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
            Console.WriteLine($"Database error during fetching Cover {ex}");
            throw;
        }

        return null;
    }

    //this func to get rating by id
    public RatingModel? getRatingModel(int Id)
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
            Console.WriteLine($"Database error during fetching Rating ${ex}");
            throw;
        }

        return null;
    }

    //this func to get feedback by id
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
                            feedbackModel.userModel = getUserModelById(feedbackModel.userId) ?? new UserModel();
                            fList.Add(feedbackModel);
                        }
                        return fList;
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during fetching Feedback ${ex}");
            throw;
        }
    }

    //this func is used for grtting pubName and return pubId
    public int gettingPublisherIdByName(string name)
    {
        var publisherId = 0;
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT id FROM Publisher WHERE name = @name;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@name", name);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            publisherId = Convert.ToInt32(reader["id"]);

                            return publisherId;
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during getting publisherId  {ex}");
            throw;
        }

        return publisherId;
    }

    //this func to get publisher by id
    public List<PublisherModel> PubModelByBookId(int bookId)
    {
        List<PublisherModel> publishers = [];
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Publisher WHERE bookId = @bookId;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@bookId", bookId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            var publisherModel = new PublisherModel
                            {
                                id = Convert.ToInt32(reader["id"]),
                                name = reader["name"]?.ToString()!,
                                createdAt = Convert.ToDateTime(reader["createdAt"])
                            };

                            publishers.Add(publisherModel);
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during fetching publisher ${ex}");
            throw;
        }

        return publishers;
    }


    //this func to get all publishers
    public List<PublisherModel> getAllPublishers()
    {
        var pubList = new List<PublisherModel>();
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Publisher";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
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
            Console.WriteLine($"Database error during fetching Publisher ${ex}");
            throw;
        }
    }

    //this func to get Aothers by id

    public AuthorModel? getAuthorModelById(int bookId)
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
                                name = reader["name"].ToString()!,
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
            Console.WriteLine($"Database error during fetching Author ${ex}");
            throw;
        }

        return null;
    }

    public int? getAuthorIDByName(string authorName)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Auther WHERE name = @authorName;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@authorName", authorName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        var id = 0;
                        if (reader.Read())
                        {
                            id = Convert.ToInt32(reader["id"]);
                        };

                        return id;
                    }
                }
            }
        }

        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during fetching Author ${ex}");
            throw;
        }

        return null;
    }


    public BookDiscountModel? getBookDiscountModelById(int bookId)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM BookDiscount WHERE bookId = @Id;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {

                    command.Parameters.AddWithValue("@Id", bookId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            var discountModel = new BookDiscountModel
                            {
                                bookId = Convert.ToInt32(reader["bookId"]),
                                discountPrecentage = float.Parse(reader["discountPercentage"].ToString()),
                                saleStartDate = Convert.ToDateTime(reader["saleStartDate"]),
                                saleEndDate = Convert.ToDateTime(reader["saleEndDate"])
                            };

                            return discountModel;
                        }
                    }
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during fetching Author ${ex}");
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
                        while (reader.Read())
                        {
                            genreList.Add(reader["name"].ToString()!);
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
                                publishers = this.PubModelByBookId(book.publisherId),
                                feedbackModel = this.getfeedbackModelById(book.id),
                                rating = this.getRatingModel(book.id) ?? new RatingModel(),
                                coverModel = this.getCoverModelById(book.id) ?? new CoverModel(),
                                genreModel = this.getGenreModelById(book.genreId) ?? new GenreModel(),
                                authorModel = this.getAuthorModelById(book.id)!,
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

                            bookViewModel.authorModel = this.getAuthorModelById(book.id)!;
                            bookViewModelList.Add(bookViewModel);

                        }
                    }
                }
            }

        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during fetching books {ex}");
            throw;
        }
        return bookViewModelList;
    }

    //this func update amount of copies of a book if someome borrows it amountOfCopies--

    public void updateAmountOfCopies(int bookId, int amount)
    {


        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Book SET amountOfCopies = @amount WHERE id = @bookId;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookId", bookId);
                    command.Parameters.AddWithValue("@amount", amount);

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


    //this query to check if amountOfCopies >0
    //if amountOfCopies == 0 canBorrow = false
    public bool checkAmountOfCopies(int bookId)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "select amountOfCopies from Book where id = @bookId and amountOfCopies>0;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookId", bookId);
                    command.ExecuteNonQuery();
                    if (command.ExecuteNonQuery() == 0)
                    {
                        return false;
                    }
                }
            }

        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during checking amount of copies {ex}");
            throw;
        }
        return true;
    }


    //this func to return rentedBook and update amountOfCopies++
    public void returnRentedBook(int bookId)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Book SET amountOfCopies = amountOfCopies + 1 WHERE id = @bookId;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookId", bookId);

                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during returning book {ex}");
            throw;
        }
    }

    //this func to change book book BuyingPrice this func is to backend
    public void changeBuyingPrice(int bookId, double newPrice)
    {
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "UPDATE Book SET buyingPrice = @newPrice WHERE id = @bookId;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookId", bookId);
                    command.Parameters.AddWithValue("@newPrice", newPrice);

                    command.ExecuteNonQuery();
                }
            }
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during changing buying price {ex}");
            throw;
        }
    }

    //this func to add to HistoryBookPriceModel this func is to frontend
    public void addHistoryBookPriceModel(int bookId, double newPrice)
    {
        float buyingPrice = 0;
        //get the current buyingPrice from book
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT buyingPrice FROM Book WHERE id = @bookId;";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookId", bookId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            buyingPrice = reader["buyingPrice"] != DBNull.Value ? Convert.ToSingle(reader["buyingPrice"]) : 0;

                        }
                    }
                }
            }
            Console.WriteLine($"buyingPrice: {buyingPrice}");

        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during getting buying price {ex}");
            throw;
        }

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO HistoryBookPrice (bookId, price, createdAt) VALUES (@bookId, @price, @createdAt);";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookId", bookId);
                    command.Parameters.AddWithValue("@price", buyingPrice);
                    command.Parameters.AddWithValue("@createdAt", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }

            //this for Upfate the buyingPrice in book
            changeBuyingPrice(bookId, newPrice);
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Database error during adding history book price {ex}");
            throw;
        }
    }

    public List<UserModel> GetAllUserModels()
    {
        var allUsers = new List<UserModel>();

        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM [User]";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allUsers.Add(new UserModel
                            {
                                username = reader["Username"]?.ToString(),
                                email = reader["Email"]?.ToString(),
                                firstName = reader["FirstName"]?.ToString(),
                                lastName = reader["LastName"]?.ToString(),
                                phoneNumber = reader["PhoneNumber"]?.ToString(),
                                type = reader["type"]?.ToString()!,
                                id = Convert.ToInt32(reader["id"]),
                                createAt = Convert.ToDateTime(reader["createdAt"])
                            });
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching users: {ex.Message}");
        }

        return allUsers;
    }
}


