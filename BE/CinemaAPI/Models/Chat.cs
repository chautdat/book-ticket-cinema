using System;
using System.Collections.Generic;

namespace cinema.Models;

public partial class Chat
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string Message { get; set; } = null!;

    public int Role { get; set; }

    public DateTime Time { get; set; }

    public virtual Account Account { get; set; } = null!;
}
