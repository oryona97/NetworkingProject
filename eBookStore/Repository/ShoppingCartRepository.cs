using Microsoft.Data.SqlClient;
using eBookStore.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace eBookStore.Repository;
public class ShoppingCartRepository
{
	private string? connectionString;

	public ShoppingCartRepository(string _connectionString)
	{
		connectionString = _connectionString;
	}
    public ShoppingCartModel GetShoppingCart(int userId)
    {
        var shoppingCart = new ShoppingCartModel
        {
            userId = userId,
            createdAt = DateTime.Now,
            Books = new List<BookShoppingCartModel>()
        };

        using (var connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = @"
                SELECT sc.userId, sc.createdAt AS CartCreatedAt, 
                bsc.bookId, bsc.bookShoppingCartId, bsc.Format, bsc.createdAt AS BookAddedAt
                FROM ShoppingCart sc
                JOIN BookShoppingCart bsc ON sc.bookId = bsc.bookId
                WHERE sc.userId = @userId";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@userId", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var book = new BookShoppingCartModel
                        {
                            bookId = Convert.ToInt32(reader["bookId"]),
                            bookShoppingCartId = Convert.ToInt32(reader["bookShoppingCartId"]),
                            format = reader["Format"].ToString(),
                            
                        };

                        shoppingCart.Books.Add(book);
                    }
                }
            }
        }

        return shoppingCart;
    }
}