using cinema.Models;
using cinema.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Diagnostics;
using cinema.Helpers;
using System.ComponentModel.DataAnnotations;

namespace cinema.Controllers
{
    // Đánh dấu đây là một API controller và định nghĩa route base cho các endpoint
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        // Dịch vụ quản lý tài khoản (business logic liên quan đến Account)
        private readonly AccountService _accountService;
        // Constructor dùng Dependency Injection để lấy các service cần thiết
        public AccountController(AccountService accountService)
        {
            _accountService = accountService;
        }

        // POST api/account/login
        // Xử lý đăng nhập bằng email và password
        [HttpPost("login")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Login([FromBody] AccountLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse.Fail("Email và mật khẩu là bắt buộc"));
            }

            try
            {
                var account = _accountService.findByEmail(dto.Email);
                if (account == null)
                {
                    return NotFound(ApiResponse.Fail("Tài khoản không tồn tại"));
                }

                var isValid = _accountService.checkLogin(dto.Email, dto.Password);
                if (!isValid)
                {
                    return Unauthorized(ApiResponse.Fail("Sai mật khẩu hoặc tài khoản"));
                }

                var role = account.Role;
                return Ok(ApiResponse.Success("Đăng nhập thành công", new { 
                    Id = account.Id,
                    Username = account.Username,
                    Email = account.Email,
                    Phone = account.Phone,
                    Birthday = account.Birthday?.ToString("dd/MM/yyyy"),
                    Gender = account.Gender,
                    Role = role,
                    Verify = account.Verify
                }));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.Fail("Lỗi hệ thống", ex.Message));
            }
        }

        // GET api/account/findAll
        // Lấy danh sách tất cả tài khoản
        [HttpGet("findAll")]
        [Produces("application/json")]
        public IActionResult FindAll()
        {
            try
            {
                // Gọi service để truy vấn toàn bộ account
                return Ok(_accountService.findAll());
            }
            catch (Exception ex)
            {
                // Trả về lỗi cùng message chi tiết
                return BadRequest(ex.Message);
            }
        }

        // GET api/account/findById/{id}
        // Lấy thông tin tài khoản theo ID
        [HttpGet("findById/{id}")]
        [Produces("application/json")]
        public IActionResult FindById(int id)
        {
            try
            {
                return Ok(_accountService.findByAccountId(id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST api/account/register
        // Đăng ký tài khoản mới (đã tắt gửi thông báo và xác thực)
        [HttpPost("register")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Register([FromBody] Account account)
        {
            // Mặc định role=1 (user), Verify=1 (tự động xác minh), Created=ngày hiện tại
            account.Role = 1;
            account.Verify = 1;
            account.Created = DateTime.Now;
            account.Securitycode = string.Empty;
            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                
                return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ: " + string.Join(", ", errors)));
            }

            try
            {
                var status = _accountService.register(account);
                return Ok(ApiResponse.Success(
                    status ? "Đăng ký thành công" : "Đăng ký thất bại", 
                    new { Status = status }
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        // POST api/account/sendMail
        // Tính năng gửi email đã tắt
        [HttpPost("sendMail")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult SendMail([FromBody] Email email)
        {
            return Ok(ApiResponse.Fail("Tính năng gửi thông báo đã tắt", false));
        }

        // PUT api/account/update
        // Cập nhật thông tin tài khoản, bao gồm hash mật khẩu mới
        [HttpPut("update")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public IActionResult Update([FromBody] Account account)
        {
            try
            {
                var status = _accountService.update(account);
                return Ok(ApiResponse.Success(status ? "Cập nhật thành công" : "Cập nhật thất bại", new { Status = status }));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        // DELETE api/account/delete/{id}
        [HttpDelete("delete/{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                return Ok(ApiResponse.Success(_accountService.delete(id) ? "Đã xóa (không hoạt động)" : "Xóa thất bại"));
            }
            catch (Exception ex)
            {
                return BadRequest(ApiResponse.Fail(ex.Message));
            }
        }

        // GET api/account/findByEmail/{email}
        // Tìm tài khoản theo email
        [HttpGet("findByEmail/{email}")]
        [Produces("application/json")]
        public IActionResult FindByEmail(string email)
        {
            try
            {
                return Ok(_accountService.findByEmail(email));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
