using TourismFrontend.Models;

namespace TourismFrontend.Models
{
    public class VacationPlanItem
    {
        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; } = null!;
        public int NumberOfPeople { get; set; }
        public decimal TotalPrice 
        { 
            get => Tour.FinalPrice * NumberOfPeople * RoomType.PriceMultiplier;
            set { } // Для сериализации
        }
    }

    public class VacationPlan
    {
        public List<VacationPlanItem> Items { get; set; } = new();
        
        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
    }
} 