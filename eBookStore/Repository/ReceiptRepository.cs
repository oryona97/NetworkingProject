using eBookStore.Models;
using Microsoft.Data.SqlClient;
namespace eBookStore.Repository;

public class ReceiptRepository
{
    private readonly string? _connectionString;

    public ReceiptRepository(string? connectionString)
    {
        _connectionString = connectionString;
    }

    private RecieptModel MapToModel(SqlDataReader reader)
    {
        return new RecieptModel
        {
            id = Convert.ToInt32(reader["id"]),
            userId = Convert.ToInt32(reader["userId"]),
            bookId = Convert.ToInt32(reader["bookId"]),
            total = Convert.ToSingle(reader["total"]),
            createdAt = Convert.ToDateTime(reader["createdAt"])
        };
    }

    public async Task<RecieptModel?> GetByIdAsync(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                "SELECT * FROM Reciept WHERE id = @id", connection))
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

    public async Task<IEnumerable<RecieptModel>> GetAllAsync()
    {
        var receipts = new List<RecieptModel>();
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                "SELECT * FROM Reciept ORDER BY createdAt DESC", connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        receipts.Add(MapToModel(reader));
                    }
                }
            }
        }
        return receipts;
    }

    public async Task<IEnumerable<RecieptModel>> GetByUserIdAsync(int userId)
    {
        var receipts = new List<RecieptModel>();
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                "SELECT * FROM Reciept WHERE userId = @userId ORDER BY createdAt DESC", connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        receipts.Add(MapToModel(reader));
                    }
                }
            }
        }
        return receipts;
    }

    public async Task<RecieptModel> AddAsync(RecieptModel receipt)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                @"INSERT INTO Reciept (userId, bookId, total, createdAt) 
                    OUTPUT INSERTED.id
                    VALUES (@userId, @bookId, @total, @createdAt)", connection))
            {
                command.Parameters.AddWithValue("@userId", receipt.userId);
                command.Parameters.AddWithValue("@bookId", receipt.bookId);
                command.Parameters.AddWithValue("@total", receipt.total);
                command.Parameters.AddWithValue("@createdAt", receipt.createdAt);

                var result = await command.ExecuteScalarAsync();
                if (result != null)
                {
                    receipt.id = Convert.ToInt32(result);
                    return receipt;
                }
                return receipt;
            }
        }
    }

    public async Task<bool> UpdateAsync(RecieptModel receipt)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                @"UPDATE Reciept 
                    SET userId = @userId, bookId = @bookId, total = @total 
                    WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", receipt.id);
                command.Parameters.AddWithValue("@userId", receipt.userId);
                command.Parameters.AddWithValue("@bookId", receipt.bookId);
                command.Parameters.AddWithValue("@total", receipt.total);

                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                "DELETE FROM Reciept WHERE id = @id", connection))
            {
                command.Parameters.AddWithValue("@id", id);
                return await command.ExecuteNonQueryAsync() > 0;
            }
        }
    }

    public async Task<decimal> GetTotalSpentByUserAsync(int userId)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                "SELECT COALESCE(SUM(total), 0.00) FROM Reciept WHERE userId = @userId",
                connection))
            {
                command.Parameters.AddWithValue("@userId", userId);
                var result = await command.ExecuteScalarAsync();
                return result != null ? Convert.ToDecimal(result) : 0m;
            }
        }
    }

    public async Task<IEnumerable<RecieptModel>> GetReceiptsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var receipts = new List<RecieptModel>();
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqlCommand(
                @"SELECT * FROM Reciept 
                    WHERE createdAt BETWEEN @startDate AND @endDate 
                    ORDER BY createdAt DESC", connection))
            {
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        receipts.Add(MapToModel(reader));
                    }
                }
            }
        }
        return receipts;
    }
}
