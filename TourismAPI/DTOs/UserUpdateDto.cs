using System.ComponentModel.DataAnnotations;

namespace TourismAPI.DTOs
{
    public class UserUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        
        [EmailAddress]
        public string? Email { get; set; }
        
        [Phone]
        public string? PhoneNumber { get; set; }
        
        [MinLength(6)]
        public string? Password { get; set; }
    }
} 