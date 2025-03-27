using TourismAPI.Models;

namespace TourismAPI.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserDto? User { get; set; }
        public int TourId { get; set; }
        public TourDto? Tour { get; set; }
        public DateTime BookingDate { get; set; }
        public int NumberOfPeople { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
    }

    public class BookingCreateDto
    {
        public int TourId { get; set; }
        public int Guests { get; set; }
        public string? Comment { get; set; }
    }
} 