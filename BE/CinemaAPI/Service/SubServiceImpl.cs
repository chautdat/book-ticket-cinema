using System.Linq;                   // Cho LINQ (Where, Select, ToList())
using cinema.Models;                  // Import model Sub và MyDbContext

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ cho SubService: CRUD và truy vấn subscription
    /// </summary>
    public class SubServiceImpl : SubService
    {
        private readonly MyDbContext db;  // DbContext chứa DbSet<Sub>

        /// <summary>
        /// Constructor: Dependency Injection nhận MyDbContext
        /// </summary>
        public SubServiceImpl(MyDbContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Tạo mới một gói Sub (subscription)
        /// </summary>
        /// <param name="sub">Đối tượng Sub chứa thông tin gói</param>
        /// <returns>true nếu lưu thành công, false nếu thất bại</returns>
        public bool create(Sub sub)
        {
            db.Subs.Add(sub);               // Thêm Sub vào DbSet
            return db.SaveChanges() > 0;    // Lưu thay đổi và kiểm tra kết quả
        }

        /// <summary>
        /// Lấy danh sách tất cả gói Sub active (Status == true)
        /// </summary>
        /// <returns>List dynamic chứa Id, Name, Created, Status</returns>
        public dynamic findAll()
        {
            return db.Subs
                .Where(s => s.Status == true) // Lọc chỉ những gói đang active
                .Select(s => new
                {
                    Id = s.Id,                // Mã gói
                    Name = s.Name,            // Tên gói
                    Created = s.Created,      // Ngày tạo gói
                    Status = s.Status         // Trạng thái active/inactive
                })
                .ToList(); // Thực thi query và trả về danh sách
        }

        /// <summary>
        /// Lấy chi tiết một gói Sub theo Id với projection
        /// </summary>
        /// <param name="id">Mã gói cần tìm</param>
        /// <returns>dynamic chứa Id, Name, Created, Status hoặc null nếu không tìm thấy</returns>
        public dynamic findById1(int id)
        {
            return db.Subs
                .Where(s => s.Id == id)    // Lọc theo Id
                .Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    Created = s.Created,
                    Status = s.Status
                })
                .FirstOrDefault(); // Lấy phần tử đầu hoặc null
        }

        /// <summary>
        /// Lấy entity Sub theo Id (để update toàn model)
        /// </summary>
        /// <param name="id">Mã gói</param>
        /// <returns>Đối tượng Sub hoặc null</returns>
        public Sub findById(int id)
        {
            return db.Subs
                .FirstOrDefault(s => s.Id == id); // Trả về Sub hoặc null
        }

        /// <summary>
        /// Cập nhật thông tin gói Sub (status, tên, ...)
        /// </summary>
        /// <param name="sub">Đối tượng Sub đã chỉnh sửa</param>
        /// <returns>true nếu cập nhật thành công, false nếu thất bại</returns>
        public bool update(Sub sub)
        {
            db.Subs.Update(sub);            // Đánh dấu entity để update
            return db.SaveChanges() > 0;    // Lưu thay đổi và trả kết quả
        }
    }
}
