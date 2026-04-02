using System;
using System.Collections.Generic;

namespace cinema.Models;

public partial class Follow
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public bool Status { get; set; }

    public virtual Account Account { get; set; } = null!;
}
