using eBookStore.Models;
using Microsoft.Data.SqlClient;

namespace eBookStore.Repository;

public class QueueRepository
{
    private readonly string? _connectionString;

    public QueueRepository(string? connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<BookRentQueueModel>> GetQueue(int bookId)
    {
        var queue = new List<BookRentQueueModel>();
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                @"SELECT id, bookId, userId, createdAt 
                  FROM BookRentQueue 
                  WHERE bookId = @bookId 
                  ORDER BY createdAt", connection))
            {
                command.Parameters.AddWithValue("@bookId", bookId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        queue.Add(new BookRentQueueModel
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            bookId = reader.GetInt32(reader.GetOrdinal("bookId")),
                            userId = reader.GetInt32(reader.GetOrdinal("userId")),
                            createdAt = reader.GetDateTime(reader.GetOrdinal("createdAt"))
                        });
                    }
                }
            }
        }
        return queue;
    }

    public async Task<BookRentQueueModel?> AddAsync(BookRentQueueModel entry)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                @"INSERT INTO BookRentQueue (userId, bookId, createdAt) 
                  VALUES (@userId, @bookId, @createdAt);
                  SELECT SCOPE_IDENTITY();", connection))
            {
                command.Parameters.AddWithValue("@userId", entry.userId);
                command.Parameters.AddWithValue("@bookId", entry.bookId);
                command.Parameters.AddWithValue("@createdAt", DateTime.UtcNow);

                try
                {
                    var id = Convert.ToInt32(await command.ExecuteScalarAsync());
                    entry.id = id;
                    entry.createdAt = DateTime.UtcNow;
                    return entry;
                }
                catch (SqlException ex) when (ex.Number == 2627) // Violation of PRIMARY KEY constraint
                {
                    return null;
                }
            }
        }
    }

    public async Task<bool> DeleteAsync(int userId, int bookId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                @"DELETE FROM BookRentQueue 
                  WHERE userId = @userId AND bookId = @bookId", connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@bookId", bookId);
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }

    public async Task<int?> GetQueuePosition(int userId, int bookId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                @"WITH QueuePositions AS (
                    SELECT 
                        userId,
                        ROW_NUMBER() OVER (ORDER BY createdAt) as Position
                    FROM BookRentQueue
                    WHERE bookId = @bookId
                )
                SELECT Position 
                FROM QueuePositions 
                WHERE userId = @userId", connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                command.Parameters.AddWithValue("@bookId", bookId);
                var result = await command.ExecuteScalarAsync();
                return result != DBNull.Value ? Convert.ToInt32(result) : null;
            }
        }
    }
}
