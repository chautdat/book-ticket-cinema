using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace cinema.Services
{
    public class ShowtimeCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ShowtimeCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(5);

        public ShowtimeCleanupService(
            IServiceProvider serviceProvider,
            ILogger<ShowtimeCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Showtime Cleanup Service is starting.");
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredShowtimes();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during showtime cleanup.");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Showtime Cleanup Service is stopping.");
        }

        private async Task CleanupExpiredShowtimes()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

            var now = DateTime.Now;
            var expiredShowtimes = await dbContext.Showtimes
                .Where(s => s.Status && s.ShowDate < now)
                .ToListAsync();

            if (!expiredShowtimes.Any())
            {
                return;
            }

            foreach (var showtime in expiredShowtimes)
            {
                showtime.Status = false;
            }

            await dbContext.SaveChangesAsync();
            _logger.LogInformation($"Marked {expiredShowtimes.Count} expired showtimes as inactive.");
        }
    }
}
