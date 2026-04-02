using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using cinema.Models;
using cinema.Services;
using System.Security.Principal;
using System;

namespace cinema.Controllers
{
    // Định nghĩa route base cho các endpoint liên quan đến Google OAuth
    [Route("api/google")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Lấy cấu hình từ appsettings (chứa Jwt:Key, Issuer, Audience)
        private readonly IConfiguration _config;
        // Service quản lý tài khoản (tương tác DB)
        private readonly AccountService _accountService;

        // Dependency Injection: inject IConfiguration và AccountService
        public AuthController(IConfiguration config, AccountService accountService)
        {
            _config = config;
            _accountService = accountService;
        }

        // POST api/google/google-login
        // Xác thực người dùng qua Google ID token
        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                // Kiểm tra tính hợp lệ của IdToken với thư viện Google
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);
            }
            catch (Exception ex)
            {
                // Nếu token không hợp lệ, trả về 401 Unauthorized với thông tin lỗi
                return Unauthorized(new { message = "Invalid Google ID token", error = ex.Message });
            }

            // Nếu thành công, sinh JWT cho client
            var token = GenerateJwtToken(payload);
            return Ok(new { token });
        }

        // POST api/google/google-register
        // Đăng ký tài khoản mới từ thông tin Google nếu chưa tồn tại
        [HttpPost("google-register")]
        public async Task<IActionResult> GoogleRegister([FromBody] GoogleLoginDto dto)
        {
            GoogleJsonWebSignature.Payload payload;
            try
            {
                // Validate IdToken
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = "Invalid Google ID token", error = ex.Message });
            }

            // Kiểm tra xem email đã có trong DB chưa
            var existingAccount = await _accountService.GetByEmailAsync(payload.Email);
            if (existingAccount == null)
            {
                // Nếu chưa, tạo mới Account với thông tin từ Google và mật khẩu mặc định
                var random = new Random();
                var newAccount = new Account
                {
                    Username = payload.Name,
                    Email = payload.Email,
                    Password = BCrypt.Net.BCrypt.HashPassword("123"), // Hash mật khẩu tạm
                    Role = 1,
                    Created = DateTime.Now,
                    Verify = 1, // Đánh dấu đã xác minh email
                    Phone = string.Empty,
                    Gender = string.Empty,
                    Birthday = DateTime.Now,
                    Securitycode = random.Next(100000, 1000000).ToString("D6"),
                };

                // Lưu account mới
                await _accountService.CreateAsync(newAccount);
                existingAccount = newAccount;
            }

            // Sau khi có account (cũ hoặc mới), sinh JWT trả về
            var token = GenerateJwtToken(payload);
            return Ok(new { token });
        }

        // Hàm sinh JWT dựa trên payload từ Google
        private string GenerateJwtToken(GoogleJsonWebSignature.Payload payload)
        {
            // Tạo các claim cho token: Subject, Email, Jti
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
                new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Lấy secret key từ cấu hình, tạo SigningCredentials
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo JWT với issuer, audience, claims, thời hạn 30 phút
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            // Trả về chuỗi token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTO nhận IdToken từ client
    public class GoogleLoginDto
    {
        public string IdToken { get; set; }
    }
}
