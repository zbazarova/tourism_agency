namespace TourismAPI.Models
{
    public class RoomType
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public decimal PriceMultiplier { get; set; }
    }
} 