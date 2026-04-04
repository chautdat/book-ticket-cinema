using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cinema.Models
{
    [Table("seat_reservation")]
    public class SeatReservation
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("seat_name")]
        [Required]
        [MaxLength(50)]
        public string SeatName { get; set; }

        [Column("showtime_id")]
        [Required]
        public int ShowtimeId { get; set; }

        [Column("session_id")]
        [Required]
        [MaxLength(100)]
        public string SessionId { get; set; }

        [Column("reserved_at")]
        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

        [Column("expires_at")]
        public DateTime ExpiresAt { get; set; }

        // Navigation property
        [ForeignKey("ShowtimeId")]
        public virtual Showtime Showtime { get; set; }
    }
}
