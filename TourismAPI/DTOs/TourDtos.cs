using System;
using System.ComponentModel.DataAnnotations;

namespace TourismAPI.DTOs
{
    public class TourDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int AvailableSeats { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public double Rating { get; set; }
        public bool IsHot { get; set; }
    }

    public class TourCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string Destination { get; set; } = string.Empty;
        
        [Required]
        public string Country { get; set; } = string.Empty;
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Duration { get; set; }
        
        [Required]
        public DateTime DepartureDate { get; set; }
        
        [Required]
        public DateTime ReturnDate { get; set; }
        
        [Required]
        [Range(0, int.MaxValue)]
        public int AvailableSeats { get; set; }
        
        public string ImageUrl { get; set; } = string.Empty;
        
        [Range(0, 5)]
        public double Rating { get; set; }
        
        public bool IsHot { get; set; }
    }

    public class TourUpdateDto : TourCreateDto
    {
        // Наследует все свойства от TourCreateDto
    }
} 