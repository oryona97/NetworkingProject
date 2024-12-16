using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Repository;
public class BookRepository
{
	private string? connectionString;

	public BookRepository(string _connectionString)
	{
		connectionString = _connectionString;
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
								feedbackModel = new FeedbackModel
								{
									bookId = book.id,
									comment = "No feedback yet", 
									createdAt = DateTime.Now
								},
								rating = new RatingModel
								{
									bookId = book.id,
									userId = 0, 
									createdAt = DateTime.Now
								}
							};

							
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
					// הוספת פרמטר לשאילתה
					command.Parameters.AddWithValue("@Id", Id);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							// יצירת אובייקט PublisherModel מהתוצאה
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