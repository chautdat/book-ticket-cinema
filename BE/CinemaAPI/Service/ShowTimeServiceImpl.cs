using System;
using System.Collections.Generic;
using System.Linq;                        // Cho LINQ như Where, Select, Any
using System.Globalization;              // Cho DateTime.ParseExact
using cinema.Models;                      // Import model Showtime và MyDbContext

namespace cinema.Services
{
    /// <summary>
    /// Triển khai nghiệp vụ cho ShowTimeService: CRUD, truy vấn và kiểm tra ghế
    /// </summary>
    public class ShowTimeServiceImpl : ShowTimeService
    {
        private readonly MyDbContext db; // DbContext chứa DbSet<Showtimes>

        /// <summary>
        /// Constructor: Dependency Injection nhận MyDbContext từ container
        /// </summary>
        public ShowTimeServiceImpl(MyDbContext _db)
        {
            db = _db;
        }

        /// <summary>
        /// Kiểm tra danh sách ghế đã đặt cho một suất chiếu
        /// </summary>
        /// <param name="id">Mã showtime</param>
        /// <returns>dynamic bao gồm danh sách BookingDetails (SeatId, SeatName)</returns>
        public dynamic checkSeat(int id)
        {
            return db.Showtimes
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    Bookings = s.Bookings.Select(b => new  // Với mỗi booking
                    {
                        BookingDetails = b.BookingDetails
                            .Select(d => new
                            {
                                SeatId = d.SeatId,      // Mã ghế
                                SeatName = d.Seat.Name   // Tên ghế
                            }).ToList()
                    })
                })
                .FirstOrDefault(); // Lấy showtime đầu tiên hoặc null
        }

        /// <summary>
        /// Lấy chi tiết một showtime theo Id, bao gồm thông tin phim, rạp, phòng, subscription
        /// </summary>
        public dynamic findById(int id)
        {
            return db.Showtimes
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    Id = s.Id,
                    Title = s.Movie.Title,                // Tiêu đề phim
                    MovieId = s.MovieId,
                    Description = s.Movie.Description,
                    Duration = s.Movie.Duration,
                    Genre = s.Movie.Genre,
                    ReleaseDate = s.Movie.ReleaseDate,
                    Age = s.Movie.Age,
                    Trailer = s.Movie.Trailer,
                    Director = s.Movie.Director,
                    Actor = s.Movie.Actor,
                    Publisher = s.Movie.Publisher,
                    Photo = s.Movie.Photo,
                    CinemaId = s.CinemaId,
                    CinemaName = s.Cinema.Name,           // Tên rạp
                    RoomId = s.RoomId,
                    ShowDate = s.ShowDate,                // Ngày-giờ đầy đủ
                    ShowTime = s.ShowDate.ToString("HH:mm"), // Chỉ giờ:phút
                    SubId = s.SubId,
                    SubName = s.Sub.Name,                 // Tên gói
                    Status = s.Status
                })
                .FirstOrDefault(); // Lấy một record hoặc null
        }

        /// <summary>
        /// Lấy đối tượng Showtime nguyên thể (để update)
        /// </summary>
        public Showtime findById1(int id)
        {
            return db.Showtimes.FirstOrDefault(s => s.Id == id);
        }

        /// <summary>
        /// Lấy danh sách tất cả showtimes active
        /// </summary>
        public dynamic findAll()
        {
            return db.Showtimes
                .Where(s => s.Status == true)         // Chỉ showtime active
                .Select(s => new
                {
                    Id = s.Id,
                    Title = s.Movie.Title,
                    Description = s.Movie.Description,
                    Duration = s.Movie.Duration,
                    Genre = s.Movie.Genre,
                    ReleaseDate = s.Movie.ReleaseDate,
                    Age = s.Movie.Age,
                    Trailer = s.Movie.Trailer,
                    Director = s.Movie.Director,
                    Actor = s.Movie.Actor,
                    Publisher = s.Movie.Publisher,
                    Photo = s.Movie.Photo,
                    CinemaName = s.Cinema.Name,
                    ShowDate = s.ShowDate,
                    ShowTime = s.ShowDate.ToString("HH:mm"),
                    SubId = s.SubId,
                    roomId = s.RoomId,
                    RoomName = s.Room.Name,
                    SubName = s.Sub.Name,
                    Status = s.Status
                })
                .ToList(); // Thực thi và trả về List
        }

        /// <summary>
        /// Lấy danh sách showtimes theo ngày, rạp và phim cụ thể
        /// </summary>
        public dynamic findMovie(bool status, string date, int cinemaId, int movieId)
        {
            // Parse ngày đầu vào dd/MM/yyyy
            var date1 = DateTime.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            Console.WriteLine(date1); // Log ngày parse

            return db.Showtimes
                .Where(s => s.Status == status
                            && s.ShowDate.Date == date1.Date
                            && s.CinemaId == cinemaId
                            && s.MovieId == movieId)    // Điều kiện lọc
                .Select(s => new
                {
                    Id = s.Id,
                    Title = s.Movie.Title,
                    Description = s.Movie.Description,
                    Duration = s.Movie.Duration,
                    Genre = s.Movie.Genre,
                    ReleaseDate = s.Movie.ReleaseDate,
                    Age = s.Movie.Age,
                    Trailer = s.Movie.Trailer,
                    Director = s.Movie.Director,
                    Actor = s.Movie.Actor,
                    Publisher = s.Movie.Publisher,
                    Photo = s.Movie.Photo,
                    CinemaId = s.Cinema.Id,
                    CinemaName = s.Cinema.Name,
                    ShowDate = s.ShowDate,
                    ShowTime = s.ShowDate.ToString("HH:mm"),
                    SubId = s.SubId,
                    RoomId = s.RoomId,
                    RoomName = s.Room.Name,
                    SubName = s.Sub.Name
                })
                .ToList(); // Chuyển kết quả sang List
        }

        /// <summary>
        /// Lấy danh sách showtimes theo cinemaId
        /// </summary>
        public dynamic findAllByCinemaId(int cinemaId)
        {
            return db.Showtimes
                .Where(s => s.Status == true && s.CinemaId == cinemaId)
                .Select(s => new
                {
                    Id = s.Id,
                    Title = s.Movie.Title,
                    Description = s.Movie.Description,
                    Duration = s.Movie.Duration,
                    Genre = s.Movie.Genre,
                    ReleaseDate = s.Movie.ReleaseDate,
                    Age = s.Movie.Age,
                    Trailer = s.Movie.Trailer,
                    Director = s.Movie.Director,
                    Actor = s.Movie.Actor,
                    Publisher = s.Movie.Publisher,
                    Photo = s.Movie.Photo,
                    CinemaName = s.Cinema.Name,
                    ShowDate = s.ShowDate,
                    ShowTime = s.ShowDate.ToString("HH:mm"),
                    SubId = s.SubId,
                    RoomId = s.RoomId,
                    RoomName = s.Room.Name,
                    SubName = s.Sub.Name,
                    Status = s.Status
                })
                .ToList();
        }

        /// <summary>
        /// Tạo mới một showtime và set active
        /// </summary>
        public bool create(Showtime showtime)
        {
            showtime.Status = true;               // Mặc định active
            db.Showtimes.Add(showtime);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Cập nhật thông tin showtime
        /// </summary>
        public bool update(Showtime showtime)
        {
            db.Showtimes.Update(showtime);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Lấy danh sách showtimes trong ngày mai (dùng cho background service)
        /// </summary>
        public dynamic listMovieTomorrow()
        {
            var tomorrow = DateTime.Today.AddDays(1); // Ngày mai tính từ hôm nay
            return db.Showtimes
                .Where(s => s.Status == true && s.ShowDate.Date == tomorrow)
                .Select(s => new
                {
                    Id = s.Id,
                    Title = s.Movie.Title,
                    Description = s.Movie.Description,
                    Duration = s.Movie.Duration,
                    Genre = s.Movie.Genre,
                    ReleaseDate = s.Movie.ReleaseDate,
                    Age = s.Movie.Age,
                    Trailer = s.Movie.Trailer,
                    Director = s.Movie.Director,
                    Actor = s.Movie.Actor,
                    Publisher = s.Movie.Publisher,
                    Photo = s.Movie.Photo,
                    CinemaName = s.Cinema.Name,
                    ShowDate = s.ShowDate,
                    ShowTime = s.ShowDate.ToString("HH:mm"),
                    SubId = s.SubId,
                    roomId = s.RoomId,
                    RoomName = s.Room.Name,
                    SubName = s.Sub.Name,
                    Status = s.Status
                })
                .ToList();
        }

        public bool deleteAll()
        {
            var showtimes = db.Showtimes.ToList();
            foreach (var showtime in showtimes)
            {
                showtime.Status = false;
            }
            return db.SaveChanges() > 0;
        }

        public bool createRandom()
        {
            const int minShowtimesPerCinema = 2;
            const int maxTotalShowtimesCap = 300;
            const int maxAttemptsPerSlot = 30;

            var random = new Random();
            var now = DateTime.Now;

            var movies = db.Movies.Where(m => m.Status).ToList();
            var cinemas = db.Cinemas.Where(c => c.Status).ToList();
            var subs = db.Subs.Where(s => s.Status).ToList();

            if (!movies.Any() || !cinemas.Any() || !subs.Any())
            {
                return false;
            }

            EnsureActiveRoomForEveryCinema(cinemas);

            var rooms = db.Rooms.Where(r => r.Status).ToList();
            if (!rooms.Any())
            {
                return false;
            }

            var roomsByCinema = rooms
                .GroupBy(r => r.CinemaId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Chỉ rạp có phòng active mới tạo được suất chiếu
            var eligibleCinemas = cinemas
                .Where(c => roomsByCinema.ContainsKey(c.Id) && roomsByCinema[c.Id].Any())
                .ToList();

            if (!eligibleCinemas.Any())
            {
                return false;
            }

            var targetTotalShowtimes = Math.Max(eligibleCinemas.Count * minShowtimesPerCinema, 20);
            targetTotalShowtimes = Math.Min(targetTotalShowtimes, maxTotalShowtimesCap);

            var showtimesToAdd = new List<Showtime>();

            // Tránh tạo trùng phòng + thời gian
            var occupiedSlots = db.Showtimes
                .Where(s => s.Status)
                .Select(s => $"{s.RoomId}_{s.ShowDate:yyyyMMddHHmm}")
                .ToHashSet(StringComparer.Ordinal);

            // Bước 1: đảm bảo mỗi rạp hợp lệ có tối thiểu 1 suất
            foreach (var cinema in eligibleCinemas.OrderBy(_ => random.Next()))
            {
                if (showtimesToAdd.Count >= targetTotalShowtimes)
                {
                    break;
                }

                var roomList = roomsByCinema[cinema.Id];
                if (!roomList.Any())
                {
                    continue;
                }

                for (var attempt = 0; attempt < maxAttemptsPerSlot; attempt++)
                {
                    var movie = movies[random.Next(movies.Count)];
                    var room = roomList[random.Next(roomList.Count)];
                    var sub = subs[random.Next(subs.Count)];
                    var start = GetRandomStart(random, now);
                    var slotKey = $"{room.Id}_{start:yyyyMMddHHmm}";

                    if (occupiedSlots.Contains(slotKey))
                    {
                        continue;
                    }

                    occupiedSlots.Add(slotKey);
                    showtimesToAdd.Add(new Showtime
                    {
                        MovieId = movie.Id,
                        CinemaId = cinema.Id,
                        RoomId = room.Id,
                        SubId = sub.Id,
                        ShowDate = start,
                        Status = true
                    });
                    break;
                }
            }

            // Bước 2: tạo thêm suất chiếu, ưu tiên trải đều theo rạp
            var shuffledCinemas = eligibleCinemas.OrderBy(_ => random.Next()).ToList();
            var cinemaCursor = 0;
            var globalAttempts = 0;
            var maxGlobalAttempts = targetTotalShowtimes * maxAttemptsPerSlot;

            while (showtimesToAdd.Count < targetTotalShowtimes && globalAttempts < maxGlobalAttempts)
            {
                var cinema = shuffledCinemas[cinemaCursor % shuffledCinemas.Count];
                cinemaCursor++;
                globalAttempts++;

                var roomList = roomsByCinema[cinema.Id];
                if (!roomList.Any())
                {
                    continue;
                }

                var movie = movies[random.Next(movies.Count)];
                var room = roomList[random.Next(roomList.Count)];
                var sub = subs[random.Next(subs.Count)];
                var start = GetRandomStart(random, now);
                var slotKey = $"{room.Id}_{start:yyyyMMddHHmm}";

                if (occupiedSlots.Contains(slotKey))
                {
                    continue;
                }

                occupiedSlots.Add(slotKey);
                showtimesToAdd.Add(new Showtime
                {
                    MovieId = movie.Id,
                    CinemaId = cinema.Id,
                    RoomId = room.Id,
                    SubId = sub.Id,
                    ShowDate = start,
                    Status = true
                });
            }

            if (!showtimesToAdd.Any())
            {
                return false;
            }

            db.Showtimes.AddRange(showtimesToAdd);
            return db.SaveChanges() > 0;
        }

        private void EnsureActiveRoomForEveryCinema(List<Cinema> cinemas)
        {
            var allRooms = db.Rooms.ToList();
            var activeCinemaIds = allRooms
                .Where(r => r.Status)
                .Select(r => r.CinemaId)
                .ToHashSet();

            var changed = false;

            foreach (var cinema in cinemas)
            {
                if (activeCinemaIds.Contains(cinema.Id))
                {
                    continue;
                }

                var reusableRoom = allRooms.FirstOrDefault(r => r.CinemaId == cinema.Id && !r.Status);
                if (reusableRoom != null)
                {
                    reusableRoom.Status = true;
                    if (reusableRoom.Quantity <= 0)
                    {
                        reusableRoom.Quantity = 80;
                    }

                    if (string.IsNullOrWhiteSpace(reusableRoom.Name))
                    {
                        reusableRoom.Name = $"AUTO-{cinema.Id}-1";
                    }

                    activeCinemaIds.Add(cinema.Id);
                    changed = true;
                    continue;
                }

                var newRoomName = BuildAutoRoomName(allRooms, cinema.Id);
                var autoRoom = new Room
                {
                    Name = newRoomName,
                    CinemaId = cinema.Id,
                    Quantity = 80,
                    Status = true
                };

                db.Rooms.Add(autoRoom);
                allRooms.Add(autoRoom);
                activeCinemaIds.Add(cinema.Id);
                changed = true;
            }

            if (changed)
            {
                db.SaveChanges();
            }
        }

        private static string BuildAutoRoomName(IEnumerable<Room> rooms, int cinemaId)
        {
            var existingNames = rooms
                .Where(r => r.CinemaId == cinemaId)
                .Select(r => (r.Name ?? string.Empty).Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var index = 1;
            while (true)
            {
                var candidate = $"AUTO-{cinemaId}-{index}";
                if (!existingNames.Contains(candidate))
                {
                    return candidate;
                }

                index++;
            }
        }

        private static DateTime GetRandomStart(Random random, DateTime now)
        {
            // Tạo lịch trong 1-3 ngày tới (theo yêu cầu nghiệp vụ hiện tại)
            var dayOffset = random.Next(1, 4); // 1..3
            var hour = random.Next(9, 23);     // 9..22
            var minute = random.Next(0, 2) * 30;

            var start = now.Date.AddDays(dayOffset).AddHours(hour).AddMinutes(minute);

            // Fallback an toàn, vẫn giữ trong 1-3 ngày tới
            if (start <= now.AddMinutes(30))
            {
                start = now.Date
                    .AddDays(random.Next(1, 4))
                    .AddHours(random.Next(9, 23))
                    .AddMinutes(random.Next(0, 2) * 30);
            }

            return start;
        }
    }
}
