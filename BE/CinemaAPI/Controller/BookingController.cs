using cinema.Models; // Kiểm định các model dữ liệu Booking và BookingDetail
using cinema.Services; // Chứa lớp BookingService xử lý logic nghiệp vụ
using Microsoft.AspNetCore.Mvc; // Cung cấp Controller và các attribute cho API

namespace cinema.Controllers
{
    [Route("api/booking")] // Đặt đường dẫn cơ sở cho các endpoint liên quan đến booking
    public class BookingController : Controller
    {
        public BookingService bookingService; // Dịch vụ thao tác với booking

        public BookingController(BookingService _bookingService)
        {
            bookingService = _bookingService; // Dependency Injection: nhận BookingService từ container
        }

        [HttpPost("create")] // POST /api/booking/create
        [Produces("application/json")] // Trả về JSON
        [Consumes("application/json")] // Nhận JSON từ request body
        public IActionResult create([FromBody] Booking booking) // Bind dữ liệu vào model Booking
        {
            booking.Created = DateTime.Now; // Gán thời điểm tạo booking
            try
            {
                bool result = bookingService.create(booking); // Gọi service lưu booking, trả về kết quả thành công hay không
                return Ok(new
                {
                    Status = result, // Trả về trạng thái
                    Id = booking.Id, // Trả về ID mới của booking
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Trả về 400 và thông báo lỗi nếu có exception
            }
        }

        [HttpPost("createBookingDetails")] // POST /api/booking/createBookingDetails
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult createBookingDetails([FromBody] BookingDetail booking) // Bind dữ liệu vào BookingDetail
        {
            booking.Status = true; // Đánh dấu chi tiết booking là active
            try
            {
                bool result = bookingService.createBookingDetails(booking); // Gọi service thêm chi tiết booking
                return Ok(new { Status = result }); // Chỉ trả về trạng thái thành công/không
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("findSeatByName/{name}")] // GET /api/booking/findSeatByName/{name}
        [Produces("application/json")]
        public IActionResult findSeatByName(string name) // Lấy tên ghế từ route
        {
            try
            {
                var seat = bookingService.findSeatByName(name); // Tìm ghế theo tên
                if (seat == null)
                {
                    return NotFound(new { Message = "Không tìm thấy ghế" });
                }
                return Ok(new
                {
                    Id = seat.Id, // Trả về ID ghế
                    Price = seat.Price // Trả về giá của ghế
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("findAll")] // GET /api/booking/findAll
        [Produces("application/json")]
        public IActionResult findAll()
        {
            try
            {
                var bookings = bookingService.findAll(); // Lấy danh sách tất cả booking
                return Ok(bookings); // Trả về mảng booking
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("findById/{bookingId}")] // GET /api/booking/findById/{bookingId}
        [Produces("application/json")]
        public IActionResult findById(int bookingId) // Lấy bookingId từ route
        {
            try
            {
                var booking = bookingService.findById(bookingId); // Tìm booking theo ID
                return Ok(booking); // Trả về đối tượng booking
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{bookingId}")] // DELETE /api/booking/delete/{bookingId}
        [Produces("application/json")]
        public IActionResult delete(int bookingId) // Xóa booking theo ID
        {
            try
            {
                bool result = bookingService.delete(bookingId); // Gọi service xóa booking
                return Ok(new { Status = result }); // Trả về trạng thái thành công/không
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
