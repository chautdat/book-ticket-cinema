using cinema.Models;                 // Import model Follow và MyDbContext
using System.Linq;                   // Cho LINQ như Where, Select, ToList()

namespace cinema.Services
{
    /// <summary>
    /// Triển khai các phương thức của FollowService, tương tác với database để quản lý theo dõi
    /// </summary>
    public class FollowServiceImpl : FollowService
    {
        private readonly MyDbContext db;  // DbContext chứa DbSet<Follows>

        /// <summary>
        /// Constructor: Dependency Injection nhận MyDbContext
        /// </summary>
        public FollowServiceImpl(MyDbContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Tạo mới hoặc cập nhật trạng thái theo dõi (soft-delete và phục hồi)
        /// </summary>
        /// <param name="follow">Đối tượng Follow chứa AccountId và Status</param>
        /// <returns>true nếu lưu thành công, false nếu thất bại</returns>
        public bool create(Follow follow)
        {
            // Dùng Update để thêm mới nếu chưa tồn tại hoặc cập nhật nếu đã có
            db.Follows.Update(follow);
            return db.SaveChanges() > 0;     // Lưu thay đổi
        }

        /// <summary>
        /// Lấy danh sách tất cả theo dõi, bao gồm email người theo dõi
        /// </summary>
        /// <returns>List dynamic với Id, AccountId, Status, Email</returns>
        public dynamic findAll()
        {
            return db.Follows
                .Select(f => new
                {
                    Id = f.Id,                     // Mã record follow
                    AccountId = f.AccountId,       // Mã tài khoản theo dõi
                    Status = f.Status,             // Trạng thái theo dõi (true/false)
                    Email = f.Account.Email        // Email của tài khoản (navigation)
                })
                .ToList();                         // Chuyển kết quả sang List
        }

        /// <summary>
        /// Lấy thông tin theo dõi theo AccountId
        /// </summary>
        /// <param name="id">AccountId cần tìm theo dõi</param>
        /// <returns>dynamic với Id, AccountId, Status hoặc null nếu không tìm thấy</returns>
        public dynamic findById(int id)
        {
            return db.Follows
                .Where(f => f.AccountId == id)  // Lọc theo AccountId
                .Select(f => new
                {
                    Id = f.Id,
                    AccountId = f.AccountId,
                    Status = f.Status
                })
                .FirstOrDefault();               // Lấy phần tử đầu hoặc null
        }
    }
}
