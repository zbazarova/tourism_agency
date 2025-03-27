using System.ComponentModel.DataAnnotations;

namespace TourismFrontend.Models
{
    public class Tour
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Название тура обязательно")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Страна обязательна")]
        public string Country { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Описание обязательно")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Цена обязательна")]
        [Range(1, 1000000, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Длительность обязательна")]
        [Range(1, 365, ErrorMessage = "Длительность должна быть от 1 до 365 дней")]
        public int Duration { get; set; }
        
        public string ImageUrl { get; set; } = "/images/default-tour.jpg";
        
        [Range(0, 5, ErrorMessage = "Рейтинг должен быть от 0 до 5")]
        public double Rating { get; set; }
        
        public bool IsHot { get; set; }
        
        [Required(ErrorMessage = "Дата отправления обязательна")]
        public DateTime DepartureDate { get; set; } = DateTime.UtcNow.AddDays(7);
        
        [Required(ErrorMessage = "Дата возвращения обязательна")]
        public DateTime ReturnDate { get; set; } = DateTime.UtcNow.AddDays(14);
        
        [Required(ErrorMessage = "Количество мест обязательно")]
        [Range(1, 1000, ErrorMessage = "Количество мест должно быть больше 0")]
        public int AvailableSeats { get; set; }
        
        public string Destination { get; set; } = string.Empty;
        
        public List<string> Inclusions { get; set; } = new();
        public List<string> Exclusions { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string HotelName { get; set; } = string.Empty;
        public string HotelDescription { get; set; } = string.Empty;
        public int HotelStars { get; set; }
        public string RoomDescription { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string TransferInfo { get; set; } = string.Empty;
        public string AdditionalInfo { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public decimal FinalPrice => Price - (Price * Discount / 100);
    }
}
