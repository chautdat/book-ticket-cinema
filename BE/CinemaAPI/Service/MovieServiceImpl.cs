using System;
using System.Linq;                    // Cho LINQ như Where, Select, Any
using System.Diagnostics;            // Cho Debug.WriteLine nếu cần
using System.Globalization;          // Cho DateTime.ParseExact
using cinema.Models;                  // Import model Movie, Showtime và các navigation property

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ cho MovieService: CRUD và truy vấn Movie cùng Showtimes
    /// </summary>
    public class MovieServiceImpl : MovieService
    {
        private readonly MyDbContext db; // DbContext chứa DbSet<Movies>, DbSet<Showtimes>

        /// <summary>
        /// Constructor inject DbContext
        /// </summary>
        public MovieServiceImpl(MyDbContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Tạo mới Movie
        /// </summary>
        public bool create(Movie movie)
        {
            // Thêm Movie vào DbSet
            db.Movies.Add(movie);
            // Lưu và kiểm tra >0 bản ghi thay đổi
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Lấy danh sách phim active có suất chiếu theo ngày và cinemaId
        /// </summary>
        public dynamic findAll(bool status, string date, int cinemaId)
        {
            // Parse chuỗi date ("dd/MM/yyyy") thành DateTime
            var date1 = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var nowUtc = DateTime.UtcNow;
            Debug.WriteLine(date1); // Ghi log ngày đã parse

            // Query Movie kết hợp Showtimes
            return db.Movies
                .Where(m => m.Status == status
                            && m.Showtimes.Any(s =>
                                s.Status == true &&
                                s.ShowDate.Date == date1.Date &&
                                s.CinemaId == cinemaId)) // Phim có suất active đúng ngày và đúng rạp
                .Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Description,
                    m.Duration,
                    m.Genre,
                    m.ReleaseDate,
                    m.Age,
                    m.Trailer,
                    m.Director,
                    m.Actor,
                    m.Publisher,
                    m.Status,
                    m.Photo,
                    // Chỉ lấy Showtimes phù hợp ngày và cinemaId
                    Showtimes = m.Showtimes
                        .Where(s =>
                            s.Status == true &&
                            s.ShowDate.Date == date1.Date &&
                            s.CinemaId == cinemaId)
                        .Select(s => new
                        {
                            s.Id,
                            s.RoomId,
                            CinemaName = s.Cinema.Name,
                            ShowDate = s.ShowDate.ToString("dd/MM/yyyy HH:mm"),
                            s.SubId,
                            SubName = s.Sub.Name,
                            TotalSeats = db.Seats.Count(se =>
                                se.RoomId == s.RoomId &&
                                se.Status == true),
                            BookedSeats = db.BookingDetails.Count(bd =>
                                bd.Status == true &&
                                bd.Booking.Status == true &&
                                bd.Booking.ShowtimeId == s.Id),
                            ReservedSeats = db.SeatReservations.Count(sr =>
                                sr.ShowtimeId == s.Id &&
                                sr.ExpiresAt > nowUtc),
                            AvailableSeats =
                                db.Seats.Count(se =>
                                    se.RoomId == s.RoomId &&
                                    se.Status == true)
                                - db.BookingDetails.Count(bd =>
                                    bd.Status == true &&
                                    bd.Booking.Status == true &&
                                    bd.Booking.ShowtimeId == s.Id)
                                - db.SeatReservations.Count(sr =>
                                    sr.ShowtimeId == s.Id &&
                                    sr.ExpiresAt > nowUtc),
                            IsSoldOut =
                                db.Seats.Count(se =>
                                    se.RoomId == s.RoomId &&
                                    se.Status == true)
                                <=
                                (
                                    db.BookingDetails.Count(bd =>
                                        bd.Status == true &&
                                        bd.Booking.Status == true &&
                                        bd.Booking.ShowtimeId == s.Id)
                                    + db.SeatReservations.Count(sr =>
                                        sr.ShowtimeId == s.Id &&
                                        sr.ExpiresAt > nowUtc)
                                ),
                        }).ToList(),
                    // Danh sách SubId tất cả showtimes
                    ListSubId = m.Showtimes
                        .Where(s => s.Status == true)
                        .Select(s => s.SubId)
                        .ToList()
                })
                .ToList(); // Thực thi query
        }

        /// <summary>
        /// Lấy danh sách Movie active
        /// </summary>
        public dynamic findAll(bool status)
        {
            var nowUtc = DateTime.UtcNow;

            return db.Movies
                .Where(m => m.Status == status) // Lọc theo status
                .Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Description,
                    m.Duration,
                    m.Genre,
                    m.ReleaseDate,
                    m.Age,
                    m.Trailer,
                    m.Director,
                    m.Actor,
                    m.Publisher,
                    m.Status,
                    m.Photo,
                    // Lấy tất cả showtimes liên quan
                    Showtimes = m.Showtimes.Select(s => new
                    {
                        s.Id,
                        s.RoomId,
                        CinemaName = s.Cinema.Name,
                        ShowDate = s.ShowDate.ToString("dd/MM/yyyy HH:mm"),
                        s.SubId,
                        SubName = s.Sub.Name,
                        TotalSeats = db.Seats.Count(se =>
                            se.RoomId == s.RoomId &&
                            se.Status == true),
                        BookedSeats = db.BookingDetails.Count(bd =>
                            bd.Status == true &&
                            bd.Booking.Status == true &&
                            bd.Booking.ShowtimeId == s.Id),
                        ReservedSeats = db.SeatReservations.Count(sr =>
                            sr.ShowtimeId == s.Id &&
                            sr.ExpiresAt > nowUtc),
                        AvailableSeats =
                            db.Seats.Count(se =>
                                se.RoomId == s.RoomId &&
                                se.Status == true)
                            - db.BookingDetails.Count(bd =>
                                bd.Status == true &&
                                bd.Booking.Status == true &&
                                bd.Booking.ShowtimeId == s.Id)
                            - db.SeatReservations.Count(sr =>
                                sr.ShowtimeId == s.Id &&
                                sr.ExpiresAt > nowUtc),
                        IsSoldOut =
                            db.Seats.Count(se =>
                                se.RoomId == s.RoomId &&
                                se.Status == true)
                            <=
                            (
                                db.BookingDetails.Count(bd =>
                                    bd.Status == true &&
                                    bd.Booking.Status == true &&
                                    bd.Booking.ShowtimeId == s.Id)
                                + db.SeatReservations.Count(sr =>
                                    sr.ShowtimeId == s.Id &&
                                    sr.ExpiresAt > nowUtc)
                            ),
                    }).ToList(),
                    ListSubId = m.Showtimes.Select(s => s.SubId).ToList()
                })
                .ToList();
        }

        /// <summary>
        /// Lấy thông tin chi tiết một Movie theo Id
        /// </summary>
        public dynamic findById(int id)
        {
            var nowUtc = DateTime.UtcNow;

            return db.Movies
                .Where(m => m.Id == id)
                .Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Description,
                    m.Duration,
                    m.Genre,
                    m.ReleaseDate,
                    m.Age,
                    m.Trailer,
                    m.Director,
                    m.Actor,
                    m.Publisher,
                    m.Status,
                    m.Photo,
                    // Lấy tất cả showtimes của phim
                    Showtimes = m.Showtimes.Select(s => new
                    {
                        s.Id,
                        s.RoomId,
                        CinemaName = s.Cinema.Name,
                        ShowDate = s.ShowDate.ToString("dd/MM/yyyy HH:mm"),
                        s.SubId,
                        SubName = s.Sub.Name,
                        TotalSeats = db.Seats.Count(se =>
                            se.RoomId == s.RoomId &&
                            se.Status == true),
                        BookedSeats = db.BookingDetails.Count(bd =>
                            bd.Status == true &&
                            bd.Booking.Status == true &&
                            bd.Booking.ShowtimeId == s.Id),
                        ReservedSeats = db.SeatReservations.Count(sr =>
                            sr.ShowtimeId == s.Id &&
                            sr.ExpiresAt > nowUtc),
                        AvailableSeats =
                            db.Seats.Count(se =>
                                se.RoomId == s.RoomId &&
                                se.Status == true)
                            - db.BookingDetails.Count(bd =>
                                bd.Status == true &&
                                bd.Booking.Status == true &&
                                bd.Booking.ShowtimeId == s.Id)
                            - db.SeatReservations.Count(sr =>
                                sr.ShowtimeId == s.Id &&
                                sr.ExpiresAt > nowUtc),
                        IsSoldOut =
                            db.Seats.Count(se =>
                                se.RoomId == s.RoomId &&
                                se.Status == true)
                            <=
                            (
                                db.BookingDetails.Count(bd =>
                                    bd.Status == true &&
                                    bd.Booking.Status == true &&
                                    bd.Booking.ShowtimeId == s.Id)
                                + db.SeatReservations.Count(sr =>
                                    sr.ShowtimeId == s.Id &&
                                    sr.ExpiresAt > nowUtc)
                            ),
                    }).ToList(),
                    ListSubId = m.Showtimes.Select(s => s.SubId).ToList()
                })
                .FirstOrDefault(); // Lấy 1 hoặc null
        }

        /// <summary>
        /// Lấy phim cụ thể theo ngày, rạp và movieId
        /// </summary>
        public dynamic findMovie(bool status, string date, int cinemaId, int movieId)
        {
            var date1 = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var nowUtc = DateTime.UtcNow;
            Debug.WriteLine(date1);

            return db.Movies
                .Where(m => m.Status == status
                            && m.Id == movieId
                            && m.Showtimes.Any(s =>
                                s.Status == true &&
                                s.ShowDate.Date == date1.Date &&
                                s.CinemaId == cinemaId))
                .Select(m => new
                {
                    m.Id,
                    m.Title,
                    m.Description,
                    m.Duration,
                    m.Genre,
                    m.ReleaseDate,
                    m.Age,
                    m.Trailer,
                    m.Director,
                    m.Actor,
                    m.Publisher,
                    m.Status,
                    m.Photo,
                    Showtimes = m.Showtimes
                        .Where(s =>
                            s.Status == true &&
                            s.ShowDate.Date == date1.Date &&
                            s.CinemaId == cinemaId)
                        .Select(s => new
                        {
                            s.Id,
                            s.RoomId,
                            CinemaName = s.Cinema.Name,
                            ShowDate = s.ShowDate.ToString("dd/MM/yyyy HH:mm"),
                            s.SubId,
                            SubName = s.Sub.Name,
                            TotalSeats = db.Seats.Count(se =>
                                se.RoomId == s.RoomId &&
                                se.Status == true),
                            BookedSeats = db.BookingDetails.Count(bd =>
                                bd.Status == true &&
                                bd.Booking.Status == true &&
                                bd.Booking.ShowtimeId == s.Id),
                            ReservedSeats = db.SeatReservations.Count(sr =>
                                sr.ShowtimeId == s.Id &&
                                sr.ExpiresAt > nowUtc),
                            AvailableSeats =
                                db.Seats.Count(se =>
                                    se.RoomId == s.RoomId &&
                                    se.Status == true)
                                - db.BookingDetails.Count(bd =>
                                    bd.Status == true &&
                                    bd.Booking.Status == true &&
                                    bd.Booking.ShowtimeId == s.Id)
                                - db.SeatReservations.Count(sr =>
                                    sr.ShowtimeId == s.Id &&
                                    sr.ExpiresAt > nowUtc),
                            IsSoldOut =
                                db.Seats.Count(se =>
                                    se.RoomId == s.RoomId &&
                                    se.Status == true)
                                <=
                                (
                                    db.BookingDetails.Count(bd =>
                                        bd.Status == true &&
                                        bd.Booking.Status == true &&
                                        bd.Booking.ShowtimeId == s.Id)
                                    + db.SeatReservations.Count(sr =>
                                        sr.ShowtimeId == s.Id &&
                                        sr.ExpiresAt > nowUtc)
                                ),
                        }).ToList(),
                    ListSubId = m.Showtimes
                        .Where(s => s.Status == true)
                        .Select(s => s.SubId)
                        .ToList()
                })
                .ToList();
        }

        /// <summary>
        /// Lấy đối tượng Movie theo Id (để update toàn bộ entity)
        /// </summary>
        public Movie findById1(int id)
        {
            return db.Movies.FirstOrDefault(m => m.Id == id);
        }

        /// <summary>
        /// Cập nhật Movie
        /// </summary>
        public bool update(Movie movie)
        {
            db.Movies.Update(movie);
            return db.SaveChanges() > 0;
        }
    }
}
