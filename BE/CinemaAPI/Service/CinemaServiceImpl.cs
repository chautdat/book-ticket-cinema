using cinema.Models; // Import model Cinema và MyDbContext (nếu đặt ở namespace Models)
using System.Linq;    // Cho phép sử dụng LINQ (Select, ToList())

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ cho CinemaService, tương tác với database để truy vấn Cinema
    /// </summary>
    public class CinemaServiceImpl : CinemaService
    {
        private readonly MyDbContext db; // DbContext chứa DbSet<Cinema>

        /// <summary>
        /// Constructor: Dependency Injection để nhận instance MyDbContext
        /// </summary>
        public CinemaServiceImpl(MyDbContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Lấy danh sách tất cả Cinema, select ra các thuộc tính cần hiển thị
        /// </summary>
        /// <returns>List dynamic chứa Id, Name, City, District và Status của mỗi cinema</returns>
        public dynamic findAll()
        {
            return db.Cinemas
                .Select(c => new
                {
                    Id = c.Id,               // Mã rạp
                    Name = c.Name,           // Tên rạp
                    City = c.City,           // Thành phố
                    District = c.District,   // Quận/huyện
                    Status = c.Status        // Trạng thái (active/inactive)
                })
                .ToList(); // Chuyển kết quả sang List
        }

        /// <summary>
        /// Cập nhật thông tin rạp (name/city/district/status)
        /// </summary>
        public bool update(Cinema cinema)
        {
            var existing = db.Cinemas.FirstOrDefault(c => c.Id == cinema.Id);
            if (existing == null) throw new InvalidOperationException("Rạp không tồn tại");

            // Kiểm tra trùng tên trong cùng thành phố/quận (nếu cần)
            var dup = db.Cinemas.Any(c =>
                c.Id != cinema.Id &&
                c.Name == cinema.Name &&
                c.City == cinema.City &&
                c.District == cinema.District);
            if (dup) throw new InvalidOperationException("Rạp đã tồn tại trong khu vực này");

            existing.Name = cinema.Name;
            existing.City = cinema.City;
            existing.District = cinema.District;
            existing.Status = cinema.Status;

            db.Update(existing);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Soft-delete rạp: đặt Status=false
        /// </summary>
        public bool delete(int id)
        {
            var cinema = db.Cinemas.FirstOrDefault(c => c.Id == id);
            if (cinema == null) return false;

            cinema.Status = false;
            db.Update(cinema);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Tạo rạp mới (mặc định Status=true nếu chưa truyền)
        /// </summary>
        public bool create(Cinema cinema)
        {
            if (!cinema.Status)
            {
                cinema.Status = true;
            }
            var dup = db.Cinemas.Any(c =>
                c.Name == cinema.Name &&
                c.City == cinema.City &&
                c.District == cinema.District);
            if (dup) throw new InvalidOperationException("Rạp đã tồn tại trong khu vực này");

            db.Cinemas.Add(cinema);
            return db.SaveChanges() > 0;
        }
    }
}
