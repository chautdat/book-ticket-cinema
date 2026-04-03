using cinema.Models;                    // Import model Movie
using cinema.Services;                  // Import service chứa nghiệp vụ liên quan đến Movie
using Microsoft.AspNetCore.Mvc;          // Cung cấp Controller và attribute cho API
using cinema.Helpers;                    // Chứa các helper chung như FileHelper
using Microsoft.AspNetCore.Hosting;      // Lấy thông tin môi trường hosting (đường dẫn wwwroot)
using System.Diagnostics;                // (Unused) có thể dùng để logging, debug
using Newtonsoft.Json.Converters;        // Cho IsoDateTimeConverter khi deserialize
using Newtonsoft.Json;                   // Cho JsonConvert
using System;                            // Cho Exception và các kiểu cơ bản
using System.IO;                         // Cho FileStream và Path
using System.Threading.Tasks;

namespace cinema.Controllers
{
    [Route("api/movie")]                // Base route cho các endpoint movie
    [ApiController]                       // Kích hoạt tính năng API Controller (model binding, validation tự động)
    public class MovieController : ControllerBase
    {
        private readonly MovieService _movieService;          // Service xử lý logic Movie
        private readonly IWebHostEnvironment _env;            // Lấy thông tin môi trường web (để lưu ảnh)
        private readonly TmdbMovieSyncService _tmdbMovieSyncService;

        public MovieController(
            MovieService movieService,
            IWebHostEnvironment webHostEnvironment,
            TmdbMovieSyncService tmdbMovieSyncService)
        {
            _movieService = movieService;                     // Dependency Injection: nhận MovieService
            _env = webHostEnvironment;                        // Dependency Injection: nhận hosting environment
            _tmdbMovieSyncService = tmdbMovieSyncService;
        }

        // GET api/movie/findAll?date=...&cinemaId=...
        [HttpGet("findAll")]
        [Produces("application/json")]
        public IActionResult findAll(string date, int cinemaId)
        {
            try
            {
                // Lấy danh sách phim đang chiếu (true) theo ngày và rạp
                var movies = _movieService.findAll(true, date, cinemaId);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  // Trả về 400 + thông báo lỗi
            }
        }

        // GET api/movie/findAllByStatus
        [HttpGet("findAllByStatus")]
        [Produces("application/json")]
        public IActionResult findAll()
        {
            try
            {
                // Lấy tất cả phim theo status=true (đang active)
                var movies = _movieService.findAll(true);
                return Ok(movies);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/movie/findMovieById/{id}
        [HttpGet("findMovieById/{id}")]
        [Produces("application/json")]
        public IActionResult findMovieById(int id)
        {
            try
            {
                // Lấy thông tin chi tiết một phim theo ID
                var movie = _movieService.findById(id);
                if (movie == null)
                {
                    return NotFound("Không tìm thấy phim");
                }
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET api/movie/findMovie?date=...&cinemaId=...&movieId=...
        [HttpGet("findMovie")]
        [Produces("application/json")]
        public IActionResult findMovie(string date, int cinemaId, int movieId)
        {
            try
            {
                // Lấy phim cụ thể theo ngày, rạp và movieId
                var movie = _movieService.findMovie(true, date, cinemaId, movieId);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/movie/syncNowPlaying?maxMovies=20&pages=2
        [HttpPost("syncNowPlaying")]
        [Produces("application/json")]
        public async Task<IActionResult> syncNowPlaying([FromQuery] int maxMovies = 20, [FromQuery] int pages = 2)
        {
            try
            {
                var result = await _tmdbMovieSyncService.SyncNowPlayingAsync(maxMovies, pages);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/movie/create
        // Upload ảnh và thêm mới Movie
        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public IActionResult Create([FromForm] CreateMovieDto dto)
        {
            // Kiểm tra validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // 1. Lưu file ảnh
                var fileName = FileHelper.generateFileName(dto.Photo.FileName);
                var folder = Path.Combine(_env.WebRootPath, "images", fileName);
                using (var stream = new FileStream(folder, FileMode.Create))
                {
                    dto.Photo.CopyTo(stream);
                }

                // 2. Deserialize JSON string thành đối tượng Movie
                var movieObj = JsonConvert.DeserializeObject<Movie>(
                    dto.Movie,
                    new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" }
                );

                // 3. Thiết lập trường bổ sung
                movieObj.Status = true;
                movieObj.Photo = $"{Request.Scheme}://{Request.Host}/images/{fileName}";

                // 4. Gọi service lưu vào DB
                var success = _movieService.create(movieObj);

                return Ok(new { fileName = movieObj.Photo, status = success });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/movie/delete/{id}
        [HttpDelete("delete/{id}")]
        [Produces("application/json")]
        public IActionResult delete(int id)
        {
            // Lấy phim từ DB rồi đánh dấu inactive (Status=false)
            var movie = _movieService.findById1(id);
            if (movie == null)
            {
                return NotFound("Không tìm thấy phim");
            }
            movie.Status = false;
            try
            {
                bool success = _movieService.update(movie); // Cập nhật status
                return Ok(new { Status = success });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/movie/edit
        [HttpPut("edit")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult edit([FromBody] Movie movie)
        {
            movie.Status = true; // Đảm bảo phim active khi edit
            try
            {
                bool success = _movieService.update(movie); // Cập nhật thông tin phim
                return Ok(new { Status = success });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
