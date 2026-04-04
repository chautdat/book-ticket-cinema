using cinema.Models;                   // Import các model Booking và BookingDetail
using Microsoft.AspNetCore.Http.HttpResults; // (Unused) có thể dùng cho kết quả HTTP

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ cho BookingService, tương tác với DbContext để CRUD Booking
    /// </summary>
    public class BookingServiceImpl : BookingService
    {
        private readonly MyDbContext db;    // DbContext chứa DbSet<Bookings>, DbSet<BookingDetails>

        public BookingServiceImpl(MyDbContext _db)
        {
            db = _db; // Dependency Injection: nhận MyDbContext từ container
        }

        /// <summary>
        /// Tạo mới một booking chính
        /// </summary>
        public bool create(Booking booking)
        {
            db.Bookings.Add(booking);          // Thêm booking vào DbSet
            return db.SaveChanges() > 0;       // Lưu thay đổi và trả về true nếu thành công
        }

        /// <summary>
        /// Thêm các chi tiết (seat, combo) cho booking
        /// </summary>
        public bool createBookingDetails(BookingDetail bookingDetail)
        {
            db.BookingDetails.Add(bookingDetail); // Thêm chi tiết booking
            return db.SaveChanges() > 0;          // Lưu và trả về kết quả
        }

        /// <summary>
        /// Lấy danh sách tất cả booking, chọn lọc các trường cần thiết
        /// </summary>
        public dynamic findAll()
        {
            // Dùng LINQ để truy vấn và select ra các thông tin summary
            return db.Bookings.Select(b => new
            {
                Id = b.Id,
                Created = b.Created,
                Name = b.Name,
                Movie = b.Showtime.Movie.Title,       // Tiêu đề phim từ navigation property
                Email = b.Email,
                Phone = b.Phone,
                CountTicket = b.BookingDetails.Count(), // Số ghế đã đặt
                CountCombo = b.ComboDetails.Count()     // Số combo đã chọn
            }).ToList(); // Thực thi truy vấn và chuyển về list
        }

        /// <summary>
        /// Lấy chi tiết một booking theo ID, bao gồm thông tin showtime, phòng, chi tiết ghế và combo
        /// </summary>
        public dynamic findById(int id)
        {
            return db.Bookings
                .Where(b => b.Id == id)       // Lọc theo booking ID
                .Select(b => new
                {
                    ShowTime = b.Showtime.ShowDate,           // Ngày chiếu
                    Cinema = b.Showtime.Cinema.Name,          // Tên rạp
                    Movie = b.Showtime.Movie.Title,           // Tiêu đề phim
                    Sub = b.Showtime.Sub.Name,                // Gói thành viên
                    Room = b.Showtime.Room.Name,              // Phòng chiếu
                    BookingDetails = b.BookingDetails
                        .Select(detail => new { Seat = detail.Seat.Name }) // Tên ghế
                        .ToList(),
                    ComboDetails = b.ComboDetails
                        .Select(c => new { Combo = c.Combo.Name, Quantity = c.Quantity }) // Tên & số lượng combo
                        .ToList()
                })
                .FirstOrDefault(); // Lấy kết quả đầu tiên hoặc null
        }

        /// <summary>
        /// Tìm ghế theo tên (ví dụ "A1") và trả về object Seat chứa Id, Price, v.v.
        /// </summary>
        public dynamic findSeatByName(string name)
        {
            return db.Seats
                .Where(s => s.Name == name)  // Lọc theo tên ghế
                .FirstOrDefault();            // Trả về ghế đầu tiên hoặc null
        }

        public bool updateStatus(int id, bool status)
        {
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking == null) return false;
            booking.Status = status;
            db.Bookings.Update(booking);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Xóa booking theo ID, bao gồm cả các BookingDetails và ComboDetails liên quan
        /// </summary>
        public bool delete(int id)
        {
            var booking = db.Bookings.FirstOrDefault(b => b.Id == id);
            if (booking == null) return false;

            // Xóa các BookingDetails liên quan
            var bookingDetails = db.BookingDetails.Where(bd => bd.BookingId == id).ToList();
            db.BookingDetails.RemoveRange(bookingDetails);

            // Xóa các ComboDetails liên quan
            var comboDetails = db.ComboDetails.Where(cd => cd.BookingId == id).ToList();
            db.ComboDetails.RemoveRange(comboDetails);

            // Xóa booking chính
            db.Bookings.Remove(booking);

            return db.SaveChanges() > 0;
        }
    }
}
