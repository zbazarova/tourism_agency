namespace TourismFrontend.Models
{
    public class BookingCreateDto
    {
        public int TourId { get; set; }
        public int Guests { get; set; }
        public string? Comment { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
} 