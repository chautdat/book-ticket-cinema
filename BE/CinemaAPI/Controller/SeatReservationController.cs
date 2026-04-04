using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using cinema.Models;
using cinema.Helpers;

namespace cinema.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatReservationController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly IHubContext<SeatHub> _hubContext;
        private const int RESERVATION_MINUTES = 3; // Thời gian giữ chỗ: 3 phút

        public SeatReservationController(MyDbContext context, IHubContext<SeatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        // DTO classes
        public class ReserveSeatRequest
        {
            public string SeatName { get; set; } = string.Empty;
            public int ShowtimeId { get; set; }
            public string SessionId { get; set; } = string.Empty;
        }

        public class ReleaseSeatRequest
        {
            public string SeatName { get; set; } = string.Empty;
            public int ShowtimeId { get; set; }
            public string SessionId { get; set; } = string.Empty;
        }

        public class ReleaseAllRequest
        {
            public int ShowtimeId { get; set; }
            public string SessionId { get; set; } = string.Empty;
        }

        public class SeatStatusResponse
        {
            public string SeatName { get; set; } = string.Empty;
            public string Status { get; set; } = "available"; // "available", "reserved", "booked"
            public string? SessionId { get; set; }
            public DateTime? ExpiresAt { get; set; }
            public bool IsOwnReservation { get; set; }
        }

        /// <summary>
        /// Lấy trạng thái tất cả ghế của một suất chiếu
        /// </summary>
        [HttpGet("status/{showtimeId}")]
        public async Task<IActionResult> GetSeatStatus(int showtimeId, [FromQuery] string sessionId)
        {
            // Xóa các reservation hết hạn trước
            await CleanupExpiredReservations(showtimeId);

            // Lấy danh sách ghế đã đặt (booked)
            var bookedSeats = await _context.BookingDetails
                .Include(bd => bd.Booking)
                .Include(bd => bd.Seat)
                .Where(bd => bd.Booking.ShowtimeId == showtimeId)
                .Select(bd => bd.Seat.Name)
                .ToListAsync();

            // Lấy danh sách ghế đang được giữ tạm (reserved)
            var reservedSeats = await _context.SeatReservations
                .Where(r => r.ShowtimeId == showtimeId && r.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            var result = new List<SeatStatusResponse>();

            // Thêm ghế đã đặt
            foreach (var seat in bookedSeats)
            {
                result.Add(new SeatStatusResponse
                {
                    SeatName = seat,
                    Status = "booked",
                    SessionId = null,
                    ExpiresAt = null,
                    IsOwnReservation = false
                });
            }

            // Thêm ghế đang được giữ tạm
            foreach (var reservation in reservedSeats)
            {
                // Nếu ghế chưa có trong danh sách (không bị trùng với booked)
                if (!result.Any(r => r.SeatName == reservation.SeatName))
                {
                    result.Add(new SeatStatusResponse
                    {
                        SeatName = reservation.SeatName,
                        Status = "reserved",
                        SessionId = reservation.SessionId,
                        ExpiresAt = reservation.ExpiresAt,
                        IsOwnReservation = reservation.SessionId == sessionId
                    });
                }
            }

            return Ok(result);
        }

        /// <summary>
        /// Giữ chỗ tạm thời cho một ghế
        /// </summary>
        [HttpPost("reserve")]
        public async Task<IActionResult> ReserveSeat([FromBody] ReserveSeatRequest request)
        {
            if (string.IsNullOrEmpty(request.SeatName) || request.ShowtimeId <= 0 || string.IsNullOrEmpty(request.SessionId))
            {
                return BadRequest(new { success = false, message = "Thông tin không hợp lệ" });
            }

            // Xóa các reservation hết hạn trước
            await CleanupExpiredReservations(request.ShowtimeId);

            // Kiểm tra ghế đã được đặt chưa (booked)
            var isBooked = await _context.BookingDetails
                .Include(bd => bd.Booking)
                .Include(bd => bd.Seat)
                .AnyAsync(bd => bd.Booking.ShowtimeId == request.ShowtimeId && bd.Seat.Name == request.SeatName);

            if (isBooked)
            {
                return BadRequest(new { success = false, message = "Ghế này đã được đặt" });
            }

            // Kiểm tra ghế có đang được người khác giữ không
            var existingReservation = await _context.SeatReservations
                .FirstOrDefaultAsync(r => 
                    r.ShowtimeId == request.ShowtimeId && 
                    r.SeatName == request.SeatName && 
                    r.ExpiresAt > DateTime.UtcNow);

            if (existingReservation != null)
            {
                // Nếu là của chính người này thì gia hạn thêm
                if (existingReservation.SessionId == request.SessionId)
                {
                    existingReservation.ExpiresAt = DateTime.UtcNow.AddMinutes(RESERVATION_MINUTES);
                    await _context.SaveChangesAsync();

                    return Ok(new { 
                        success = true, 
                        message = "Đã gia hạn giữ chỗ",
                        expiresAt = existingReservation.ExpiresAt
                    });
                }

                // Nếu là của người khác thì từ chối
                return BadRequest(new { 
                    success = false, 
                    message = "Ghế này đang được người khác giữ chỗ",
                    expiresAt = existingReservation.ExpiresAt
                });
            }

            // Tạo reservation mới
            var reservation = new SeatReservation
            {
                SeatName = request.SeatName,
                ShowtimeId = request.ShowtimeId,
                SessionId = request.SessionId,
                ReservedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(RESERVATION_MINUTES)
            };

            _context.SeatReservations.Add(reservation);
            await _context.SaveChangesAsync();

            // Broadcast realtime qua SignalR
            await _hubContext.Clients.Group($"showtime_{request.ShowtimeId}").SendAsync("SeatReserved", new
            {
                seatName = request.SeatName,
                showtimeId = request.ShowtimeId,
                sessionId = request.SessionId,
                expiresAt = reservation.ExpiresAt
            });

            return Ok(new { 
                success = true, 
                message = "Đã giữ chỗ thành công",
                expiresAt = reservation.ExpiresAt
            });
        }

        /// <summary>
        /// Hủy giữ chỗ một ghế
        /// </summary>
        [HttpPost("release")]
        public async Task<IActionResult> ReleaseSeat([FromBody] ReleaseSeatRequest request)
        {
            var reservation = await _context.SeatReservations
                .FirstOrDefaultAsync(r => 
                    r.ShowtimeId == request.ShowtimeId && 
                    r.SeatName == request.SeatName && 
                    r.SessionId == request.SessionId);

            if (reservation == null)
            {
                return Ok(new { success = true, message = "Không có reservation để hủy" });
            }

            _context.SeatReservations.Remove(reservation);
            await _context.SaveChangesAsync();

            // Broadcast realtime qua SignalR
            await _hubContext.Clients.Group($"showtime_{request.ShowtimeId}").SendAsync("SeatReleased", new
            {
                seatName = request.SeatName,
                showtimeId = request.ShowtimeId
            });

            return Ok(new { success = true, message = "Đã hủy giữ chỗ" });
        }

        /// <summary>
        /// Hủy tất cả giữ chỗ của một session
        /// </summary>
        [HttpPost("release-all")]
        public async Task<IActionResult> ReleaseAllSeats([FromBody] ReleaseAllRequest request)
        {
            var reservations = await _context.SeatReservations
                .Where(r => r.ShowtimeId == request.ShowtimeId && r.SessionId == request.SessionId)
                .ToListAsync();

            if (reservations.Any())
            {
                var seatNames = reservations.Select(r => r.SeatName).ToList();
                _context.SeatReservations.RemoveRange(reservations);
                await _context.SaveChangesAsync();

                // Broadcast realtime
                foreach (var seatName in seatNames)
                {
                    await _hubContext.Clients.Group($"showtime_{request.ShowtimeId}").SendAsync("SeatReleased", new
                    {
                        seatName = seatName,
                        showtimeId = request.ShowtimeId
                    });
                }
            }

            return Ok(new { success = true, message = "Đã hủy tất cả giữ chỗ" });
        }

        /// <summary>
        /// Xác nhận đặt vé (xóa reservation sau khi thanh toán thành công)
        /// </summary>
        [HttpPost("confirm/{showtimeId}")]
        public async Task<IActionResult> ConfirmBooking(int showtimeId, [FromQuery] string sessionId)
        {
            var reservations = await _context.SeatReservations
                .Where(r => r.ShowtimeId == showtimeId && r.SessionId == sessionId)
                .ToListAsync();

            if (reservations.Any())
            {
                _context.SeatReservations.RemoveRange(reservations);
                await _context.SaveChangesAsync();
            }

            return Ok(new { success = true, message = "Đã xác nhận đặt vé" });
        }

        /// <summary>
        /// Dọn dẹp các reservation hết hạn
        /// </summary>
        private async Task CleanupExpiredReservations(int showtimeId)
        {
            var expiredReservations = await _context.SeatReservations
                .Where(r => r.ShowtimeId == showtimeId && r.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();

            if (expiredReservations.Any())
            {
                var seatNames = expiredReservations.Select(r => r.SeatName).ToList();
                _context.SeatReservations.RemoveRange(expiredReservations);
                await _context.SaveChangesAsync();

                // Broadcast để các client khác biết ghế đã được mở
                foreach (var seatName in seatNames)
                {
                    await _hubContext.Clients.Group($"showtime_{showtimeId}").SendAsync("SeatReleased", new
                    {
                        seatName = seatName,
                        showtimeId = showtimeId
                    });
                }
            }
        }
    }
}
