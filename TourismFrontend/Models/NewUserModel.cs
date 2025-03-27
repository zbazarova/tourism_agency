using System.ComponentModel.DataAnnotations;
using TourismFrontend.Constants;

namespace TourismFrontend.Models
{
    public class NewUserModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Имя обязательно")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Фамилия обязательна")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public string Password { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Роль обязательна")]
        public string Role { get; set; } = UserRoles.Customer;
    }
} 