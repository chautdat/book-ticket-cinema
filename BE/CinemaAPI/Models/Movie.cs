using System;
using System.Collections.Generic;

namespace cinema.Models;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Photo { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Duration { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public DateTime ReleaseDate { get; set; }

    public int Age { get; set; }

    public string Trailer { get; set; } = null!;

    public string Director { get; set; } = null!;

    public string Actor { get; set; } = null!;

    public string Publisher { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<MovieLanguage> MovieLanguages { get; set; } = new List<MovieLanguage>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
