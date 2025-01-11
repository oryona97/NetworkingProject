using Microsoft.Data.SqlClient;
using eBookStore.Models;
using eBookStore.Models.ViewModels;

namespace eBookStore.Repository;
public class ShoppingCartRepository
{
  private string? connectionString;

  public ShoppingCartRepository(string? _connectionString)
  {
	connectionString = _connectionString;
  }

  //this func get id of user and return the shopping cart of that user 
  public ShoppingCartViewModel GetShoppingCart(int _userId)
  {
	var shoppingCartViewModel = new ShoppingCartViewModel();
	shoppingCartViewModel.shoppingCart = new ShoppingCartModel();
	try
	{
	  using (SqlConnection connection = new SqlConnection(connectionString))
	  {
		connection.Open();
		string query = "SELECT * from BookShoppingCart WHERE userId = @userId";
		using (SqlCommand command = new SqlCommand(query, connection))
		{
		  command.Parameters.AddWithValue("@userId", _userId);
		  using (SqlDataReader reader = command.ExecuteReader())
		  {

			while (reader.Read())
			{
			  shoppingCartViewModel.shoppingCart.userId = _userId;
			  var bookShoppingCart = new BookShoppingCartModel
			  {
				bookId = Convert.ToInt32(reader["BookId"]),
				userId = Convert.ToInt32(reader["userId"]),
				format = reader["format"].ToString(),
				createdAt = reader["createdAt"] != DBNull.Value
				  ? Convert.ToDateTime(reader["createdAt"])
				  : DateTime.MinValue

			  };
			  shoppingCartViewModel.shoppingCart.Books.Add(bookShoppingCart);
			}


		  }
		}
	  }
	}
	catch (Exception ex)
	{
	  Console.WriteLine($"Error fetching shopping cart: {ex.Message}");
	}


	return shoppingCartViewModel;
  }

  //this func add book to shopping cart
  public void AddToShoppingCart(int userId, int bookId, string format)
  {
	try
	{
	  using (SqlConnection connection = new SqlConnection(connectionString))
	  {
		connection.Open();


		string query = "INSERT INTO BookShoppingCart ( bookId,userId, format, createdAt) VALUES (@bookId ,@userId ,@format ,@createdAt)";
		using (SqlCommand command = new SqlCommand(query, connection))
		{
		  command.Parameters.AddWithValue("@bookId", bookId);
		  command.Parameters.AddWithValue("@userId", userId);
		  command.Parameters.AddWithValue("@format", format);
		  command.Parameters.AddWithValue("@createdAt", DateTime.Now);
		  command.ExecuteNonQuery();
		}
	  }
	}
	catch (Exception ex)
	{
	  Console.WriteLine($"Error adding to shopping cart: {ex.Message}");
	}
  }

  //this func remove one book from shopping cart
  public void RemoveOneFromShoppingCart(int userId, int bookId)
  {
	try
	{
	  using (SqlConnection connection = new SqlConnection(connectionString))
	  {
		connection.Open();
		string query = "DELETE FROM BookShoppingCart WHERE userId = @userId AND bookId = @bookId";
		using (SqlCommand command = new SqlCommand(query, connection))
		{
		  command.Parameters.AddWithValue("@userId", userId);
		  command.Parameters.AddWithValue("@bookId", bookId);
		  command.ExecuteNonQuery();
		}
	  }
	}
	catch (Exception ex)
	{
	  Console.WriteLine($"Error removing from shopping cart: {ex.Message}");
	}
  }

  //this func remove all books from shopping cart
  public void RemoveAllFromShoppingCart(int userId)
  {
	try
	{
	  using (SqlConnection connection = new SqlConnection(connectionString))
	  {
		connection.Open();
		string query = "DELETE FROM BookShoppingCart WHERE userId = @userId";
		using (SqlCommand command = new SqlCommand(query, connection))
		{
		  command.Parameters.AddWithValue("@userId", userId);
		  command.ExecuteNonQuery();
		}
	  }
	}
	catch (Exception ex)
	{
	  Console.WriteLine($"Error removing from shopping cart: {ex.Message}");
	}
  }

}
