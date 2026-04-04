using cinema.Models; // Import các model Combo và ComboDetail
using cinema.Services; // Import ComboService chứa logic nghiệp vụ
using Microsoft.AspNetCore.Mvc; // Cung cấp Controller và attribute cho API
using cinema.Helpers;

namespace cinema.Controllers
{
    [Route("api/combo")] // Base route cho tất cả endpoint của controller này
    public class ComboController : Controller // Kế thừa Controller để hỗ trợ trả view/JSON
    {
        public ComboService comboService; // Service để thao tác với dữ liệu combo

        public ComboController(ComboService _comboService)
        {
            // Dependency Injection: nhận ComboService từ container và gán vào field
            this.comboService = _comboService;
        }

        [HttpGet("findAll")] // GET /api/combo/findAll
        [Produces("application/json")] // Trả về dữ liệu JSON
        public IActionResult findAll()
        {
            try
            {
                // Gọi service lấy danh sách tất cả combo
                var combos = comboService.findAll();
                return Ok(ApiResponse.Success(data: combos)); // Trả về 200 + danh sách
            }
            catch (Exception ex)
            {
                // Nếu xảy ra lỗi, trả về 400 + message chi tiết
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        [HttpPost("create")] // POST /api/combo/create
        [Produces("application/json")]
        [Consumes("application/json")] // Nhận JSON request body
        public IActionResult create([FromBody] Combo combo)
        {
            combo.Status = true; // Mặc định combo mới là active
            if (combo == null || string.IsNullOrWhiteSpace(combo.Name) || combo.Price <= 0)
            {
                return BadRequest(ApiResponse.Fail("Tên combo và giá phải hợp lệ"));
            }
            try
            {
                // Gọi service tạo combo, trả về true/false
                var success = comboService.create(combo);
                return Ok(ApiResponse.Success(success ? "Thêm combo thành công" : "Thêm combo thất bại", new { Status = success }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        [HttpPost("createComboDetails")] // POST /api/combo/createComboDetails
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult createComboDetails([FromBody] ComboDetail comboDetail)
        {
            comboDetail.Status = true; // Mặc định chi tiết combo là active
            if (comboDetail == null || comboDetail.ComboId <= 0 || comboDetail.BookingId <= 0 || comboDetail.Quantity <= 0)
            {
                return BadRequest(ApiResponse.Fail("Thông tin chi tiết combo không hợp lệ"));
            }
            try
            {
                // Gọi service thêm chi tiết combo, trả về true/false
                var success = comboService.createComboDetails(comboDetail);
                return Ok(ApiResponse.Success(success ? "Thêm chi tiết combo thành công" : "Thêm chi tiết combo thất bại", new { Status = success }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        [HttpDelete("delete/{id}")] // DELETE /api/combo/delete/{id}
        [Produces("application/json")]
        public IActionResult delete(int id)
        {
            if (id <= 0) return BadRequest(ApiResponse.Fail("Combo Id không hợp lệ"));
            // Lấy combo theo id
            var combo = comboService.findById(id);
            if (combo == null) return NotFound(ApiResponse.Fail("Combo không tồn tại"));
            combo.Status = false; // Đánh dấu inactive thay vì xóa vật lý
            try
            {
                // Cập nhật trạng thái combo, trả về true/false
                var success = comboService.update(combo);
                return Ok(ApiResponse.Success(success ? "Đã xóa combo (inactive)" : "Xóa combo thất bại", new { Status = success }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        [HttpGet("findById/{id}")] // GET /api/combo/findById/{id}
        [Produces("application/json")]
        public IActionResult findById(int id)
        {
            if (id <= 0) return BadRequest(ApiResponse.Fail("Combo Id không hợp lệ"));
            try
            {
                // Gọi service lấy chi tiết combo theo id
                var combo = comboService.findById1(id);
                if (combo == null) return NotFound(ApiResponse.Fail("Combo không tồn tại"));
                return Ok(ApiResponse.Success(data: combo)); // Trả về object ComboDetail hoặc Combo
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        [HttpPut("edit")] // PUT /api/combo/edit
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult edit([FromBody] Combo combo)
        {
            if (combo == null || combo.Id <= 0 || string.IsNullOrWhiteSpace(combo.Name) || combo.Price <= 0)
            {
                return BadRequest(ApiResponse.Fail("Thông tin combo không hợp lệ"));
            }

            try
            {
                var success = comboService.update(combo);
                return Ok(ApiResponse.Success(success ? "Cập nhật combo thành công" : "Cập nhật combo thất bại", new { Status = success }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }
    }
}
