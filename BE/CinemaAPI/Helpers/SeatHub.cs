using Microsoft.AspNetCore.SignalR;

namespace cinema.Helpers
{
    public class SeatHub : Hub
    {
        /// <summary>
        /// Khi client kết nối vào một suất chiếu để theo dõi ghế
        /// </summary>
        public async Task JoinShowtime(int showtimeId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"showtime_{showtimeId}");
            await Clients.Caller.SendAsync("JoinedShowtime", new { 
                showtimeId = showtimeId,
                connectionId = Context.ConnectionId 
            });
        }

        /// <summary>
        /// Khi client rời khỏi trang đặt vé
        /// </summary>
        public async Task LeaveShowtime(int showtimeId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"showtime_{showtimeId}");
        }

        /// <summary>
        /// Ping để kiểm tra kết nối
        /// </summary>
        public async Task Ping()
        {
            await Clients.Caller.SendAsync("Pong", DateTime.UtcNow);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Khi client ngắt kết nối, có thể xử lý cleanup ở đây nếu cần
            await base.OnDisconnectedAsync(exception);
        }
    }
}
