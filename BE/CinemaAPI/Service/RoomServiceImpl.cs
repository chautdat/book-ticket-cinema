using System.Linq;                // Cho LINQ như Where, Select, ToList
using cinema.Models;               // Import model Room và MyDbContext

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ cho RoomService: CRUD và truy vấn Room
    /// </summary>
    public class RoomServiceImpl : RoomService
    {
        private readonly MyDbContext db; // DbContext chứa DbSet<Room>

        /// <summary>
        /// Constructor: Dependency Injection nhận MyDbContext từ container
        /// </summary>
        public RoomServiceImpl(MyDbContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Tạo mới phòng chiếu
        /// </summary>
        /// <param name="room">Đối tượng Room chứa thông tin phòng</param>
        /// <returns>true nếu lưu thành công, false nếu thất bại</returns>
        public bool create(Room room)
        {
            db.Rooms.Add(room);               // Thêm Room vào DbSet
            return db.SaveChanges() > 0;      // Lưu và kiểm tra kết quả
        }

        /// <summary>
        /// Lấy danh sách phòng active (Status == true)
        /// </summary>
        /// <returns>List dynamic chứa Id, Name, Cinema, Quantity, Status, CinemaId</returns>
        public dynamic findAll()
        {
            return db.Rooms
                .Where(r => r.Status == true) // Lọc chỉ phòng active
                .Select(r => new
                {
                    Id = r.Id,                   // Mã phòng
                    Name = r.Name,               // Tên phòng
                    Cinema = r.Cinema.Name,      // Tên rạp (navigation)
                    Quantity = r.Quantity,       // Số ghế/phòng
                    Status = r.Status,           // Trạng thái active/inactive
                    CinemaId = r.CinemaId        // Mã rạp
                })
                .ToList(); // Thực thi query, chuyển sang List
        }

        /// <summary>
        /// Lấy chi tiết phòng theo Id, với cùng projection như findAll
        /// </summary>
        /// <param name="id">Mã phòng</param>
        /// <returns>dynamic chứa thông tin phòng hoặc null</returns>
        public dynamic findById1(int id)
        {
            return db.Rooms
                .Where(r => r.Id == id)       // Lọc theo Id
                .Select(r => new
                {
                    Id = r.Id,
                    Name = r.Name,
                    Cinema = r.Cinema.Name,
                    Quantity = r.Quantity,
                    Status = r.Status,
                    CinemaId = r.CinemaId
                })
                .FirstOrDefault(); // Lấy phần tử đầu hoặc null
        }

        /// <summary>
        /// Lấy entity Room theo Id để update nguyên model
        /// </summary>
        public Room findById(int id)
        {
            return db.Rooms
                .FirstOrDefault(r => r.Id == id); // Trả về Room hoặc null
        }

        /// <summary>
        /// Cập nhật thông tin phòng (soft-delete, edit)
        /// </summary>
        /// <param name="room">Đối tượng Room đã sửa đổi</param>
        /// <returns>true nếu cập nhật thành công</returns>
        public bool update(Room room)
        {
            db.Rooms.Update(room);            // Đánh dấu entity để update
            return db.SaveChanges() > 0;    // Lưu và trả về kết quả
        }

        /// <summary>
        /// Lấy danh sách phòng active theo rạp (CinemaId)
        /// </summary>
        /// <param name="cinemaId">Mã rạp</param>
        /// <returns>List dynamic như findAll nhưng chỉ cho CinemaId</returns>
        public dynamic findAllByCinema(int cinemaId)
        {
            return db.Rooms
                .Where(r => r.Status == true && r.CinemaId == cinemaId)
                .Select(r => new
                {
                    Id = r.Id,
                    Name = r.Name,
                    Cinema = r.Cinema.Name,
                    Quantity = r.Quantity,
                    Status = r.Status,
                    CinemaId = r.CinemaId
                })
                .ToList(); // Chuyển kết quả sang List
        }
    }
}
