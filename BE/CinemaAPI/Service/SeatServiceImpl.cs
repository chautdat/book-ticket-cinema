using System.Linq;                   // Cho LINQ như Where, Select, ToList
using cinema.Models;                  // Import model Seat và MyDbContext

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ CRUD và truy vấn cho SeatService
    /// </summary>
    public class SeatServiceImpl : SeatService
    {
        private readonly MyDbContext db; // DbContext chứa DbSet<Seat>

        /// <summary>
        /// Constructor: Dependency Injection để nhận instance MyDbContext
        /// </summary>
        public SeatServiceImpl(MyDbContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Tạo mới một ghế (seat)
        /// </summary>
        /// <param name="seat">Đối tượng Seat cần lưu</param>
        /// <returns>true nếu lưu thành công, false nếu thất bại</returns>
        public bool create(Seat seat)
        {
            db.Seats.Add(seat);              // Thêm Seat vào DbSet
            return db.SaveChanges() > 0;     // Lưu và trả kết quả
        }

        /// <summary>
        /// Lấy danh sách tất cả ghế active (Status == true)
        /// </summary>
        /// <returns>List dynamic chứa Id, Name, Room, RoomId, SeatType, Price và Status</returns>
        public dynamic findAll()
        {
            return db.Seats
                .Where(s => s.Status == true) // Lọc chỉ ghế active
                .Select(s => new
                {
                    Id = s.Id,                 // Mã ghế
                    Name = s.Name,             // Tên ghế (ví dụ A1)
                    Room = s.Room.Name,        // Tên phòng (navigation property)
                    RoomId = s.RoomId,         // Mã phòng
                    SeatType = s.SeatType,     // Loại ghế (thường: vip, thường)
                    Price = s.Price,           // Giá ghế
                    Status = s.Status          // Trạng thái active/inactive
                })
                .ToList(); // Thực thi truy vấn, chuyển kết quả sang List
        }

        /// <summary>
        /// Lấy chi tiết một ghế theo Id, trả về projection không bao gồm navigation Room
        /// </summary>
        /// <param name="id">Mã ghế</param>
        /// <returns>dynamic chứa Id, Name, RoomId, SeatType, Price và Status hoặc null</returns>
        public dynamic findById1(int id)
        {
            return db.Seats
                .Where(s => s.Id == id)     // Lọc theo Id
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    RoomID = s.RoomId,
                    SeatType = s.SeatType,
                    Price = s.Price,
                    Status = s.Status
                })
                .FirstOrDefault(); // Lấy phần tử đầu hoặc null
        }

        /// <summary>
        /// Lấy entity Seat đầy đủ theo Id (để update nguyên model)
        /// </summary>
        /// <param name="id">Mã ghế</param>
        /// <returns>Đối tượng Seat hoặc null</returns>
        public Seat findById(int id)
        {
            return db.Seats
                .FirstOrDefault(s => s.Id == id); // Trả về Seat hoặc null
        }

        /// <summary>
        /// Cập nhật thông tin ghế (edit hoặc soft-delete)
        /// </summary>
        /// <param name="seat">Đối tượng Seat đã chỉnh sửa</param>
        /// <returns>true nếu cập nhật thành công, false nếu thất bại</returns>
        public bool update(Seat seat)
        {
            db.Seats.Update(seat);          // Đánh dấu entity để update
            return db.SaveChanges() > 0;    // Lưu và trả kết quả
        }
    }
}
