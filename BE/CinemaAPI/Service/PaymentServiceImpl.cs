using cinema.Models;              // Import model Payment và MyDbContext
using System.Linq;               // Cho LINQ như Where, Select, ToList
using Microsoft.EntityFrameworkCore;

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ thanh toán PaymentService: tạo và truy vấn Payments
    /// </summary>
    public class PaymentServiceImpl : PaymentService
    {
        private readonly MyDbContext db;  // DbContext chứa DbSet<Payment>

        /// <summary>
        /// Constructor: Dependency Injection để nhận MyDbContext
        /// </summary>
        public PaymentServiceImpl(MyDbContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Tạo mới một Payment record
        /// </summary>
        /// <param name="payment">Đối tượng Payment chứa thông tin thanh toán</param>
        /// <returns>true nếu lưu thành công, false nếu thất bại</returns>
        public bool create(Payment payment)
        {
            // Thêm payment vào DbSet
            db.Payments.Add(payment);
            // Lưu và trả về true nếu có thay đổi
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Lấy danh sách tất cả payment đang active (Status == true)
        /// </summary>
        /// <returns>List dynamic chứa thông tin cơ bản của payment</returns>
        public dynamic findAll()
        {
            return db.Payments
                .Where(p => p.Status == true) // Lọc payments active
                .Select(p => new
                {
                    Id = p.Id,                            // Mã payment
                    BookingId = p.BookingId,              // Mã booking liên kết
                    PaymentType = p.PaymentType,          // Loại thanh toán
                    TransactionNo = p.TransactionNo,      // Mã giao dịch
                    TicketNumber = p.TicketNumber,        // Số vé
                    Qr = p.Qr,                            // Dữ liệu QR code
                    Created = p.Created,                  // Ngày tạo
                    Description = p.Description,          // Mô tả thanh toán
                    Price = p.Price,                      // Giá trị thanh toán
                    Status = p.Status                     // Trạng thái active/inactive
                })
                .ToList(); // Chuyển kết quả sang List
        }

        /// <summary>
        /// Lấy chi tiết Payment theo Id,
        /// bao gồm thông tin mua vé từ Booking và ShowTime
        /// </summary>
        /// <param name="id">Mã payment cần truy vấn</param>
        /// <returns>dynamic chứa thông tin chi tiết thanh toán và booking liên quan</returns>
        public dynamic findById(int id)
        {
            return db.Payments
                .Where(p => p.Id == id) // Lọc theo payment Id
                .Select(p => new
                {
                    Id = p.Id,
                    Cinema = p.Booking.Showtime.Cinema.Name,           // Tên rạp từ booking liên kết
                    ShowDate = p.Booking.Showtime.ShowDate.ToString("dd/MM/yyyy"), // Ngày chiếu
                    ShowTime = p.Booking.Showtime.ShowDate.ToString("HH:mm"),     // Giờ chiếu
                    SubName = p.Booking.Showtime.Sub.Name,              // Tên gói subscription
                    Title = p.Booking.Showtime.Movie.Title,             // Tiêu đề phim
                    Photo = p.Booking.Showtime.Movie.Photo,             // URL ảnh phim
                    Duration = p.Booking.Showtime.Movie.Duration,       // Thời lượng phim
                    TicketNumber = p.TicketNumber,                      // Số vé
                    Price = p.Price,                                    // Tổng giá
                    TransactionNo = p.TransactionNo,                    // Mã GD
                    Room = p.Booking.Showtime.Room.Name,                // Phòng chiếu
                    BookingDetails = p.Booking.BookingDetails          // Danh sách chi tiết booking (ghế)
                        .Select(b => new { seatId = b.Seat.Name })
                })
                .FirstOrDefault(); // Lấy record đầu hoặc null nếu không tìm thấy
        }

        /// <summary>
        /// Lấy lịch sử thanh toán/đặt vé theo email người đặt
        /// </summary>
        public dynamic findByEmail(string email, string? phone = null)
        {
            var normalizedEmail = (email ?? string.Empty).Trim().ToLower();
            var normalizedPhone = NormalizePhone(phone);

            if (string.IsNullOrEmpty(normalizedEmail) && string.IsNullOrEmpty(normalizedPhone))
            {
                return new List<object>();
            }

            var payments = db.Payments
                .AsNoTracking()
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Movie)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Cinema)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.Showtime)
                        .ThenInclude(s => s.Room)
                .Include(p => p.Booking)
                    .ThenInclude(b => b.BookingDetails)
                        .ThenInclude(bd => bd.Seat)
                .Where(p => p.Booking != null)
                .OrderByDescending(p => p.Created)
                .ToList();

            var filteredPayments = payments.Where(p =>
            {
                var bookingEmail = (p.Booking?.Email ?? string.Empty).Trim().ToLower();
                var bookingPhone = NormalizePhone(p.Booking?.Phone);

                var matchedEmail = !string.IsNullOrEmpty(normalizedEmail) && bookingEmail == normalizedEmail;
                var matchedPhone = !string.IsNullOrEmpty(normalizedPhone) && bookingPhone == normalizedPhone;

                return matchedEmail || matchedPhone;
            });

            return filteredPayments.Select(p => new
                {
                    PaymentId = p.Id,
                    BookingId = p.BookingId,
                    PaymentType = p.PaymentType,
                    TransactionNo = p.TransactionNo,
                    TicketNumber = p.TicketNumber,
                    Price = p.Price,
                    Created = p.Created,
                    Description = p.Description,
                    Movie = p.Booking?.Showtime?.Movie?.Title ?? "Không rõ phim",
                    Photo = p.Booking?.Showtime?.Movie?.Photo ?? string.Empty,
                    Cinema = p.Booking?.Showtime?.Cinema?.Name ?? "Không rõ rạp",
                    Room = p.Booking?.Showtime?.Room?.Name ?? "Không rõ phòng",
                    ShowDate = p.Booking?.Showtime != null
                        ? p.Booking.Showtime.ShowDate.ToString("dd/MM/yyyy")
                        : string.Empty,
                    ShowTime = p.Booking?.Showtime != null
                        ? p.Booking.Showtime.ShowDate.ToString("HH:mm")
                        : string.Empty,
                    Seats = p.Booking?.BookingDetails?
                        .Where(b => b.Seat != null && !string.IsNullOrWhiteSpace(b.Seat.Name))
                        .Select(b => b.Seat.Name)
                        .ToList() ?? new List<string>()
                })
                .ToList();
        }

        private static string NormalizePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return string.Empty;
            }

            return new string(phone.Where(char.IsDigit).ToArray());
        }

        public bool updateStatus(int id, bool status, string? transactionNo = null, int? paymentType = null)
        {
            var payment = db.Payments.FirstOrDefault(p => p.Id == id);
            if (payment == null) return false;
            payment.Status = status;
            if (!string.IsNullOrEmpty(transactionNo))
            {
                payment.TransactionNo = transactionNo;
            }
            if (paymentType.HasValue)
            {
                payment.PaymentType = paymentType.Value;
            }
            db.Payments.Update(payment);
            return db.SaveChanges() > 0;
        }

        public Payment? findPaymentEntityById(int id)
        {
            return db.Payments.FirstOrDefault(p => p.Id == id);
        }

        public Payment? findByTransactionNo(string transNo)
        {
            return db.Payments.FirstOrDefault(p => p.TransactionNo == transNo);
        }

        /// <summary>
        /// Xóa Payment theo Id (HARD DELETE - xóa hoàn toàn khỏi database)
        /// </summary>
        /// <param name="id">Mã payment cần xóa</param>
        /// <returns>true nếu xóa thành công, false nếu không tìm thấy hoặc lỗi</returns>
        public bool delete(int id)
        {
            var payment = db.Payments.FirstOrDefault(p => p.Id == id);
            if (payment == null) return false;
            
            // Hard delete: XÓA HOÀN TOÀN khỏi database
            db.Payments.Remove(payment);
            return db.SaveChanges() > 0;
        }
    }
}
