using System.ComponentModel.DataAnnotations;

namespace cinema.Models
{
    public class AccountLoginDto
    {
        [Required]
        public string Email { get; set; }    // hoặc Username nếu bạn login bằng username

        [Required]
        public string Password { get; set; }
    }
}
