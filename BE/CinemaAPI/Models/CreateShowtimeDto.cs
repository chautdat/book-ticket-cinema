namespace cinema.Models;

public class CreateShowtimeDto
{
    public DateTime ShowDate { get; set; }
    public int CinemaId { get; set; }
    public int RoomId { get; set; }
    public int MovieId { get; set; }
    public int SubId { get; set; }
}
