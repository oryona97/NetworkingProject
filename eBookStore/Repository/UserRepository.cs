using System.Threading.Tasks;
using eBookStore.Models;
using eBookStore.Models.ViewModels;
using eBookStore.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace eBookStore.Repository
{
	public class UserRepository
	{
		private readonly string _connectionString;
		private readonly ILogger<UserRepository> _logger;

		public UserRepository(string? connectionString, ILogger<UserRepository> logger)
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
				string sql = "UPDATE [User] SET type = 'admin' WHERE id = @userId";
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

		public void UpdateUserType(int userId, string newType)
		{
			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();
					using (var command = new SqlCommand("UPDATE [User] SET type = @type WHERE Id = @UserId", connection))
					{
						command.Parameters.AddWithValue("@type", newType);
						command.Parameters.AddWithValue("@UserId", userId);

						int rowsAffected = command.ExecuteNonQuery();

						if (rowsAffected > 0)
						{
							Console.WriteLine($"Successfully updated UserId: {userId} to Type: {newType}");
						}
						else
						{
							Console.WriteLine($"No rows were updated. Check if UserId: {userId} exists.");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error updating UserId: {userId} to Type: {newType}. Exception: {ex.Message}");
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

		//this func for active Trigger to notify users on borrowed books
		public async Task NotifyUsersOnBorrowedBooksAsync()
		{
			try
			{
				using var connection = new SqlConnection(_connectionString);
				await connection.OpenAsync();

				using var command = new SqlCommand("sp_NotifyUsersOnBorrowedBooks", connection)
				{
					CommandType = CommandType.StoredProcedure
				};

				await command.ExecuteNonQueryAsync();
				Console.WriteLine("Notification procedure executed successfully.");
				_logger.LogInformation("Notification procedure executed successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error executing notification procedure.");
				_logger.LogError(ex, "Error executing notification procedure.");
				throw; 
			}
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

		//this func for admin can discount for book
		public void AddDiscountAndUpdateBook(int bookId, float discountPercentage, DateTime saleEndDate)
		{
			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					using (var transaction = connection.BeginTransaction())
					{
						try
						{
							string insertQuery = @"
								INSERT INTO BookDiscount (bookId, discountPercentage, saleStartDate, saleEndDate)
								VALUES (@bookId, @discountPercentage, GETDATE(), @saleEndDate);
							";

							using (var insertCommand = new SqlCommand(insertQuery, connection, transaction))
							{
								insertCommand.Parameters.AddWithValue("@bookId", bookId);
								insertCommand.Parameters.AddWithValue("@discountPercentage", discountPercentage);
								insertCommand.Parameters.AddWithValue("@saleEndDate", saleEndDate);

								insertCommand.ExecuteNonQuery();
							}

							string updateQuery = @"
								UPDATE Book
								SET onSale = 1
								WHERE id = @bookId;
							";

							using (var updateCommand = new SqlCommand(updateQuery, connection, transaction))
							{
								updateCommand.Parameters.AddWithValue("@bookId", bookId);

								updateCommand.ExecuteNonQuery();
							}

							transaction.Commit();

							Console.WriteLine("BookDiscount added and Book table updated successfully.");
							_logger.LogInformation("BookDiscount added and Book table updated successfully for bookId {bookId}", bookId);
						}
						catch (Exception ex)
						{
							transaction.Rollback();
							Console.WriteLine($"Error during adding discount and updating book: {ex.Message}");
							_logger.LogError(ex, "Error during adding discount and updating book for bookId {bookId}", bookId);
							throw;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Database error: {ex.Message}");
				_logger.LogError(ex, "Database error during AddDiscountAndUpdateBook for bookId {bookId}", bookId);
				throw;
			}
		}

		public async Task<bool> ResetPasswordAsync(string email)
		{
			try
			{
				// Check if the email exists in the database
				using var connection = new SqlConnection(_connectionString);
				await connection.OpenAsync();

				const string getUserQuery = "SELECT id FROM [User] WHERE Email = @Email;";
				using var command = new SqlCommand(getUserQuery, connection);
				command.Parameters.AddWithValue("@Email", email);

				var userId = await command.ExecuteScalarAsync();
				if (userId == null)
				{
					return false; // Email not found
				}

				// Generate a new random password
				var newPassword = GenerateRandomPassword();
				var hashedPassword = HashPassword(newPassword);

				// Update the password in the database
				const string updatePasswordQuery = "UPDATE [User] SET password = @Password WHERE id = @UserId;";
				using var updateCommand = new SqlCommand(updatePasswordQuery, connection);
				updateCommand.Parameters.AddWithValue("@Password", hashedPassword);
				updateCommand.Parameters.AddWithValue("@UserId", userId);

				await updateCommand.ExecuteNonQueryAsync();

				// Send the email with the new password
				Console.WriteLine("Sending email with new password to: {0}", email);
				Console.WriteLine("New password: {0}", newPassword);
				SendEmail(email, "Password Reset", $"Your new password is: {newPassword}");

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error resetting password for email: {Email}", email);
				return false;
			}
		}

	// Function to generate a random password
	private string GenerateRandomPassword()
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var random = new RNGCryptoServiceProvider();
		var password = new StringBuilder();
		var data = new byte[4];

		for (int i = 0; i < 8; i++)
		{
			random.GetBytes(data);
			var index = BitConverter.ToUInt32(data, 0) % chars.Length;
			password.Append(chars[(int)index]);
		}

		return password.ToString();
	}

	// Function to hash the password using SHA256
	private string HashPassword(string password)
	{
		using var sha256 = SHA256.Create();
		var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
		return Convert.ToBase64String(hashedBytes);
	}
	//this is the code for sending email ----  JTSA8CEGTD1189NX7D3DCXNQ
	// Function to send an email with the new password
		public void SendEmail(string toEmail, string subject, string body)
		{	
			try{
				var smtpClient = new SmtpClient("smtp.gmail.com") 
				{
					Port = 587,
					Credentials = new NetworkCredential("dor.isreali@gmail.com", "gkbj mvnj uscg hfev"),
					EnableSsl = true,
				};

				var mailMessage = new MailMessage
				{
					From = new MailAddress("dor.isreali@gmail.com"),
					Subject = subject,
					Body = body,
					IsBodyHtml = true,
				};
				mailMessage.To.Add(toEmail);

				smtpClient.Send(mailMessage);
				Console.WriteLine("Email sent successfully to: {0}", toEmail);
			}catch(Exception ex)
			{
				Console.WriteLine("Error sending email: {0}", ex.Message);
				_logger.LogError(ex, "Error sending email to: {ToEmail}", toEmail);
				throw;
			}
		}
		public List<UserNotificationModel> GetUserNotifications(int userId)
		{
			var notifications = new List<UserNotificationModel>();

			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					const string query = @"
						SELECT id, message, createdAt
						FROM UserNotifications
						WHERE userId = @userId
						ORDER BY createdAt DESC;
					";

					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@userId", userId);

						using (var reader = command.ExecuteReader())
						{
							while (reader.Read())
							{
								notifications.Add(new UserNotificationModel
								{
									Id = reader.GetInt32(reader.GetOrdinal("id")),
									Message = reader.GetString(reader.GetOrdinal("message")),
									CreatedAt = reader.GetDateTime(reader.GetOrdinal("createdAt"))
								});
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching notifications for userId {UserId}", userId);
				throw;
			}

			return notifications;
		}
	
		// this funnc to change password
		public bool ChangePassword(int userId, string newPassword)
		{
			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					const string query = @"
						UPDATE [User]
						SET password = @newPassword
						WHERE id = @userId;
					";

					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@newPassword", HashPassword(newPassword));
						command.Parameters.AddWithValue("@userId", userId);

						command.ExecuteNonQuery();
					}
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error changing password for userId {UserId}", userId);
				throw;
			}
		}




		//this func to change user Email
		public void changeEmailById(int userId, string newEmail)
		{
			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					const string query = @"
						UPDATE [User]
						SET email = @newEmail
						WHERE id = @userId;
					";

					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@newEmail", newEmail);
						command.Parameters.AddWithValue("@userId", userId);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error changing email for userId {UserId}", userId);
				throw;
			}
		}

		//this func is for change fist name
		public void changeFirsName(int userId, string FirstName)
		{
			try
			{
			using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					const string query = @"
						UPDATE [User]
						SET FirstName = @FirstName
						WHERE id = @userId;
					";

					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@FirstName", FirstName);
						command.Parameters.AddWithValue("@userId", userId);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error changing email for userId {UserId}", userId);
				throw;
			}
		}



		//this func is for change last name
		public void changelastName(int userId, string lastName)
		{
			try
			{
			using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					const string query = @"
						UPDATE [User]
						SET lastName = @lastName
						WHERE id = @userId;
					";

					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@lastName", lastName);
						command.Parameters.AddWithValue("@userId", userId);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error changing email for userId {UserId}", userId);
				throw;
			}
		}


		public void changePhone(int userId, string phoneNumber)
		{
			try
			{
			using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					const string query = @"
						UPDATE [User]
						SET phoneNumber = @phoneNumber
						WHERE id = @userId;
					";

					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
						command.Parameters.AddWithValue("@userId", userId);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error changing email for userId {UserId}", userId);
				throw;
			}
		}


		//this func to delete user by id

		public void deleteUserById(int userId)
		{
			try
			{
			using (var connection = new SqlConnection(_connectionString))
				{
					connection.Open();

					const string query = @"
						Delete from [User]
						WHERE id = @userId;
					";

					using (var command = new SqlCommand(query, connection))
					{
						command.Parameters.AddWithValue("@userId", userId);

						command.ExecuteNonQuery();
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error Delete user: {UserId}", userId);
				throw;
			}
		}
	}
	
	

	
}
