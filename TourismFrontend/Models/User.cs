using System;
using System.ComponentModel.DataAnnotations;

namespace TourismFrontend.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Имя обязательно")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Фамилия обязательна")]
        public string LastName { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string Phone { get; set; } = string.Empty;
        
        public string Role { get; set; } = "Customer";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
