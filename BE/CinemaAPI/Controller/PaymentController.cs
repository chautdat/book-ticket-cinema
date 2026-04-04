using cinema.Helpers;
using cinema.Models;
using cinema.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace cinema.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentService _paymentService;
        private readonly ZaloPayService _zaloPayService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            PaymentService paymentService,
            ZaloPayService zaloPayService,
            ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _zaloPayService = zaloPayService;
            _logger = logger;
        }

        [HttpGet("findAll")]
        [Produces("application/json")]
        public IActionResult findAll()
        {
            try
            {
                var list = _paymentService.findAll();
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("history")]
        [Produces("application/json")]
        public IActionResult history([FromQuery] string email = "", [FromQuery] string phone = "")
        {
            if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(phone))
            {
                return Ok(ApiResponse.Success("Thiếu email hoặc số điện thoại để tra cứu lịch sử.", new List<object>()));
            }

            try
            {
                var history = _paymentService.findByEmail(email, phone);
                return Ok(ApiResponse.Success("Lấy lịch sử đặt vé thành công", history));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi truy vấn lịch sử đặt vé. email={Email}, phone={Phone}", email, phone);
                return Ok(ApiResponse.Success("Không lấy được lịch sử đặt vé", new List<object>()));
            }
        }

        [HttpPost("create")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult create([FromBody] Payment payment)
        {
            payment.Created = DateTime.Now;
            payment.Status = true;
            payment.TransactionNo ??= $"TXN-{DateTime.Now:yyyyMMddHHmmss}";
            payment.Qr ??= string.Empty;
            payment.Description ??= "Thanh toán vé xem phim";

            try
            {
                bool status = _paymentService.create(payment);
                return Ok(new
                {
                    Status = status,
                    Id = payment.Id
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("findById/{id}")]
        [Produces("application/json")]
        public IActionResult findById(int id)
        {
            try
            {
                var payment = _paymentService.findById(id);
                return Ok(payment);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("by-transaction/{transNo}")]
        [Produces("application/json")]
        public IActionResult getByTransaction(string transNo)
        {
            try
            {
                var payment = _paymentService.findByTransactionNo(transNo);
                if (payment == null)
                {
                    return NotFound(ApiResponse.Fail("Không tìm thấy giao dịch"));
                }

                return Ok(ApiResponse.Success(string.Empty, new
                {
                    payment.Id,
                    payment.BookingId,
                    payment.PaymentType,
                    payment.TransactionNo,
                    payment.Status,
                    payment.Price
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi lấy thông tin giao dịch");
                return StatusCode(500, ApiResponse.Fail("Lỗi hệ thống", ex.Message));
            }
        }

        [HttpPost("sendMail")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult sendMail([FromBody] Email email)
        {
            return Ok(ApiResponse.Fail("Tính năng gửi thông báo đã tắt", false));
        }

        [HttpGet("sendSMS")]
        [Produces("application/json")]
        public IActionResult sendSMS(string body)
        {
            return Ok(ApiResponse.Fail("Tính năng gửi thông báo đã tắt", false));
        }

        [HttpPost("zalopay/create")]
        [Produces("application/json")]
        public async Task<IActionResult> CreateZaloPayOrder([FromBody] ZaloPayCreateRequest req)
        {
            if (req == null || req.Amount <= 0 || string.IsNullOrWhiteSpace(req.AppUser))
            {
                return BadRequest(ApiResponse.Fail("Dữ liệu tạo đơn ZaloPay không hợp lệ"));
            }

            try
            {
                var result = await _zaloPayService.CreateOrderAsync(req.BookingId, req.Amount, req.AppUser, req.ReturnUrl ?? string.Empty);
                return Ok(ApiResponse.Success("Tạo đơn ZaloPay thành công", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create ZaloPay order error");
                return StatusCode(500, ApiResponse.Fail("Tạo đơn ZaloPay thất bại", ex.Message));
            }
        }

        [HttpPost("zalopay/callback")]
        public async Task<IActionResult> ZaloPayCallback([FromForm] Dictionary<string, string> form)
        {
            try
            {
                var ok = await _zaloPayService.HandleCallbackAsync(form);
                if (ok)
                {
                    return Ok(new { return_code = 1, return_message = "ok" });
                }

                return Ok(new { return_code = -1, return_message = "fail" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ZaloPay callback error");
                return Ok(new { return_code = -1, return_message = "error" });
            }
        }

        [HttpGet("zalopay/query/{appTransId}")]
        [Produces("application/json")]
        public async Task<IActionResult> QueryZaloPayOrder(string appTransId)
        {
            if (string.IsNullOrWhiteSpace(appTransId))
            {
                return BadRequest(ApiResponse.Fail("Thiếu appTransId"));
            }

            try
            {
                var result = await _zaloPayService.QueryOrderAsync(appTransId);
                return Ok(ApiResponse.Success("Lấy trạng thái giao dịch thành công", result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Query ZaloPay order error");
                return StatusCode(500, ApiResponse.Fail("Không thể kiểm tra trạng thái ZaloPay", ex.Message));
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                bool success = _paymentService.delete(id);
                if (success)
                {
                    return Ok(ApiResponse.Success("Đã xóa giao dịch"));
                }

                return BadRequest(ApiResponse.Fail("Không tìm thấy giao dịch"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail("Xóa thất bại", ex.Message));
            }
        }
    }
}
