using System;
using System.Collections.Generic;

namespace cinema.Models;

public partial class Cinema
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string District { get; set; } = null!;

    public string City { get; set; } = null!;

    public bool Status { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
