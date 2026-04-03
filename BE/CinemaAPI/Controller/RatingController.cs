using cinema.Models;               // Import model Rating
using cinema.Services;             // Import RatingService chứa logic thao tác với Rating
using Microsoft.AspNetCore.Mvc;     // Cung cấp Controller, attribute cho API
using System;                       // Cho DateTime và Exception

namespace cinema.Controllers
{
    [Route("api/rating")]         // Base route cho tất cả endpoint liên quan đến rating
    [ApiController]                  // Kích hoạt tính năng API Controller (model binding, validation tự động)
    public class RatingController : ControllerBase
    {
        private readonly RatingService _ratingService; // Service xử lý nghiệp vụ Rating

        // Dependency Injection: inject RatingService từ container
        public RatingController(RatingService ratingService)
        {
            _ratingService = ratingService;
        }

        // POST /api/rating/create
        [HttpPost("create")]
        [Produces("application/json")]  // Trả về JSON
        [Consumes("application/json")]  // Nhận JSON từ request body
        public IActionResult Create([FromBody] Rating rating)
        {
            rating.Created = DateTime.Now; // Gán thời điểm tạo Rating
            rating.Status = true;          // Đánh dấu rating là active
            try
            {
                bool result = _ratingService.create(rating); // Gọi service lưu rating
                return Ok(new { Status = result });         // Trả về kết quả thành công/không
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);                // Trả về 400 và thông báo lỗi
            }
        }

        // GET /api/rating/findAll/{movieId}
        [HttpGet("findAll/{movieId}")]
        [Produces("application/json")]
        public IActionResult FindAll(int movieId)
        {
            try
            {
                var list = _ratingService.findAll(movieId);  // Lấy tất cả rating cho movieId
                return Ok(new { Status = list });            // Trả về danh sách trong trường Status
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/rating/findAllStatus
        [HttpGet("findAllStatus")]
        [Produces("application/json")]
        public IActionResult FindAllStatus()
        {
            try
            {
                var list = _ratingService.findAll();       // Lấy tất cả rating dựa trên status default (true)
                return Ok(new { Status = list });          // Trả về danh sách
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/rating/findAllByMovie/{movieId}
        [HttpGet("findAllByMovie/{movieId}")]
        [Produces("application/json")]
        public IActionResult FindAllByMovie(int movieId)
        {
            try
            {
                var list = _ratingService.findAll(movieId, true); // Lấy rating chỉ với status = true cho movieId
                return Ok(new { Status = list });                 // Trả về danh sách
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/rating/average/{movieId}
        [HttpGet("average/{movieId}")]
        [Produces("application/json")]
        public IActionResult AvgByMovieId(int movieId)
        {
            try
            {
                var avg = _ratingService.avgByMovieId(movieId); // Tính điểm trung bình cho movieId
                return Ok(new { Status = avg });                // Trả về giá trị trung bình
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
