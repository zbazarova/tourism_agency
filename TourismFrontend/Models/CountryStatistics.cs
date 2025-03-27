namespace TourismFrontend.Models
{
    public class CountryStatistics
    {
        public string Country { get; set; } = string.Empty;
        public int TotalTours { get; set; }
        public decimal AveragePrice { get; set; }
        public int HotTours { get; set; }
        public int TotalBookings { get; set; }
        public double AverageRating { get; set; }
    }
} 