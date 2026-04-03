using System.Linq;                 // Cho LINQ (Where, Select, Any, Average)
using cinema.Models;                // Import model Rating và MyDbContext

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ cho RatingService: tính toán và truy vấn rating
    /// </summary>
    public class RatingServiceImpl : RatingService
    {
        private readonly MyDbContext db; // DbContext chứa DbSet<Ratings>

        /// <summary>
        /// Constructor: Dependency Injection để nhận instance MyDbContext
        /// </summary>
        public RatingServiceImpl(MyDbContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Tính điểm trung bình của một phim theo movieId
        /// </summary>
        /// <param name="movieId">Mã phim cần tính điểm</param>
        /// <returns>Điểm trung bình (double)</returns>
        public double avgByMovieId(int movieId)
        {
            // Lấy tất cả rating của phim
            var ratings = db.Ratings.Where(r => r.MovieId == movieId).ToList();

            // Tính trung bình nếu có ratings, ngược lại trả 0.0
            double averageRating = ratings.Any() ? ratings.Average(r => r.Rate) : 0.0;
            return averageRating;
        }

        /// <summary>
        /// Tạo mới rating cho phim
        /// </summary>
        /// <param name="rating">Đối tượng Rating cần lưu</param>
        /// <returns>true nếu lưu thành công, false nếu thất bại</returns>
        public dynamic create(Rating rating)
        {
            db.Ratings.Add(rating);          // Thêm rating vào DbSet
            return db.SaveChanges() > 0;     // Lưu và trả kết quả
        }

        /// <summary>
        /// Lấy danh sách rating của một phim theo movieId, sắp xếp giảm dần theo Id
        /// </summary>
        public dynamic findAll(int movieId)
        {
            return db.Ratings
                .Where(r => r.MovieId == movieId)       // Lọc theo movieId
                .OrderByDescending(r => r.Id)           // Sắp xếp giảm dần theo Id
                .Select(r => new                         // Project các trường cần thiết
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rate = r.Rate,
                    AccountId = r.Account.Username,       // Tên tài khoản (navigation property)
                })
                .ToList();                               // Thực thi truy vấn
        }

        /// <summary>
        /// Lấy tất cả rating bất kể movieId, sắp xếp theo tạo thêm
        /// </summary>
        public dynamic findAll()
        {
            return db.Ratings
                .Select(r => new
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rate = r.Rate,
                    AccountId = r.Account.Username,
                })
                .ToList(); // Chuyển sang List
        }

        /// <summary>
        /// Lấy rating của phim theo movieId với filter status
        /// </summary>
        /// <param name="movieId">Mã phim</param>
        /// <param name="status">Trạng thái active/inactive</param>
        public dynamic findAll(int movieId, bool status)
        {
            return db.Ratings
                .Where(r => r.MovieId == movieId && r.Status == status) // Lọc theo movieId và status
                .OrderByDescending(r => r.Id)
                .Select(r => new
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rate = r.Rate,
                    AccountId = r.Account.Username,
                })
                .ToList();
        }
    }
}