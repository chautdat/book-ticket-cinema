using System;
using System.Collections.Generic;

namespace cinema.Models;

public partial class Seat
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int RoomId { get; set; }

    public int SeatType { get; set; }

    public double Price { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual Room Room { get; set; } = null!;
}
