using System;
using System.Collections.Generic;

namespace cinema.Models;

public partial class MovieLanguage
{
    public int Id { get; set; }

    public int MovieId { get; set; }

    public int LanguageId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Duration { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public DateTime ReleaseDate { get; set; }

    public bool Status { get; set; }

    public virtual Language Language { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
