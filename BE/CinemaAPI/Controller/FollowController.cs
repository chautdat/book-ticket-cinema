using cinema.Helpers; // Import các helper chung (nếu có sử dụng)
using cinema.Models; // Định nghĩa model Follow
using cinema.Services; // Import FollowService chứa logic nghiệp vụ
using Microsoft.AspNetCore.Mvc; // Cung cấp Controller và attribute cho API
using System; // Cho Exception và các kiểu cơ bản
using System.Collections.Generic; // Cho List<>
using Twilio; // SDK Twilio (nếu gửi notification qua SMS)
using Twilio.Rest.Api.V2010.Account; // Cho REST resources của Twilio
using Twilio.Types; // Định nghĩa các kiểu dữ liệu Twilio như PhoneNumber

namespace cinema.Controllers
{
    [Route("api/follow")]           // Base route cho các endpoint liên quan đến chức năng follow
    [ApiController]                   // Kích hoạt các tính năng API Controller (model binding, validation)
    public class FollowController : ControllerBase
    {
        private readonly FollowService _followService; // Service xử lý logic follow

        public FollowController(FollowService followService)
        {
            _followService = followService; // Dependency Injection: nhận instance FollowService
        }

        [HttpPost("create")]            // POST /api/follow/create
        [Produces("application/json")] // Trả về JSON
        [Consumes("application/json")] // Nhận JSON từ request body
        public IActionResult Create([FromBody] Follow follow) // Bind payload vào model Follow
        {
            try
            {
                // Gọi service tạo follow (theo dõi phim/rạp), trả về true/false
                bool status = _followService.create(follow);
                return Ok(new { Status = status }); // Trả về trạng thái thành công
            }
            catch (Exception ex)
            {
                // Trả về 400 và message lỗi nếu có exception
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("findById/{id}")]     // GET /api/follow/findById/{id}
        [Produces("application/json")]// Trả về JSON
        public IActionResult FindById(int id) // Lấy id từ route
        {
            try
            {
                // Gọi service tìm follow theo id và trả về đối tượng Follow
                var follow = _followService.findById(id);
                return Ok(follow);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("findAll")]           // GET /api/follow/findAll
        [Produces("application/json")]// Trả về JSON
        public IActionResult FindAll()
        {
            try
            {
                // Gọi service lấy danh sách tất cả follow (có thể theo user hoặc chung)
                var list = _followService.findAll();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
