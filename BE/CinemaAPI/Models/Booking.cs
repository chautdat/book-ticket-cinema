using System;
using System.Collections.Generic;

namespace cinema.Models;

public partial class Booking
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int ShowtimeId { get; set; }

    public DateTime Created { get; set; }

    public bool Status { get; set; }

    public virtual ICollection<BookingDetail> BookingDetails { get; set; } = new List<BookingDetail>();

    public virtual ICollection<ComboDetail> ComboDetails { get; set; } = new List<ComboDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Showtime Showtime { get; set; } = null!;
}
