using System.ComponentModel.DataAnnotations;

namespace cinema.Models
{
    public class Email
    {
        [Required]
        [EmailAddress]
        public string To { get; set; } = string.Empty;

        public string Subject { get; set; } = string.Empty;

        public string Body { get; set; } = string.Empty;
    }
}
