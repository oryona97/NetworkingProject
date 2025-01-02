using eBookStore.Models;
using eBookStore.Models.ViewModels;
using eBookStore.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace eBookStore.Repository
{
	public class UserRepository
	{
		private readonly string _connectionString;
		private readonly ILogger<UserRepository> _logger;

		public UserRepository(string connectionString, ILogger<UserRepository> logger)
		{
			_connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public async Task<List<UserModel>> GetAllAsync()
		{
			var users = new List<UserModel>();
			try
			{
				using var connection = new SqlConnection(_connectionString);
				await connection.OpenAsync();

				const string query = @"
                    SELECT id, Username, Email, FirstName, LastName, PhoneNumber 
                    FROM [User];";

				using var command = new SqlCommand(query, connection);
				using var reader = await command.ExecuteReaderAsync();

				while (await reader.ReadAsync())
				{
					users.Add(MapUserFromReader(reader));
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving all users");
				throw;
			}
			return users;
		}

		public async Task<UserModel?> GetByUsernameAsync(string username)
		{
			try
			{
				using var connection = new SqlConnection(_connectionString);
				await connection.OpenAsync();

				const string query = @"
                    SELECT id, Username, Email, FirstName, LastName, PhoneNumber 
                    FROM [User] 
                    WHERE Username = @username;";

				using var command = new SqlCommand(query, connection);
				command.Parameters.AddWithValue("@username", username);

				using var reader = await command.ExecuteReaderAsync();

				if (await reader.ReadAsync())
				{
					return MapUserFromReader(reader);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error retrieving user by username: {Username}", username);
				throw;
			}
			return null;
		}


		//this func for admin can make other user to be admin
		public bool UpdateUserToAdmin(int userId)
		{
			try
			{
				using SqlConnection connection = new SqlConnection(_connectionString);
				connection.Open();
				string sql = "UPDATE [User] SET type = admin WHERE id = @userId";
				using SqlCommand command = new SqlCommand(sql, connection);
				command.Parameters.AddWithValue("@userId", userId);
				command.ExecuteNonQuery();
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error updating user to admin: {0}", userId);
				_logger.LogError(ex, "Error updating user to admin: {UserId}", userId);
				throw;
			}
		}

		//this func for admin can add book to db
		public bool AddBookViewModel(BookViewModel bookViewModel)
		{
			try
			{
				BookRepository bookRepository = new BookRepository(_connectionString);
				bookRepository.AddBookViewModel(bookViewModel);

			}catch(Exception ex)
			{
				Console.WriteLine("Error adding book: {0}", bookViewModel);
				_logger.LogError(ex, "Error adding book: {BookModel}", new { bookViewModel });
				throw;
			}
			return true;
		}




		public async Task<int> SaveAsync(UserModel user)
		{
			ArgumentNullException.ThrowIfNull(user);

			try
			{
				using var connection = new SqlConnection(_connectionString);
				await connection.OpenAsync();

				const string query = @"
                    INSERT INTO [User] (
                        Username, Password, Email, FirstName, LastName, PhoneNumber
                    )  
                    VALUES (
                        @Username, @Password, @Email, @FirstName, @LastName, @PhoneNumber
                    );
                    SELECT SCOPE_IDENTITY();";

				using var command = new SqlCommand(query, connection);
				AddUserParameters(command, user);

				var result = await command.ExecuteScalarAsync();
				return Convert.ToInt32(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error saving user: {@User}", new { user.username, user.email });
				throw;
			}
		}

		private static UserModel MapUserFromReader(SqlDataReader reader)
		{
			return new UserModel
			{
				id = reader.GetInt32(reader.GetOrdinal("id")),
				username = reader.GetString(reader.GetOrdinal("Username")),
				email = reader.GetString(reader.GetOrdinal("Email")),
				firstName = reader.IsDBNull(reader.GetOrdinal("FirstName")) ? null : reader.GetString(reader.GetOrdinal("FirstName")),
				lastName = reader.IsDBNull(reader.GetOrdinal("LastName")) ? null : reader.GetString(reader.GetOrdinal("LastName")),
				phoneNumber = reader.IsDBNull(reader.GetOrdinal("PhoneNumber")) ? null : reader.GetString(reader.GetOrdinal("PhoneNumber"))
			};
		}

		private static void AddUserParameters(SqlCommand command, UserModel user)
		{
			command.Parameters.Add("@Username", SqlDbType.NVarChar).Value = user.username ?? throw new ArgumentException("Username is required");
			command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = user.password ?? throw new ArgumentException("Password is required");
			command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = user.email ?? throw new ArgumentException("Email is required");
			command.Parameters.Add("@FirstName", SqlDbType.NVarChar).Value = (object?)user.firstName ?? DBNull.Value;
			command.Parameters.Add("@LastName", SqlDbType.NVarChar).Value = (object?)user.lastName ?? DBNull.Value;
			command.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar).Value = (object?)user.phoneNumber ?? DBNull.Value;
		}
	}
}
