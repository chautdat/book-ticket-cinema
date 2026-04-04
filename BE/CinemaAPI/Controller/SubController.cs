using cinema.Models; // Import model Sub (subscription)
using cinema.Services; // Import SubService chứa logic nghiệp vụ cho subscriptions
using Microsoft.AspNetCore.Mvc; // Cung cấp Controller và attribute cho API
using System; // Cho Exception và các kiểu cơ bản

namespace cinema.Controllers
{
    [Route("api/sub")] // Base route cho các endpoint về subscription
    [ApiController] // Kích hoạt tính năng API Controller: tự động validate và binding
    public class SubController : ControllerBase
    {
        private readonly SubService subService; // Service xử lý nghiệp vụ Sub

        public SubController(SubService _subService) // Constructor: Dependency Injection cho SubService
        {
            subService = _subService;
        }

        [HttpGet("findAll")] // GET /api/sub/findAll: Lấy danh sách subscription
        [Produces("application/json")]
        public IActionResult findAll()
        {
            try
            {
                var list = subService.findAll(); // Gọi service lấy tất cả subscriptions
                return Ok(list); // Trả về 200 + danh sách
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Trả về 400 + message lỗi
            }
        }

        [HttpPost("create")] // POST /api/sub/create: Tạo subscription mới
        [Produces("application/json")]
        [Consumes("application/json")] // Nhận JSON body bind vào Sub model
        public IActionResult create([FromBody] Sub sub)
        {
            sub.Status = true; // Mặc định active
            try
            {
                bool result = subService.create(sub); // Lưu subscription
                return Ok(new { Status = result }); // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Trả về 400 + lỗi
            }
        }

        [HttpDelete("delete/{id}")] // DELETE /api/sub/delete/{id}: Xoá mềm subscription
        [Produces("application/json")]
        public IActionResult delete(int id)
        {
            var sub = subService.findById(id); // Lấy subscription
            if (sub == null)
            {
                return NotFound("Không tìm thấy phụ đề");
            }
            sub.Status = false; // Đánh dấu inactive
            try
            {
                bool result = subService.update(sub); // Cập nhật trạng thái
                return Ok(new { Status = result }); // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Trả về 400 + lỗi
            }
        }

        [HttpPut("edit")] // PUT /api/sub/edit: Cập nhật thông tin subscription
        [Produces("application/json")]
        [Consumes("application/json")] // Nhận JSON body bind vào Sub model
        public IActionResult edit([FromBody] Sub sub)
        {
            sub.Status = true; // Đảm bảo active
            try
            {
                bool result = subService.update(sub); // Cập nhật subscription
                return Ok(new { Status = result }); // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Trả về 400 + lỗi
            }
        }

        [HttpGet("findById/{id}")] // GET /api/sub/findById/{id}: Lấy chi tiết subscription
        [Produces("application/json")]
        public IActionResult findById(int id)
        {
            try
            {
                var sub = subService.findById1(id); // Gọi service lấy subscription theo ID
                if (sub == null)
                {
                    return NotFound("Không tìm thấy phụ đề");
                }
                return Ok(sub); // Trả về 200 + object
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Trả về 400 + lỗi
            }
        }
    }
}
