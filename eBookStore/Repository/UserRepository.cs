using eBookStore.Models.ViewModels;
using Microsoft.Data.SqlClient;
using eBookStore.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Repository;

public class UserRepository
{
	string? _connectionString;

	public UserRepository(string? connectionString)
	{
		_connectionString = connectionString;
	}

	public List<UserModel>? GetAll()
	{
		List<UserModel> users = new List<UserModel>();
		try
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM [User];";

				using (SqlCommand command = new SqlCommand(query, connection))
				{

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							var userModel = new UserModel
							{
								username = reader["Username"]?.ToString(),
								email = reader["Email"]?.ToString(),
								firstName = reader["FirstName"]?.ToString(),
								lastName = reader["LastName"]?.ToString(),
								phoneNumber = reader["PhoneNumber"]?.ToString(),
								id = Convert.ToInt32(reader["id"])
							};
							users.Add(userModel);
						}
					}
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine("Database error during login ", ex);
			return null;
		}

		return users;
	}

	public UserModel? GetByUsername(string username)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = "SELECT * FROM [User] WHERE Username = @username;";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					command.Parameters.AddWithValue("@username", username);

					using (SqlDataReader reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							var userModel = new UserModel
							{
								username = reader["Username"]?.ToString(),
								email = reader["Email"]?.ToString(),
								firstName = reader["FirstName"]?.ToString(),
								lastName = reader["LastName"]?.ToString(),
								phoneNumber = reader["PhoneNumber"]?.ToString(),
								id = Convert.ToInt32(reader["id"])
							};

						}
					}
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine("Database error during login ", ex);
		}
		return null;
	}

	public int? Save(UserModel user)
	{
		try
		{
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string queries = @"INSERT INTO [User] 
                    (Username, Password, Email, FirstName, LastName, PhoneNumber)  
                    VALUES (@Username, @Password, @Email, @FirstName, @LastName, @PhoneNumber);";

				using (SqlCommand command = new SqlCommand(queries, connection))
				{
					command.Parameters.AddWithValue("@Username", user.username);
					command.Parameters.AddWithValue("@Password", user.password);
					command.Parameters.AddWithValue("@Email", user.email);
					command.Parameters.AddWithValue("@FirstName", user.firstName);
					command.Parameters.AddWithValue("@LastName", user.lastName);
					command.Parameters.AddWithValue("@PhoneNumber", user.phoneNumber);

					int rowAffect = command.ExecuteNonQuery();
					return rowAffect;
				}
			}
		}
		catch (SqlException ex)
		{
			Console.WriteLine("Registration failed. Please try again.", ex);
		}
		return null;
	}
}
