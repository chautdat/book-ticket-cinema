using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using cinema.Helpers;
using cinema.Models;
using Microsoft.EntityFrameworkCore;

namespace cinema.Services
{
    public class SeatReservationCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<SeatHub> _hubContext;
        private readonly ILogger<SeatReservationCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromSeconds(30);

        public SeatReservationCleanupService(
            IServiceProvider serviceProvider,
            IHubContext<SeatHub> hubContext,
            ILogger<SeatReservationCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _hubContext = hubContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Seat Reservation Cleanup Service is starting.");
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupExpiredReservations();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during seat reservation cleanup.");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }

            _logger.LogInformation("Seat Reservation Cleanup Service is stopping.");
        }

        private async Task CleanupExpiredReservations()
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();

            var now = DateTime.UtcNow;
            var expiredReservations = await dbContext.SeatReservations
                .Where(r => r.ExpiresAt < now)
                .ToListAsync();

            if (expiredReservations.Any())
            {
                _logger.LogInformation($"Cleaning up {expiredReservations.Count} expired seat reservations.");

                // Nhóm theo showtimeId để gửi notification
                var groupedByShowtime = expiredReservations.GroupBy(r => r.ShowtimeId);

                foreach (var group in groupedByShowtime)
                {
                    var showtimeId = group.Key;
                    var seats = group.ToList();

                    // Xóa khỏi database
                    dbContext.SeatReservations.RemoveRange(seats);
                    
                    // Gửi notification qua SignalR
                    foreach (var seat in seats)
                    {
                        await _hubContext.Clients
                            .Group($"showtime_{showtimeId}")
                            .SendAsync("SeatReleased", new 
                            { 
                                seatName = seat.SeatName,
                                reason = "expired"
                            });
                        
                        _logger.LogInformation($"Released expired seat: {seat.SeatName} for showtime {showtimeId}");
                    }
                }

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
