using System;
using System.Collections.Generic;

namespace cinema.Models;

public partial class Rating
{
    public int Id { get; set; }

    public string Comment { get; set; } = null!;

    public int Rate { get; set; }

    public int AccountId { get; set; }

    public int MovieId { get; set; }

    public DateTime Created { get; set; }

    public bool Status { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual Movie Movie { get; set; } = null!;
}
