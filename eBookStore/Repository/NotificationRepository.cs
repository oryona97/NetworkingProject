using eBookStore.Models;
using Microsoft.Data.SqlClient;

namespace eBookStore.Repository;

public class NotificationRepository
{
  private readonly string? _connectionString;

  public NotificationRepository(string? connectionString)
  {
    _connectionString = connectionString;
  }

  private UserNotificationModel MapToModel(SqlDataReader reader)
  {
    return new UserNotificationModel
    {
      Id = Convert.ToInt32(reader["id"]),
      userId = Convert.ToInt32(reader["userId"]),
      Message = reader["message"].ToString() ?? string.Empty,
      CreatedAt = Convert.ToDateTime(reader["createdAt"])
    };
  }

  public async Task<UserNotificationModel?> GetByIdAsync(int id)
  {
    using (var connection = new SqlConnection(_connectionString))
    {
      await connection.OpenAsync();
      using (var command = new SqlCommand(
          "SELECT * FROM UserNotifications WHERE id = @id", connection))
      {
        command.Parameters.AddWithValue("@id", id);
        using (var reader = await command.ExecuteReaderAsync())
        {
          if (await reader.ReadAsync())
          {
            return MapToModel(reader);
          }
          return null;
        }
      }
    }
  }

  public async Task<IEnumerable<UserNotificationModel>> GetAllAsync()
  {
    var notifications = new List<UserNotificationModel>();
    using (var connection = new SqlConnection(_connectionString))
    {
      await connection.OpenAsync();
      using (var command = new SqlCommand(
          "SELECT * FROM UserNotifications ORDER BY createdAt DESC", connection))
      {
        using (var reader = await command.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            notifications.Add(MapToModel(reader));
          }
        }
      }
    }
    return notifications;
  }

  public async Task<IEnumerable<UserNotificationModel>> GetUserNotificationsAsync(int userId)
  {
    var notifications = new List<UserNotificationModel>();
    using (var connection = new SqlConnection(_connectionString))
    {
      await connection.OpenAsync();
      using (var command = new SqlCommand(
          "SELECT * FROM UserNotifications WHERE userId = @userId ORDER BY createdAt DESC",
          connection))
      {
        command.Parameters.AddWithValue("@userId", userId);
        using (var reader = await command.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            notifications.Add(MapToModel(reader));
          }
        }
      }
    }
    return notifications;
  }

  public async Task<UserNotificationModel> AddAsync(UserNotificationModel notification)
  {
    using (var connection = new SqlConnection(_connectionString))
    {
      await connection.OpenAsync();
      using (var command = new SqlCommand(
          @"INSERT INTO UserNotifications (userId, message, createdAt) 
                  OUTPUT INSERTED.id 
                  VALUES (@userId, @message, @createdAt)", connection))
      {
        command.Parameters.AddWithValue("@userId", notification.userId);
        command.Parameters.AddWithValue("@message", notification.Message);
        command.Parameters.AddWithValue("@createdAt", notification.CreatedAt);

        var result = await command.ExecuteScalarAsync();
        if (result != null)
        {
          notification.Id = Convert.ToInt32(result);
        }
        return notification;
      }
    }
  }

  public async Task<bool> DeleteAsync(int id)
  {
    using (var connection = new SqlConnection(_connectionString))
    {
      await connection.OpenAsync();
      using (var command = new SqlCommand(
          "DELETE FROM UserNotifications WHERE id = @id", connection))
      {
        command.Parameters.AddWithValue("@id", id);
        return await command.ExecuteNonQueryAsync() > 0;
      }
    }
  }

  public async Task<bool> DeleteUserNotificationsAsync(int userId)
  {
    using (var connection = new SqlConnection(_connectionString))
    {
      await connection.OpenAsync();
      using (var command = new SqlCommand(
          "DELETE FROM UserNotifications WHERE userId = @userId", connection))
      {
        command.Parameters.AddWithValue("@userId", userId);
        return await command.ExecuteNonQueryAsync() > 0;
      }
    }
  }

  public async Task<int> GetUnreadNotificationCountAsync(int userId)
  {
    using (var connection = new SqlConnection(_connectionString))
    {
      await connection.OpenAsync();
      using (var command = new SqlCommand(
          "SELECT COUNT(*) FROM UserNotifications WHERE userId = @userId",
          connection))
      {
        command.Parameters.AddWithValue("@userId", userId);
        return Convert.ToInt32(await command.ExecuteScalarAsync());
      }
    }
  }

  public async Task<IEnumerable<UserNotificationModel>> GetRecentNotificationsAsync(int userId, int count)
  {
    var notifications = new List<UserNotificationModel>();
    using (var connection = new SqlConnection(_connectionString))
    {
      await connection.OpenAsync();
      using (var command = new SqlCommand(
          @"SELECT TOP(@count) * 
                  FROM UserNotifications 
                  WHERE userId = @userId 
                  ORDER BY createdAt DESC", connection))
      {
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@count", count);
        using (var reader = await command.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            notifications.Add(MapToModel(reader));
          }
        }
      }
    }
    return notifications;
  }

  public async Task<IEnumerable<UserNotificationModel>> GetNotificationsByDateRangeAsync(
      int userId, DateTime startDate, DateTime endDate)
  {
    var notifications = new List<UserNotificationModel>();
    using (var connection = new SqlConnection(_connectionString))
    {
      await connection.OpenAsync();
      using (var command = new SqlCommand(
          @"SELECT * FROM UserNotifications 
                  WHERE userId = @userId 
                  AND createdAt BETWEEN @startDate AND @endDate 
                  ORDER BY createdAt DESC", connection))
      {
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@startDate", startDate);
        command.Parameters.AddWithValue("@endDate", endDate);

        using (var reader = await command.ExecuteReaderAsync())
        {
          while (await reader.ReadAsync())
          {
            notifications.Add(MapToModel(reader));
          }
        }
      }
    }
    return notifications;
  }
}
