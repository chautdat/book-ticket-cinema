using System;
using System.Collections.Generic;

namespace cinema.Models;

public partial class Language
{
    public int Id { get; set; }

    public string CountryId { get; set; } = null!;

    public string CountryName { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<MovieLanguage> MovieLanguages { get; set; } = new List<MovieLanguage>();
}
