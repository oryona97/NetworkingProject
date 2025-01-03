using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace eBookStore.Services
{
    public class SaleEndDateCheckerService : BackgroundService
    {
        private readonly string _connectionString;
        private readonly ILogger<SaleEndDateCheckerService> _logger;

        public SaleEndDateCheckerService(IConfiguration configuration, ILogger<SaleEndDateCheckerService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndUpdateSalesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking sale end dates.");
                }

                // Run the task daily
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        private async Task CheckAndUpdateSalesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Delete expired discounts and update onSale
            const string query = @"
                DELETE FROM BookDiscount 
                OUTPUT DELETED.bookId 
                WHERE saleEndDate < GETDATE();

                UPDATE Book
                SET onSale = 0
                WHERE id IN (
                    SELECT bookId
                    FROM BookDiscount
                    WHERE saleEndDate < GETDATE()
                );
            ";

            using var command = new SqlCommand(query, connection);

            var affectedRows = await command.ExecuteNonQueryAsync();
            _logger.LogInformation("{Rows} rows processed during sale end date check.", affectedRows);
        }
    }
}