using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace cinema.Models
{
    public class CreateMovieDto
    {
        [Required]
        [FromForm(Name = "photo")]
        public IFormFile Photo { get; set; }

        [Required]
        [FromForm(Name = "movie")]
        public string Movie { get; set; }
    }
}
