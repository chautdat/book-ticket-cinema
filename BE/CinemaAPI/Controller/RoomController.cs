using cinema.Models;               // Import model Room
using cinema.Services;             // Import RoomService chứa logic nghiệp vụ
using Microsoft.AspNetCore.Mvc;     // Cung cấp Controller và attribute cho API
using System;                       // Cho Exception và các kiểu cơ bản

namespace cinema.Controllers
{
    [Route("api/room")]           // Base route cho tất cả endpoint liên quan đến phòng chiếu
    [ApiController]                  // Kích hoạt tính năng API Controller (model binding, validation tự động)
    public class RoomController : ControllerBase
    {
        private readonly RoomService _roomService; // Service xử lý logic Room

        // Dependency Injection: inject RoomService từ container
        public RoomController(RoomService roomService)
        {
            _roomService = roomService;
        }

        // GET /api/room/findAll
        [HttpGet("findAll")]
        [Produces("application/json")]  // Trả về JSON
        public IActionResult FindAll()
        {
            try
            {
                // Gọi service để lấy danh sách tất cả phòng chiếu
                var rooms = _roomService.findAll();
                return Ok(rooms);            // 200 + JSON danh sách
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // 400 + thông báo lỗi
            }
        }

        // POST /api/room/create
        [HttpPost("create")]
        [Produces("application/json")]
        [Consumes("application/json")]  // Nhận JSON request body
        public IActionResult Create([FromBody] Room room)
        {
            room.Status = true;            // Thiết lập phòng active khi tạo mới
            try
            {
                bool status = _roomService.create(room); // Gọi service lưu phòng
                return Ok(new { Status = status });     // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/room/delete/{id}
        [HttpDelete("delete/{id}")]
        [Produces("application/json")]
        public IActionResult Delete(int id)
        {
            var room = _roomService.findById(id);
            if (room == null)
            {
                return NotFound("Không tìm thấy phòng");
            }
            room.Status = false;                  // Đánh dấu inactive (soft delete)
            try
            {
                bool status = _roomService.update(room); // Cập nhật status
                return Ok(new { Status = status });     // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/room/edit
        [HttpPut("edit")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Edit([FromBody] Room room)
        {
            room.Status = true;            // Đảm bảo phòng active khi sửa
            try
            {
                bool status = _roomService.update(room); // Cập nhật thông tin phòng
                return Ok(new { Status = status });     // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/room/findById/{id}
        [HttpGet("findById/{id}")]
        [Produces("application/json")]
        public IActionResult FindById(int id)
        {
            try
            {
                var room = _roomService.findById1(id);
                if (room == null)
                {
                    return NotFound("Không tìm thấy phòng");
                }
                return Ok(room);                        // Trả về object Room
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/room/findByCinemaId/{id}
        [HttpGet("findByCinemaId/{id}")]
        [Produces("application/json")]
        public IActionResult FindByCinemaId(int id)
        {
            try
            {
                var rooms = _roomService.findAllByCinema(id); // Lấy danh sách phòng theo cinemaId
                return Ok(rooms);                             // Trả về danh sách
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
