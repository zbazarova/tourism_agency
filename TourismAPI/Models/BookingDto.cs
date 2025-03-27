using TourismAPI.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public UserDto? User { get; set; }
    public int TourId { get; set; }
    public TourDto? Tour { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime DepartureDate { get; set; }
    public int Adults { get; set; }
    public int Children { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomTypeName { get; set; } = "Стандартный";
    public bool IncludeInsurance { get; set; }
    public bool IncludeTransfer { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public int NumberOfPeople { get; set; }
} 