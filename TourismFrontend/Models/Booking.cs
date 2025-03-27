namespace TourismFrontend.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;
        public int UserId { get; set; }
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; } = null!;
        public int NumberOfPeople { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
    }
}