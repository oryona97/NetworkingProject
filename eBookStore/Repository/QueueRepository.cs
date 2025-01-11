using eBookStore.Models;
using Microsoft.Data.SqlClient;

namespace eBookStore.Repository;

public class QueueRepository
{
    private string? _connectionString;

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
                @"
                SELECT * FROM BookRentQueue brq
                WHERE brq.bookId = @bookId", connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        queue.Add(new BookRentQueueModel
                        {
                            id = reader.GetInt32(reader.GetOrdinal("id")),
                            bookId = reader.GetInt32(reader.GetOrdinal("bookId")),
                            userId = reader.GetInt32(reader.GetOrdinal("userId")),
                        });
                    }
                }
            }
        }

        return queue;
    }

    public async Task<BookRentQueueModel> AddAsync(BookRentQueueModel last)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                @"INSERT INTO BookRentQueue (userId, bookId, createdAt) 
                    OUTPUT INSERTED.id
                    VALUES (@userId, @bookId, @createdAt)", connection))
            {
                command.Parameters.AddWithValue("@userId", last.userId);
                command.Parameters.AddWithValue("@bookId", last.bookId);
                command.Parameters.AddWithValue("@createdAt", last.createdAt);

                var result = await command.ExecuteScalarAsync();
                if (result != null)
                {
                    last.id = Convert.ToInt32(result);
                    return last;
                }
                return last;
            }
        }
    }

    public async Task<bool> UpdateAsync(BookRentQueueModel queue)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                @"UPDATE BookRentQueue 
                    SET userId = @userId, bookId = @bookId
                    WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", queue.id);
                command.Parameters.AddWithValue("@userId", queue.userId);
                command.Parameters.AddWithValue("@bookId", queue.bookId);

                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }

    public async Task<bool> DeleteAsync(int userId, int bookId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                "DELETE FROM BookRentQueue WHERE userId = @userId AND bookId = @bookId", connection))
            {
                command.Parameters.AddWithValue("@bookId", bookId);
                command.Parameters.AddWithValue("@userId", userId);
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }
}
