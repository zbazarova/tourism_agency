using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourismAPI.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;
        
        public DateTime BookingDate { get; set; }
        
        public int Guests { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        
        public BookingStatus Status { get; set; }
        
        public DateTime DepartureDate { get; set; }
        
        public int Adults { get; set; }
        
        public int Children { get; set; }
        
        public int RoomTypeId { get; set; }
        
        public bool IncludeInsurance { get; set; }
        
        public bool IncludeTransfer { get; set; }
    }
} 