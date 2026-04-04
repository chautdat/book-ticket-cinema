namespace cinema.Models
{
    public class ZaloPayCreateRequest
    {
        public int BookingId { get; set; }
        public double Amount { get; set; }
        public string AppUser { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
    }
}
