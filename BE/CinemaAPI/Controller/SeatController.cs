using cinema.Models;               // Import model Seat
using cinema.Services;             // Import SeatService chứa logic nghiệp vụ cho Seat
using Microsoft.AspNetCore.Mvc;     // Cung cấp Controller và attribute cho API
using System;                       // Cho Exception và các kiểu cơ bản

namespace cinema.Controllers
{
    [Route("api/seat")]           // Base route cho các endpoint liên quan đến Seat
    [ApiController]                  // Kích hoạt tính năng API Controller: tự động validate và binding
    public class SeatController : ControllerBase
    {
        private readonly SeatService _seatService; // Service xử lý các thao tác với Seat

        // Dependency Injection: nhận instance của SeatService từ container
        public SeatController(SeatService seatService)
        {
            _seatService = seatService;
        }

        // GET /api/seat/findAll
        [HttpGet("findAll")]
        [Produces("application/json")]  // Trả về JSON
        public IActionResult FindAll()
        {
            try
            {
                var seats = _seatService.findAll();   // Lấy danh sách tất cả ghế
                return Ok(seats);                     // 200 + JSON danh sách
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);       // 400 + lỗi
            }
        }

        // POST /api/seat/create
        [HttpPost("create")]
        [Produces("application/json")]
        [Consumes("application/json")]      // Nhận JSON từ request body
        public IActionResult Create([FromBody] Seat seat)
        {
            seat.Status = true;                 // Đánh dấu ghế active khi tạo mới
            try
            {
                bool result = _seatService.create(seat); // Gọi service lưu ghế
                return Ok(new { Status = result });      // Trả về kết quả thành công/không
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/seat/delete/{id}
        [HttpDelete("delete/{id}")]
        [Produces("application/json")]
        public IActionResult Delete(int id)
        {
            var seat = _seatService.findById(id);
            if (seat == null)
            {
                return NotFound("Không tìm thấy ghế");
            }
            seat.Status = false;                  // Đánh dấu inactive (soft delete)
            try
            {
                bool result = _seatService.update(seat); // Cập nhật trạng thái ghế
                return Ok(new { Status = result });      // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/seat/edit
        [HttpPut("edit")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Edit([FromBody] Seat seat)
        {
            seat.Status = true;                 // Đảm bảo ghế active khi cập nhật
            try
            {
                bool result = _seatService.update(seat); // Cập nhật thông tin ghế
                return Ok(new { Status = result });      // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/seat/findById/{id}
        [HttpGet("findById/{id}")]
        [Produces("application/json")]
        public IActionResult FindById(int id)
        {
            try
            {
                var seat = _seatService.findById1(id);
                if (seat == null)
                {
                    return NotFound("Không tìm thấy ghế");
                }
                return Ok(seat);                       // Trả về object Seat
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
