using System.ComponentModel.DataAnnotations;

namespace TourismFrontend.Models
{
    public class TouristInfo
    {
        [Required(ErrorMessage = "Введите имя")]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Введите фамилию")]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Введите дату рождения")]
        public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-18);

        [Required(ErrorMessage = "Введите серию и номер паспорта")]
        [RegularExpression(@"^\d{4}\s?\d{6}$", ErrorMessage = "Неверный формат паспорта")]
        public string Passport { get; set; } = "";

        [Required(ErrorMessage = "Введите гражданство")]
        public string Citizenship { get; set; } = "РФ";
    }
} 