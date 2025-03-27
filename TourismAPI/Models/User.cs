using System.ComponentModel.DataAnnotations;
using TourismAPI.Constants;

namespace TourismAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        
        public string FirstName { get; set; } = string.Empty;
        
        public string LastName { get; set; } = string.Empty;
        
        public string Phone { get; set; } = string.Empty;
        
        public string Role { get; set; } = UserRoles.Customer; // По умолчанию - покупатель
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
    }
} 