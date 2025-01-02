using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using eBookStore.Repository;

namespace eBookStore.Services
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;

        public NotificationBackgroundService(IServiceProvider serviceProvider, ILogger<NotificationBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var userRepository = scope.ServiceProvider.GetRequiredService<UserRepository>();
                        await userRepository.NotifyUsersOnBorrowedBooksAsync();
                        _logger.LogInformation("Daily notification check executed successfully at {Time}", DateTime.Now);
                        Console.WriteLine("Daily notification check executed successfully at " + DateTime.Now);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while executing daily notification check.");
                }

                // How many time to wait before executing the next check
                await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
            }
        }
    }
}