using cinema.Helpers;
using cinema.Models;               // Import model Showtime
using cinema.Services;             // Import ShowTimeService chứa logic nghiệp vụ cho suất chiếu
using Microsoft.AspNetCore.Mvc;     // Cung cấp Controller, attribute cho API
using System;                       // Cho Exception và DateTime

namespace cinema.Controllers
{
    [Route("api/showTime")]       // Base route cho endpoint liên quan đến suất chiếu
    [ApiController]                  // Kích hoạt tính năng API Controller (tự động binding và validate)
    public class ShowTimeController : ControllerBase
    {
        private readonly ShowTimeService _showTimeService; // Service xử lý nghiệp vụ suất chiếu

        // Dependency Injection: nhận ShowTimeService từ container
        public ShowTimeController(ShowTimeService showTimeService)
        {
            _showTimeService = showTimeService;
        }

        // GET /api/showTime/findById/{id}
        [HttpGet("findById/{id}")]
        [Produces("application/json")]
        public IActionResult FindById(int id)
        {
            try
            {
                var showtime = _showTimeService.findById(id);
                if (showtime == null)
                {
                    return NotFound(ApiResponse.Fail("Không tìm thấy suất chiếu"));
                }
                return Ok(showtime);                         // Trả về object Showtime
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);               // 400 + thông báo lỗi
            }
        }

        // GET /api/showTime/findAll
        [HttpGet("findAll")]
        [Produces("application/json")]
        public IActionResult FindAll()
        {
            try
            {
                var list = _showTimeService.findAll();       // Lấy danh sách tất cả suất chiếu
                return Ok(list);                             // 200 + JSON danh sách
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE /api/showTime/delete/{id}
        [HttpDelete("delete/{id}")]
        [Produces("application/json")]
        public IActionResult Delete(int id)
        {
            var showtime = _showTimeService.findById1(id);
            if (showtime == null)
            {
                return NotFound(ApiResponse.Fail("Không tìm thấy suất chiếu"));
            }
            showtime.Status = false;                        // Đánh dấu suất chiếu inactive
            try
            {
                bool status = _showTimeService.update(showtime); // Cập nhật status
                return Ok(new { Status = status });             // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpDelete("deleteAll")]
        [Produces("application/json")]
        public IActionResult DeleteAll()
        {
            try
            {
                bool status = _showTimeService.deleteAll();
                return Ok(new { Status = status });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("createRandom")]
        [Produces("application/json")]
        public IActionResult CreateRandom()
        {
            try
            {
                bool status = _showTimeService.createRandom();
                return Ok(new { Status = status });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/showTime/checkSeat/{bookingId}
        [HttpGet("checkSeat/{bookingId}")]
        [Produces("application/json")]
        public IActionResult CheckSeat(int bookingId)
        {
            try
            {
                var seats = _showTimeService.checkSeat(bookingId); // Kiểm tra trạng thái ghế cho booking
                return Ok(seats);                                 // Trả về danh sách ghế đã chọn hoặc trống
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/showTime/findAllByCinema/{cinemaId}
        [HttpGet("findAllByCinema/{cinemaId}")]
        [Produces("application/json")]
        public IActionResult FindAllByCinemaId(int cinemaId)
        {
            try
            {
                var list = _showTimeService.findAllByCinemaId(cinemaId); // Lấy suất chiếu theo cinemaId
                return Ok(list);                                         // Trả về danh sách
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST /api/showTime/create
        [HttpPost("create")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Create([FromBody] CreateShowtimeDto dto)
        {
            var showtime = new Showtime
            {
                ShowDate = dto.ShowDate,
                CinemaId = dto.CinemaId,
                RoomId = dto.RoomId,
                MovieId = dto.MovieId,
                SubId = dto.SubId,
                Status = true
            };
            try
            {
                bool status = _showTimeService.create(showtime);
                return Ok(new { Status = status });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /api/showTime/findMovie?date=...&cinemaId=...&movieId=...
        [HttpGet("findMovie")]
        [Produces("application/json")]
        public IActionResult FindMovie(string date, int cinemaId, int movieId)
        {
            try
            {
                var movie = _showTimeService.findMovie(true, date, cinemaId, movieId); // Lấy thông tin suất chiếu của phim
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT /api/showTime/edit
        [HttpPut("edit")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Edit([FromBody] Showtime showtime)
        {
            showtime.Status = true;                            // Đảm bảo active khi chỉnh sửa
            try
            {
                bool status = _showTimeService.update(showtime); // Cập nhật thông tin suất chiếu
                return Ok(new { Status = status });             // Trả về kết quả
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
