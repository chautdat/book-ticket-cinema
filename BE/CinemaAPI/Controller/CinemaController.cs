using cinema.Helpers; // Chứa các helper chung (ví dụ gửi mail, xử lý file)
using cinema.Models; // Định nghĩa model Cinema và các model liên quan
using cinema.Services; // Service chứa logic thao tác với Cinema
using Microsoft.AspNetCore.Mvc; // Cung cấp Controller, attribute cho API
using System; // Cho Exception và các kiểu cơ bản
using System.Collections.Generic; // Cho các collection như List<>

namespace cinema.Controllers
{
    [Route("api/cinema")] // Đặt route mặc định cho controller
    [ApiController] // Kích hoạt tính năng API Controller (model validation, binding tự động)
    public class CinemaController : ControllerBase // Dùng ControllerBase vì không trả view
    {
        private readonly CinemaService _cinemaService; // Service xử lý nghiệp vụ Cinema

        public CinemaController(CinemaService cinemaService)
        {
            _cinemaService = cinemaService; // Dependency Injection: nhận instance của CinemaService
        }

        // GET api/cinema/findAll
        [HttpGet("findAll")] // Định nghĩa route và phương thức HTTP
        [Produces("application/json")] // Kết quả trả về dưới dạng JSON
        public IActionResult FindAll()
        {
            try
            {
                // Lấy danh sách tất cả rạp chiếu từ service
                var cinemas = _cinemaService.findAll();
                return Ok(ApiResponse.Success(data: cinemas)); // Trả về mã 200 kèm dữ liệu
            }
            catch (Exception ex)
            {
                // Nếu có lỗi (ví dụ DB không kết nối), trả về mã 400 và thông báo lỗi
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        // PUT api/cinema/update
        [HttpPut("update")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Update([FromBody] Cinema cinema)
        {
            if (cinema == null || cinema.Id <= 0)
            {
                return BadRequest(ApiResponse.Fail("Cinema Id không hợp lệ"));
            }
            try
            {
                var status = _cinemaService.update(cinema);
                return Ok(ApiResponse.Success(status ? "Cập nhật thành công" : "Cập nhật thất bại", new { Status = status }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        // DELETE api/cinema/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ApiResponse.Fail("Cinema Id không hợp lệ"));
            }
            try
            {
                return Ok(ApiResponse.Success(_cinemaService.delete(id) ? "Đã xóa (đặt không hoạt động)" : "Xóa thất bại"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        // POST api/cinema/create
        [HttpPost("create")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Create([FromBody] Cinema cinema)
        {
            if (cinema == null || string.IsNullOrWhiteSpace(cinema.Name) || string.IsNullOrWhiteSpace(cinema.City) || string.IsNullOrWhiteSpace(cinema.District))
            {
                return BadRequest(ApiResponse.Fail("Tên/Thành phố/Quận không được bỏ trống"));
            }
            try
            {
                var status = _cinemaService.create(cinema);
                return Ok(ApiResponse.Success(status ? "Thêm rạp thành công" : "Thêm rạp thất bại", new { Status = status }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }
    }
}
