using System.ComponentModel.DataAnnotations;

namespace TourismAPI.Models
{
    public class Tour
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public decimal Price { get; set; }
        
        public int Duration { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        
        public double Rating { get; set; } = 0;
        
        public bool IsHot { get; set; } = false;
        
        public DateTime DepartureDate { get; set; }
        
        public DateTime ReturnDate { get; set; }
        
        public List<string> Inclusions { get; set; } = new List<string>();
        
        public List<string> Exclusions { get; set; } = new List<string>();
        
        public int AvailableSeats { get; set; }
        
        public string Destination { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string HotelName { get; set; } = string.Empty;
        public string HotelDescription { get; set; } = string.Empty;
        public int HotelStars { get; set; }
        public string RoomDescription { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string TransferInfo { get; set; } = string.Empty;
        public string AdditionalInfo { get; set; } = string.Empty;

        public decimal Discount { get; set; }
        public decimal FinalPrice => Price - (Price * Discount / 100);

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
} 