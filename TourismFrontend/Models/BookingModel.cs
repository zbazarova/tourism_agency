using System.ComponentModel.DataAnnotations;

namespace TourismFrontend.Models
{
    public class BookingModel
    {
        [Required(ErrorMessage = "Выберите тип номера")]
        public int RoomTypeId { get; set; }

        [Required(ErrorMessage = "Укажите количество человек")]
        [Range(1, 10, ErrorMessage = "Количество человек должно быть от 1 до 10")]
        public int NumberOfPeople { get; set; } = 1;

        public string? Comment { get; set; }

        [Required(ErrorMessage = "Добавьте информацию о туристах")]
        [MinLength(1, ErrorMessage = "Добавьте хотя бы одного туриста")]
        public List<TouristInfo> Tourists { get; set; } = new();
    }
} 